# MULTIPLAYER_SPEC

Status: draft v1

## MVP requirement
Live multiplayer is required for MVP.

## MVP multiplayer scope
- create private match room
- join room by code/invite link if feasible
- both players see same board state
- deterministic turn ownership
- score synchronization
- reconnect and resume support
- end-of-match flow and rematch

## Design principles
- turn-based input with real-time physics resolution is easier to stabilize than fully simultaneous continuous control
- source of truth for whose turn it is must be authoritative
- illegal moves/fouls must be adjudicated consistently
- network architecture should support spectators/replays later if possible, but not at MVP expense
- pre-geometry multiplayer should synchronize semantic match authority, not invented board coordinates

## Authority boundary before geometry
Until canonical layout is known, the network authority should own only what is topology-safe or rule-safe:
- room/match lifecycle
- active side / turn phase / restart obligation
- score and match end
- foul adjudication and awarded restart type
- abstract entity availability / suspension state
- named topology references (`center_restart`, `north_box`, `south_goal_line`, etc.)
- authoritative event order and snapshot versioning

Authority should not require yet:
- final coordinate snapshots for all pieces
- exact penalty spots
- final collision meshes authored from unresolved board geometry

This keeps multiplayer implementable before the layout gate clears.

## Match event contract
Recommended authoritative envelope:

```json
{
  "matchId": "match_123",
  "eventId": "evt_000124",
  "sequence": 124,
  "causationId": "cmd_000057",
  "eventType": "restartAwarded",
  "phase": "restartPending",
  "payload": {
    "restartType": "penaltyRestart",
    "beneficiarySide": "south",
    "originRef": "south_penalty_region",
    "resolutionState": "provisional"
  },
  "stateVersion": 18,
  "stateHash": "abc123",
  "timestampUtc": "2026-03-14T03:24:00Z"
}
```

### Contract rules
- `eventType` names should reflect domain semantics, not transport-specific messages
- `originRef` and similar fields must point to stable semantic IDs, not temporary scene transforms
- `resolutionState` must allow `unresolved`, `provisional`, or `final`
- event ordering is authoritative even if client animation lags behind
- snapshots must be reconstructable from ordered events plus an initial match seed

## Command/event split
Recommended command types:
- `startMatch`
- `submitShotIntent`
- `acknowledgeReconnectState`
- `requestRematch`
- `concedeMatch`

Recommended domain event types:
- `matchStarted`
- `turnBegan`
- `shotIntentAccepted`
- `shotResolutionCommitted`
- `foulCalled`
- `restartAwarded`
- `restartResolved`
- `goalScored`
- `scoreChanged`
- `turnEnded`
- `matchEnded`
- `playerDisconnected`
- `playerReconnected`

The domain event layer is the stable cross-cutting contract for replay, resume, debugging, and later coordinate-backed presentation.

## Snapshot boundary
Recommended snapshot contents before geometry:
- match metadata and participants
- score
- phase / turn owner / shot index
- pending restart obligation
- abstract entity availability state
- last committed event sequence
- topology-readiness / geometry-readiness marker
- optional semantic placement refs for ball/pieces when known

Example fields:
- `activeSide`
- `phase`
- `pendingRestart`
- `entities[].topologyRef`
- `entities[].geometryRef`
- `entities[].resolutionState`
- `geometryReadiness`

## Recommended initial direction
Start with a room-based architecture where:
- room service owns match metadata and authoritative turn state
- clients submit shot events as intent plus current-state hash/version
- authority validates command admissibility before simulation/resolution
- authority emits accepted domain events and resulting snapshot/delta
- clients animate approved result

This may evolve after prototype feel testing.

## Topology-safe validation rules
Before geometry is available, the authority can still reject:
- commands from the wrong side or wrong phase
- duplicate or stale submissions using version/hash mismatch
- restart attempts from the wrong restart class
- actions targeting suspended/unavailable entities
- illegal continuation after a match-ended state

These validations provide useful multiplayer integrity without pretending to know exact coordinates.

## Geometry-dependent validation kept out of scope for now
Do not lock these into the first authority layer yet:
- exact shot origin legality against final anchor coordinates
- exact in-box polygon checks
- exact out-of-bounds collision adjudication tuned to final rails/bands
- exact barrier obstruction validation against canonical scene geometry

Those validations belong behind the later geometry adapter/simulation layer.

## Reconnect/resume implication
Reconnect should restore:
- last committed event sequence
- current authoritative snapshot
- pending restart obligation if any
- any unresolved/provisional semantic refs still awaiting geometry-backed rendering

Clients should be able to rehydrate UI from semantic snapshot data alone.

## Service-stack evaluation criteria for next step
When selecting the actual networking stack, compare candidates against:
- support for authoritative room state and server logic
- simple event ordering/versioning
- reconnect/resume ergonomics
- Unity integration maturity
- cost/ops burden for an MVP private-room game
- ability to evolve from semantic snapshots now to geometry-backed playback later

## Candidate stack comparison (pre-geometry evaluation)
This comparison is intentionally scoped to the current semantic-authority architecture.
It favors candidates that can own room state, event ordering, snapshots, and reconnect flow **before** exact board coordinates or final physics authority are settled.

### Evaluation criteria
- authoritative room state support
- event-log plus reconstructable snapshot fit
- reconnect/resume ergonomics
- Unity integration maturity
- MVP operations burden
- tolerance for unresolved canonical board coordinates
- migration path once geometry-backed resolution exists

### Candidate A — Nakama
Profile:
- open-source backend with realtime multiplayer, RPCs, storage, matchmaking/rooms, and server-authoritative logic options

Fit against current architecture:
- **authoritative room state:** strong fit; room/match state and ordered domain events map cleanly to a server-owned match loop
- **event-log/snapshot fit:** strong; event envelopes and snapshots can be modeled explicitly in custom server logic instead of being forced into object replication semantics
- **reconnect/resume:** good; reconnect can be treated as snapshot + last-sequence resume at the application layer
- **Unity maturity:** good; established Unity client support and common usage in session/room-based games
- **MVP ops burden:** medium; requires running and monitoring backend infrastructure unless a managed offering is chosen
- **tolerance for unresolved geometry:** strong; semantic state can remain authoritative without requiring canonical transform sync
- **migration path later:** strong; can evolve from semantic turn authority now toward geometry-backed resolution later without changing the core room/event model

Primary risks:
- backend ownership is heavier than fully managed room products
- more custom backend design work up front
- team must own correctness of event persistence, resume policy, and abuse/time-out rules

### Candidate B — Photon Fusion
Profile:
- Unity-focused networking stack with strong realtime tooling and room/session patterns, commonly used for authoritative or host-based game state sync

Fit against current architecture:
- **authoritative room state:** moderate to strong; can support authoritative progression, but its center of gravity is runtime state replication rather than a domain-event-first backend model
- **event-log/snapshot fit:** moderate; workable if the project deliberately adds an application-layer event log, but not the most natural fit for event-sourced semantic authority
- **reconnect/resume:** moderate; likely workable for session rejoin and state catch-up, but resumable semantic snapshots would still need explicit product-layer design
- **Unity maturity:** very strong; one of the most Unity-native options in the shortlist
- **MVP ops burden:** low to medium; lighter than self-hosted backend stacks for an MVP
- **tolerance for unresolved geometry:** moderate; acceptable if used mainly for room/session transport and command flow, weaker if leaned on for transform/state replication before geometry is final
- **migration path later:** moderate to strong; especially viable if the project later wants richer realtime shot-resolution sync, but only if early contracts stay domain-first rather than object-first

Primary risks:
- easy to drift toward scene-object/network-transform authority too early
- event sourcing and replay discipline would need to be imposed by project architecture, not inherited naturally
- may encourage solving future physics sync problems before current semantic authority needs are locked

### Candidate C — Unity Gaming Services stack (Lobby/Relay/Netcode family)
Profile:
- Unity-managed service family for session bootstrap, transport/relay, and game networking integration

Fit against current architecture:
- **authoritative room state:** moderate; feasible, but typically requires the project to assemble the authority model from multiple services/components
- **event-log/snapshot fit:** moderate to weak out of the box; semantic event-log persistence is not the natural default and would need explicit application-layer contracts
- **reconnect/resume:** moderate; room rejoin flows are possible, but durable resume semantics remain mostly application-defined
- **Unity maturity:** strong; direct ecosystem alignment is a practical advantage
- **MVP ops burden:** low to medium; attractive for quick Unity-first iteration if product needs remain modest
- **tolerance for unresolved geometry:** moderate; fine if the stack is kept as transport/session plumbing and not mistaken for the source of authoritative gameplay semantics
- **migration path later:** moderate; can work if the team is disciplined, but risks becoming a patchwork of Unity services before the authority model is fully stabilized

Primary risks:
- product architecture may become fragmented across lobby/relay/netcode concerns
- event-first replay/resume contracts would be custom work anyway
- weaker default story for durable match authority than a backend-centric option

### Candidate D — PlayFab-style backend plus custom realtime layer
Profile:
- managed backend/data/auth platform paired with custom transport, relay, or server-authoritative realtime logic

Fit against current architecture:
- **authoritative room state:** moderate to strong depending on the exact realtime component, but not a single obvious pre-geometry fit
- **event-log/snapshot fit:** moderate to strong if built deliberately; backend storage helps, but the event model is still custom
- **reconnect/resume:** moderate to strong with deliberate storage design
- **Unity maturity:** good; widely used in Unity-adjacent production contexts
- **MVP ops burden:** medium; less raw infrastructure than self-hosting, but more product assembly complexity than a tighter single-stack choice
- **tolerance for unresolved geometry:** strong in principle because the semantic model can live above transport
- **migration path later:** strong in principle, but only after more architecture assembly than this MVP likely wants right now

Primary risks:
- too many moving parts for current scope
- easy to overbuild platform concerns before validating the match loop
- shortlist value is more strategic than immediate unless broader backend needs become important

## Authority model shortlist
Current best-fit authority models for the pre-geometry phase:

### 1. Server-authoritative semantic room state
Recommended default.

Shape:
- clients submit commands/intents
- authority validates against phase, side-to-move, restart obligations, and version/hash
- authority commits ordered domain events
- authority publishes snapshots/deltas based on semantic state
- clients animate from accepted results

Why it fits now:
- does not require canonical coordinates to become the source of truth
- best alignment with reconnect/resume and replay
- tolerates unresolved penalty origins and provisional topology refs
- keeps later geometry-backed simulation as an additive layer instead of a rewrite

### 2. Host-authoritative room with server-backed event ledger
Conditional fallback for faster MVP only.

Shape:
- one client acts as temporary room authority
- backend/service still stores ordered event history and snapshots
- reconnect and anti-divergence depend on host continuity or host migration strategy

Why it is only a fallback:
- lighter to start in some stacks
- but adds fairness, disconnect, and host-migration risk exactly where MVP requires reliable private multiplayer
- especially fragile during shot resolution or app backgrounding

### 3. Client-simulated shot resolution with authoritative semantic commit
Promising later hybrid, not first choice for initial MVP foundation.

Shape:
- authority owns legal command acceptance and final committed result
- clients may simulate/preview locally for feel
- once geometry exists, richer resolution details can be incorporated behind the same event contract

Why it belongs later:
- compatible with current semantic authority model
- but depends on future geometry/simulation confidence and should not drive vendor choice prematurely

## Recommended shortlist and decision direction
### Shortlist ranking for current project state
1. **Nakama** — best fit for event-first semantic authority, reconnect/resume, and tolerance for unresolved geometry
2. **Photon Fusion** — strongest Unity-native fallback if the project prioritizes lower-friction realtime integration over backend-centric event architecture
3. **Unity Gaming Services stack** — acceptable Unity-first plumbing option, but less clean as the primary home for durable semantic authority
4. **PlayFab-style assembled backend** — strategically viable, but currently heavier and less focused than needed

### Working recommendation
Adopt **server-authoritative semantic room state** as the target authority model and treat **Nakama** as the leading stack candidate.
Keep **Photon Fusion** as the main fallback if later prototype work shows that Unity-side realtime ergonomics matter more than backend event-model cleanliness for MVP.

This is intentionally a **planning recommendation**, not a locked vendor commitment yet.
The remaining unknown is how much future geometry-backed shot resolution should live server-side versus as client simulation plus authoritative commit.
That question can be deferred without blocking the room/event architecture.

## Risk register by authority approach
### Host-authoritative
Risks:
- disconnect during active turn can strand the room
- host migration is awkward during shot resolution
- fairness/trust model is weaker for competitive multiplayer
- backgrounding on iOS is more dangerous if host is a phone
- resume semantics become coupled to host continuity

Pre-geometry verdict:
- tolerates unresolved coordinates reasonably well
- but operational reliability is worse than desirable for MVP live multiplayer

### Server-authoritative semantic state
Risks:
- more backend work and/or infrastructure ownership
- requires explicit server-side match loop and persistence design
- some later physics-resolution questions remain open

Pre-geometry verdict:
- strongest overall fit
- best replay/reconnect story
- best protection against geometry uncertainty turning into sync debt

### Hybrid client-sim + authoritative commit
Risks:
- divergence risk if visual/local simulation semantics drift from committed authority
- may tempt premature geometry assumptions
- debugging becomes harder if semantic and physical layers are introduced together too early

Pre-geometry verdict:
- good long-term shape
- not ideal as the very first multiplayer foundation while canonical board coordinates remain unresolved

## Open technical decisions
- whether the recommended Nakama-first direction should be elevated from shortlist to formal stack decision after one more implementation-readiness review
- whether future shot resolution should be fully server-resolved or use client simulation plus authoritative semantic commit
- acceptable latency budget for shot submission and resolution
- state sync granularity during ball movement once geometry exists
- how much event history should be durably retained per room for reconnect, dispute/debugging, and replay

## Critical edge cases
- player disconnect during their turn
- player disconnect during shot resolution
- state divergence after collision-rich shots
- duplicate shot submission
- timeout / abandonment
- app backgrounding


## Private-room lifecycle and reconnect policy (authoritative MVP draft)
This section is the authoritative planning target for the private-room MVP.
It is intentionally defined at the semantic room/state layer so it can validate backend choices **without** assuming canonical board coordinates.

### Lifecycle goals
- private-room flow must survive ordinary mobile network loss and common iOS backgrounding cases
- room authority must not depend on one player's device staying foregrounded
- reconnect must restore the exact last committed semantic match state
- timeout/abandon rules must be explicit and symmetric
- the MVP should prefer predictable resumability over trying to hide every disconnect in real time

### Lifecycle phases
Recommended room/match phases:
- **roomCreated**
  - room exists, host/player A reserved a seat, match not armed yet
- **waitingForOpponent**
  - join code active, second seat empty
- **bothPresentNotReady**
  - both seats occupied, at least one player not ready
- **readyToStart**
  - both players connected and marked ready; start command allowed
- **startingMatch**
  - server is seeding the match, assigning first side/turn, emitting initial snapshot
- **activeTurnPendingInput**
  - authoritative state is waiting for the active side to submit a legal shot intent or other allowed command
- **activeTurnResolving**
  - a legal shot/turn command has been accepted and the match is resolving/committing the resulting semantic events
- **restartPending**
  - a restart obligation exists and must be satisfied before ordinary turn continuation resumes
- **pausedForDisconnect**
  - room is temporarily paused because a required player is disconnected during an active match and the grace window is still open
- **abandonedPendingForfeit**
  - disconnect/absence timer expired for one side; room is awaiting final forfeit/abandon confirmation event
- **matchEnded**
  - result is final for the current match
- **rematchPending**
  - one or both players have requested rematch; room remains alive while waiting for mutual consent
- **roomClosed**
  - no further resume/rematch is available for this room

### Room state machine (MVP)
#### Pre-match
- `roomCreated -> waitingForOpponent`
- `waitingForOpponent -> bothPresentNotReady` when second player joins and both seats are bound
- `bothPresentNotReady -> readyToStart` when both players explicitly mark ready
- `readyToStart -> startingMatch` on authoritative `startMatch` acceptance
- `startingMatch -> activeTurnPendingInput` after initial snapshot + `matchStarted` + `turnBegan`

#### In-match
- `activeTurnPendingInput -> activeTurnResolving` when a valid command is accepted
- `activeTurnResolving -> restartPending` if the committed result creates a restart obligation
- `activeTurnResolving -> activeTurnPendingInput` if turn progression continues normally
- `restartPending -> activeTurnPendingInput` once the restart is authoritatively satisfied and next turn begins
- `activeTurnPendingInput|activeTurnResolving|restartPending -> pausedForDisconnect` if a required player disconnects long enough to be considered absent from active play
- `pausedForDisconnect -> activeTurnPendingInput` after reconnect and successful snapshot acknowledgment when no re-resolution is needed
- `pausedForDisconnect -> restartPending` after reconnect if the authoritative state still contains an unresolved restart obligation
- `pausedForDisconnect -> abandonedPendingForfeit` when the disconnect grace window expires
- `abandonedPendingForfeit -> matchEnded` when the server commits an abandonment/forfeit result
- `activeTurnPendingInput|activeTurnResolving|restartPending -> matchEnded` on win condition, concession, or other terminal result

#### Post-match
- `matchEnded -> rematchPending` when either side requests rematch while the room is still retained
- `rematchPending -> startingMatch` when both players request/accept rematch before retention timeout expires
- `matchEnded|rematchPending -> roomClosed` when both players leave, retention expires, or one player declines and the room is no longer retained

### Participation/connection model
Track presence separately from gameplay phase.
Each seat should have at least these semantic states:
- `joined`
- `connected`
- `backgroundedOrUnstable` (optional advisory presence state)
- `disconnected`
- `forfeited`
- `leftRoom`

Important rule:
- **seat occupancy is durable for the retained room, but active-play eligibility depends on live connection status plus acknowledgment of the latest authoritative snapshot.**

## Ready/start policy
### MVP recommendation
Use an explicit two-step pre-match handshake:
1. player joins room and occupies a seat
2. player taps ready
3. match starts only when both are ready and connected

Why:
- reduces accidental starts
- makes reconnect semantics cleaner if someone drops during setup
- gives a stable boundary for match seeding and initial snapshot issuance

### Start command policy
- only the server may transition from `readyToStart` to `startingMatch`
- if a player disconnects after marking ready but before authoritative start commit, clear that player's ready state and return to `bothPresentNotReady`
- the initial match seed must be durable enough that both reconnecting clients can obtain the same first committed state

## Turn ownership and command admissibility
### Authoritative turn ownership
The backend must own:
- active side
- current phase
- whether a restart obligation overrides ordinary turn flow
- last committed sequence/state version
- whether a command window is open

### MVP admissibility rule
At most one gameplay command is accepted into resolution at a time.
The authority rejects:
- commands from the non-active side
- commands during `activeTurnResolving`
- commands against stale state version/hash
- duplicate submissions for the same command window
- restart commands that do not satisfy the pending restart class

This strongly favors backend flows with clear serialized command handling over host-migration-heavy designs.

## Disconnect/background handling on iOS (MVP)
### Design stance
Treat disconnect/backgrounding as **presence interruptions**, not as implicit gameplay actions.
Do not rely on iOS clients remaining authoritative or continuously connected while backgrounded.

### Practical MVP policy
- if a player backgrounds briefly, the room may remain logically active, but the server should quickly classify the seat as unstable/disconnected if heartbeats or socket presence lapse
- if the disconnected player is not required for immediate resolution, the server may wait through a short grace window before formally pausing
- if the disconnected player is required for continued fair play, transition to `pausedForDisconnect`

### Recommended resolution by phase
#### During lobby / before match start
- disconnected player loses ready state
- room remains open under same code while retention window lasts
- reconnect returns them to the lobby state, not into a started match

#### During `activeTurnPendingInput`
- if the **inactive** player disconnects, the match may remain in current phase briefly, but no irreversible match-end or rematch action should depend on their hidden consent
- if the **active** player disconnects, pause the match and start a reconnect grace timer

#### During `activeTurnResolving`
- once the authority has accepted the command and begun resolution, the result must commit server-side even if one or both clients disconnect
- after commit, the room should land in the correct next semantic phase (`activeTurnPendingInput`, `restartPending`, or `matchEnded`)
- reconnecting clients then receive the committed result; do **not** try to roll back a committed resolution because a phone backgrounded

#### During `restartPending`
- if the player who must take the restart disconnects, pause and open grace timer
- if the non-obligated player disconnects, the restart remains pending but no additional action should progress until both sides can observe resumed state or abandonment is triggered

### Grace-window policy
Use explicit timers at the room layer:
- **short reconnect grace window:** enough time for ordinary mobile network wobble/app switching
- **room retention window after match end:** enough time for rematch/review without keeping stale rooms forever

This spec intentionally avoids exact numeric durations until product tuning, but the backend choice must support server-owned timers, not client-local ones.

## Resume semantics
### Resume contract
A reconnecting player must receive:
- current room phase
- current seat/presence map
- latest authoritative snapshot
- last committed event sequence/state version
- pending restart obligation if any
- current timeout/grace deadline if a pause is active
- whether rematch is pending/requested

### Acknowledgment rule
Resume is not complete until the client acknowledges the delivered snapshot/version.
Until then, the player may be connected for transport purposes but not yet eligible to submit gameplay commands.

### Idempotence rule
Reconnect/resume must be idempotent:
- repeated reconnect attempts with the same player identity should produce the same current snapshot and sequence watermark
- duplicate acknowledgments should be harmless
- replay from the last acknowledged sequence should not create duplicate domain events

### MVP resume expectation
The MVP should support **state resume**, not full continuous shot-stream resume.
That means:
- the player returns to the latest committed authoritative semantic state
- if a shot was mid-resolution when they dropped, they resume **after** the committed outcome, not from an exact in-flight physics frame

This is a major point in favor of an event/snapshot backend over a replication-first transport becoming the system of record.

## Timeout, abandon, and forfeit policy
### Distinguish three outcomes
- **temporary pause:** disconnect grace timer still open
- **abandonment:** player failed to return before grace deadline
- **voluntary concession:** player explicitly concedes via command

### Recommended MVP consequences
- abandonment during an active match should resolve as a server-committed forfeit result for the absent side
- abandonment before `matchStarted` should dissolve the room back to pre-match or close it if only one seat remains
- concession is immediate and does not require grace expiration

### Auditability rule
The final result should encode whether the terminal reason was:
- normal win
- concession
- timeout/abandon forfeit
- room dissolved before start

## Rematch policy
### MVP rematch recommendation
- rematch is only available from `matchEnded`
- rematch requires explicit consent from **both** players
- rematch reuses the same private room if still retained
- rematch must create a **new** match instance / event stream, not append new gameplay into the old match history

### Disconnect interaction
- if one player disconnects after match end, the room can remain in `matchEnded` or `rematchPending` until retention expires
- if retention expires before both players consent, close the room

## Minimum backend capabilities required
These are the real must-haves for the MVP authority model.
A backend that cannot support these cleanly is a poor fit even if it has strong Unity transport tooling.

### Must-have
- authoritative room or match process that is **not** hosted on one player's phone
- server-side serialized command acceptance for turn windows
- durable per-room/match state storage sufficient for reconnect resume
- ordered event sequencing or an equivalent monotonic state-version model
- authoritative timers for reconnect grace, abandonment, and room retention
- reconnect flow that can restore a snapshot plus sequence watermark to a returning client
- presence/disconnect hooks at room scope
- ability to keep processing accepted turn resolution even if a client disconnects mid-result
- secure seat identity / rejoin entitlement so the correct player can reclaim their seat
- explicit server-side match-end / forfeit commit path

### Strongly preferred
- append-only event history plus periodic snapshots
- easy custom server logic for room lifecycle/state machine enforcement
- observability/debug tooling for disconnect and duplicate-command incidents
- clean support for invite codes or private-room discovery
- simple way to evolve toward spectators/replay later

### Nice-to-have
- managed hosting / reduced ops burden
- built-in analytics or matchmaking expansion paths
- richer realtime replication for later geometry-backed shot playback

## Pass/fail rubric for current stack recommendation
### Nakama-first remains the best recommendation if most of these stay true
- the project values server-owned room lifecycle/state machine more than drop-in object replication
- reconnect/resume is defined around snapshot + event/version recovery rather than host continuity
- iOS backgrounding is treated as a common interruption that must not strand authority on-device
- accepted shot resolutions need to commit even while clients are absent
- custom room rules/timers/forfeit logic are first-class MVP concerns

### Photon-fallback becomes more attractive if these become dominant
- later prototype work shows the MVP truly needs richer realtime Unity-native replication before backend event durability matters
- resume can be simplified to rejoin-current-session with minimal durable history
- server-owned room timers/state machine can be implemented acceptably without fighting the stack
- operational simplicity and Unity-side ergonomics clearly outweigh the cost of imposing an application-layer event ledger

### Recommendation after lifecycle analysis
Current lifecycle/reconnect analysis **strengthens** the existing recommendation rather than overturning it.
The MVP room policy wants durable server-owned room state, authoritative timers, clean pause/abandon handling, and snapshot-based resume after iOS/background interruptions.
That still points most naturally to **Nakama-first** for the initial architecture, with **Photon Fusion** retained as the main fallback if later hands-on prototyping proves the Unity runtime ergonomics decisively better than the backend-centric event model.

### What would overturn the recommendation later
Only overturn if implementation research demonstrates one of these:
- Nakama materially complicates private-room reconnect/resume beyond acceptable MVP scope
- Photon can supply equivalent server-owned lifecycle guarantees without pushing authority onto client-hosted runtime state
- the product changes from durable room-state authority toward a lighter session/rejoin model

## Resulting planning decision
For pre-geometry MVP planning, the authoritative backend contract should assume:
- server-owned private room lifecycle state machine
- snapshot + sequence based resume
- pause/abandon logic driven by authoritative timers
- no dependence on host migration or client-held authoritative match state

## Sync contract for room resume and timeout state (authoritative MVP draft)
This section converts the lifecycle policy into a contract-level target that can be reviewed for implementation readiness **without** inventing geometry.
Everything here stays at the semantic room/state layer.

### Contract goals
- reconnect must restore the latest committed semantic state, not an in-flight physics frame
- room state must remain server-owned even while one or both clients are absent
- command admissibility after reconnect must depend on snapshot/version acknowledgment
- timeout, abandon, dissolve, and rematch must be expressible from the same room snapshot
- all payloads must tolerate unresolved geometry by using semantic refs plus readiness metadata

### Snapshot payload boundary
A room resume payload should contain three nested layers:
1. **room layer** — lifecycle, seats, timers, rematch, retention
2. **match layer** — current authoritative semantic snapshot for the active match instance
3. **sync layer** — sequence/version/watermark data used for idempotent resume and stale-command rejection

### Recommended room snapshot shape
```json
{
  "roomId": "room_123",
  "roomCode": "ABCD12",
  "roomPhase": "pausedForDisconnect",
  "roomVersion": 42,
  "retention": {
    "status": "retained",
    "expiresAtUtc": "2026-03-14T03:52:00Z"
  },
  "seats": [
    {
      "seat": "north",
      "playerId": "player_a",
      "occupancyState": "joined",
      "presenceState": "disconnected",
      "resumeState": {
        "lastAckedMatchVersion": 18,
        "lastAckedEventSequence": 124,
        "resumeRequired": true,
        "resumeToken": "resume_tok_001"
      }
    },
    {
      "seat": "south",
      "playerId": "player_b",
      "occupancyState": "joined",
      "presenceState": "connected",
      "resumeState": {
        "lastAckedMatchVersion": 19,
        "lastAckedEventSequence": 128,
        "resumeRequired": false,
        "resumeToken": null
      }
    }
  ],
  "pauseContext": {
    "reason": "seatDisconnected",
    "requiredSeat": "north",
    "graceDeadlineUtc": "2026-03-14T03:39:00Z",
    "abandonmentOutcome": "forfeitLoss"
  },
  "rematch": {
    "status": "notAvailable",
    "requestedBy": []
  },
  "match": {
    "matchId": "match_123",
    "matchInstance": 1,
    "phase": "restartPending",
    "activeSide": "north",
    "shotIndex": 7,
    "score": { "north": 2, "south": 3 },
    "pendingRestart": {
      "restartType": "penaltyRestart",
      "beneficiarySide": "north",
      "originRef": "north_penalty_region",
      "resolutionState": "provisional",
      "sourceBasis": "confirmed-rule-concept"
    },
    "entities": {
      "pieces": [
        {
          "id": "north_piece_01",
          "team": "north",
          "availability": "active",
          "topologyRef": "north_defensive_group_a",
          "geometryRef": null,
          "resolutionState": "unresolved"
        }
      ],
      "balls": [
        {
          "id": "ball_01",
          "state": "atRest",
          "topologyRef": "center_restart",
          "geometryRef": null,
          "resolutionState": "provisional"
        }
      ]
    },
    "geometryReadiness": "topology-ready"
  },
  "sync": {
    "snapshotId": "snap_019",
    "stateVersion": 19,
    "lastCommittedEventSequence": 128,
    "lastEventId": "evt_000128",
    "stateHash": "hash_19",
    "resumeRequiresAck": true,
    "replayAvailableFromSequence": 120
  },
  "serverTimeUtc": "2026-03-14T03:36:00Z"
}
```

### Snapshot field rules
- `roomPhase` describes room lifecycle state; `match.phase` describes active match semantics inside the room
- `roomVersion` must advance when room-level lifecycle/rematch/seat state changes, even if gameplay state does not
- `stateVersion` must advance when committed match state changes
- `lastCommittedEventSequence` is the authoritative event-order watermark for replay/resume
- `resumeToken` is a server-issued entitlement proving the seat can acknowledge this snapshot; it is **not** gameplay authority by itself
- `originRef`, `topologyRef`, and `geometryRef` must remain semantic/unresolved-capable until geometry is evidence-safe
- `serverTimeUtc` is included so clients can render timer UI against authoritative deadlines without trusting local clocks

### Minimum seat presence data model
Track seat state with three related but distinct concepts.

#### Occupancy
- `empty`
- `joined`
- `leftRoom`
- `forfeited`

#### Presence
- `connected`
- `backgroundedOrUnstable`
- `disconnected`

#### Resume eligibility
- `synced`
- `resumeRequired`
- `resumeAcknowledging`

Reason:
A player can be transport-connected but still ineligible to submit commands until the latest authoritative snapshot has been acknowledged.

## Reconnect acknowledgment and watermark flow
### Goal
Ensure reconnect is idempotent and stale commands are rejected without requiring continuous object replication.

### Minimum command additions
Add these command types to the earlier command list:
- `resumeSession`
  - client requests authoritative room/match snapshot for its seat identity
- `acknowledgeSnapshot`
  - client confirms it has accepted the delivered snapshot/version/watermark and is ready to re-enter admissible play
- `declineRematch`
  - explicit negative response so rematch retention can close deterministically
- `leaveRoom`
  - explicit post-match or pre-match room exit command distinct from a transient disconnect

Keep existing:
- `startMatch`
- `submitShotIntent`
- `requestRematch`
- `concedeMatch`

### Resume flow (recommended)
1. client reconnects and authenticates seat identity
2. client sends `resumeSession` with last locally known watermark
3. authority returns current room snapshot plus a server-issued `resumeToken`
4. client hydrates local UI from snapshot
5. client sends `acknowledgeSnapshot`
6. authority marks seat `synced` for the delivered `stateVersion` / `lastCommittedEventSequence`
7. only after that acknowledgment may the seat submit gameplay commands

### Suggested `resumeSession` command
```json
{
  "commandId": "cmd_resume_001",
  "commandType": "resumeSession",
  "roomId": "room_123",
  "matchId": "match_123",
  "seat": "north",
  "clientWatermark": {
    "lastKnownStateVersion": 18,
    "lastKnownEventSequence": 124,
    "lastSnapshotId": "snap_018"
  },
  "sentAtUtc": "2026-03-14T03:36:03Z"
}
```

### Suggested `acknowledgeSnapshot` command
```json
{
  "commandId": "cmd_ack_001",
  "commandType": "acknowledgeSnapshot",
  "roomId": "room_123",
  "matchId": "match_123",
  "seat": "north",
  "resumeToken": "resume_tok_001",
  "ackedSnapshotId": "snap_019",
  "ackedStateVersion": 19,
  "ackedEventSequence": 128,
  "ackedRoomVersion": 42,
  "sentAtUtc": "2026-03-14T03:36:05Z"
}
```

### Acknowledgment rules
- `acknowledgeSnapshot` must be rejected if `resumeToken` is expired, seat-mismatched, or already superseded by a newer snapshot
- duplicate acknowledgment for the same seat + `snapshotId` should be accepted idempotently
- if a new committed event lands before acknowledgment, the server may either:
  - issue a newer snapshot and require acknowledgment of the newer watermark, or
  - accept the older acknowledgment but keep `resumeRequired` true until the newest snapshot is acknowledged
- gameplay commands must include the client’s last acknowledged `stateVersion` or `eventSequence`; stale commands are rejected with a resume-required error

### Event additions supporting reconnect flow
Minimum helpful domain events:
- `snapshotAcknowledged`
- `disconnectPauseStarted`
- `disconnectGraceExpired`
- `abandonmentCommitted`
- `rematchRequested`
- `rematchDeclined`
- `roomRetentionExpired`

These are room/domain semantics, not transport diagnostics.

## Timeout and grace-state contract
### Goal
Represent pause/forfeit/rematch retention using explicit server-owned timer records.

### Recommended timer record shape
```json
{
  "timerId": "timer_disconnect_north",
  "timerType": "disconnectGrace",
  "scope": "room",
  "seat": "north",
  "status": "running",
  "startedAtUtc": "2026-03-14T03:34:00Z",
  "deadlineUtc": "2026-03-14T03:39:00Z",
  "expiryAction": "commitAbandonmentForfeit",
  "cancelReason": null
}
```

### Timer classes
- `disconnectGrace`
  - active-match reconnect window for a required player
- `preMatchReadyHold`
  - optional short hold before clearing ready state after lobby disconnect
- `postMatchRetention`
  - room/rematch retention after a finished match
- `rematchConsentWindow`
  - optional explicit deadline for both rematch responses if product tuning wants one

### Timeout/grace data rules
- timers must be server-owned and included in snapshots when relevant
- timer expiry must cause an authoritative room/match event, not merely a silent state flip
- timer consequences must identify whether they affect `roomPhase`, `match.phase`, seat status, or all three
- clients may display countdowns, but deadlines are adjudicated against `serverTimeUtc`

### Abandonment outcome model
Recommended minimum fields on terminal timeout outcome:
```json
{
  "terminalReason": "timeoutForfeit",
  "losingSeat": "north",
  "winningSeat": "south",
  "triggerTimerId": "timer_disconnect_north",
  "committedAtSequence": 129,
  "matchEnded": true,
  "roomRetainedForRematch": false
}
```

## Resume/abandon/rematch command-event matrix
### Resume path
- command: `resumeSession`
- server reply: room snapshot payload
- command: `acknowledgeSnapshot`
- events: `snapshotAcknowledged` and, if applicable, `playerReconnected`

### Abandon/timeout path
- events: `disconnectPauseStarted` -> `disconnectGraceExpired` -> `abandonmentCommitted` -> `matchEnded`
- command alternative: `concedeMatch` may bypass grace and commit immediate terminal result

### Rematch path
- command: `requestRematch`
- event: `rematchRequested`
- command: `declineRematch` or second `requestRematch` from other seat
- events: `rematchDeclined` or `matchStarted` for new `matchInstance`

### New match-instance rule
A rematch must increment `matchInstance`, reset match-scoped `stateVersion`/event stream as a new logical match, and preserve only room-scoped continuity such as seat identity and room code.

## Implementation-readiness checks at the contract layer
These checks intentionally avoid transport/vendor internals and ask only whether a stack can satisfy the contract cleanly.

### Nakama-first readiness checks
A Nakama-first implementation remains contract-ready if it can support all of the following without forcing client-hosted authority:
- server-owned room process or equivalent authoritative room handler
- seat-authenticated resume request returning snapshot + watermark + token
- serialized command handling for `submitShotIntent`, `acknowledgeSnapshot`, `requestRematch`, and timeout-driven transitions
- durable storage for room snapshot, current match snapshot, and at least the latest committed event watermark
- server-owned timers for disconnect grace and post-match retention
- explicit event or callback path for disconnect and reconnect presence changes
- ability to commit accepted resolution even if a client disconnects during the outcome window

### Photon-fallback contract checks
Photon-style fallback remains plausible only if the project can still preserve these contract guarantees at the application layer:
- room authority does not disappear when one player backgrounds or disconnects
- snapshot/version/watermark acknowledgment can gate command admissibility independently of scene-object replication state
- timeout/grace/abandon logic remains server-owned or otherwise not dependent on a foreground host phone
- room-scoped rematch and new-match-instance boundaries are explicit rather than inferred from continued session membership
- event order and terminal timeout reasons can be durably reconstructed for reconnect/debugging

### Contract-level warning signs
Pause and reconsider the stack if any candidate pushes the project toward these anti-patterns:
- transport connection state is treated as proof that a client is safe to submit commands without snapshot acknowledgment
- room continuity depends on host migration during normal iOS backgrounding cases
- reconnect can only restore current object state, not a clear semantic snapshot + version watermark
- timeout/forfeit behavior lives mostly in client code or UI timers
- rematch is modeled as continuing the old event stream instead of starting a fresh match instance

### Current planning verdict
At the contract layer, the new resume/timeout specification still favors **Nakama-first** because the room lifecycle, timer ownership, and snapshot-acknowledgment flow map naturally to a backend-centric semantic authority model.
**Photon Fusion** remains a plausible fallback only if the project is disciplined enough to keep these contracts application-owned rather than letting replication state become de facto authority.


## DTO/package boundary for `Core.Contracts` and `Core.Sync`
This section translates the room-sync contract into implementation-facing boundaries without starting geometry-dependent code.

### Package ownership rule
- **`Core.Contracts`** owns stable domain DTOs, identifiers, enums, and envelope shapes that may be shared by rules, sync, persistence, replay, and Unity integration.
- **`Core.Sync`** owns transport/session orchestration, command validation helpers, snapshot assembly, watermark progression, and persistence adapters built on top of contract types.
- **`Core.Contracts` must not depend on networking vendor SDKs, storage SDKs, or Unity runtime types.**
- **`Core.Sync` may depend on `Core.Contracts` and `Core.Rules`, but must not become the home of geometry-derived gameplay truth.**

### Recommended `Core.Contracts` DTO families
#### Shared identifiers / metadata
- `RoomId`, `RoomCode`, `MatchId`, `MatchInstance`, `SeatId`, `PlayerId`
- `EventId`, `CommandId`, `SnapshotId`, `TimerId`
- `StateVersion`, `RoomVersion`, `EventSequence`
- `ResolutionState`, `EvidenceStatus`, `GeometryReadiness`

#### Room-scoped DTOs
- `RoomSnapshotDto`
- `RoomLifecycleStateDto`
- `SeatStateDto`
- `SeatPresenceDto`
- `SeatResumeStateDto`
- `RoomRetentionDto`
- `PauseContextDto`
- `RematchStateDto`
- `TimerRecordDto`

Room DTO rule:
- room DTOs describe seat occupancy/presence, pause state, timers, retention, and rematch consent
- they must not own shot-by-shot gameplay facts except by referencing the active `MatchSnapshotDto`

#### Match-scoped DTOs
- `MatchSnapshotDto`
- `MatchPhaseDto`
- `ScoreStateDto`
- `TurnStateDto`
- `PendingRestartDto`
- `EntityAvailabilityDto`
- `SemanticEntityStateDto`
- `MatchTerminalOutcomeDto`

Match DTO rule:
- match DTOs describe semantic gameplay authority for one logical match instance
- rematch must create a fresh `MatchId` or fresh `MatchInstance` boundary under the same room, not mutate prior match history in place

#### Sync-scoped DTOs
- `SyncStateDto`
- `ClientWatermarkDto`
- `AcknowledgedWatermarkDto`
- `ResumeTokenDto` or opaque token field carried by command DTOs
- `ReplayWindowDto`
- `CommandEnvelopeDto<TPayload>`
- `DomainEventEnvelopeDto<TPayload>`

Sync DTO rule:
- sync DTOs describe ordering, idempotence, admissibility gating, and replay/snapshot boundaries
- sync DTOs must not redefine room or match semantics already expressed elsewhere

#### Persistence/log DTOs
- `RoomSnapshotRecordDto`
- `MatchSnapshotRecordDto`
- `EventLogRecordDto`
- `TimerRecordDto`
- `SeatResumeCheckpointDto`

Persistence DTO rule:
- record DTOs may mirror contract fields closely, but should preserve explicit scope (`room` vs `match`) so storage does not blur lifecycle history with gameplay history

### Recommended `Core.Sync` namespaces / responsibilities
- `Core.Sync.Room`
  - room lifecycle orchestration
  - seat presence transitions
  - pause/resume/rematch state transitions
- `Core.Sync.Match`
  - match-scoped command admissibility
  - event sequencing
  - snapshot rebuild/projector logic
- `Core.Sync.Persistence`
  - room snapshot store
  - match snapshot store
  - event log store
  - timer checkpoint store
- `Core.Sync.Resume`
  - `resumeSession` / `acknowledgeSnapshot` handlers
  - watermark comparison helpers
  - stale-command rejection helpers
- `Core.Sync.TransportAdapters`
  - vendor-facing adapters for Nakama-first or fallback stacks
  - converts external session/presence/runtime events into contract-level commands/events

### Anti-coupling rule
Do not place these inside the same DTO/package by default:
- room retention/rematch consent and active gameplay turn state
- transport presence callbacks and domain event history
- timer persistence records and UI countdown formatting
- geometry placeholders and authoritative resume watermarks

## Room-scoped vs match-scoped ownership matrix

### Room-scoped authority
Own these at room scope because they can outlive one match instance:
- room lifecycle phase
- room code and room retention status
- seat occupancy and seat identity entitlement
- seat presence / disconnect state
- resume-required vs synced eligibility
- disconnect grace, ready-hold, and post-match retention timers
- rematch requested/declined state
- current active match pointer (`matchId` / `matchInstance`)

Recommended room-scoped events:
- `roomCreated`
- `seatJoined`
- `seatLeftRoom`
- `seatPresenceChanged`
- `disconnectPauseStarted`
- `disconnectGraceExpired`
- `snapshotAcknowledged`
- `roomRetentionExpired`
- `rematchRequested`
- `rematchDeclined`
- `roomClosed`

### Match-scoped authority
Own these at match scope because they belong to one logical game instance only:
- score
- turn owner and shot index
- match phase
- pending restart obligation
- foul/restart/goal/turn events
- semantic entity availability
- terminal outcome for the match
- match state version and event sequence within the active instance

Recommended match-scoped events:
- `matchStarted`
- `turnBegan`
- `shotIntentAccepted`
- `shotResolutionCommitted`
- `foulCalled`
- `restartAwarded`
- `restartResolved`
- `goalScored`
- `scoreChanged`
- `turnEnded`
- `matchEnded`
- `abandonmentCommitted`

### Boundary rule for ambiguous events
Use this split when an event touches both layers:
- if it changes **room participation or resumability**, it is room-scoped
- if it changes **who is winning, whose turn it is, or what gameplay state exists**, it is match-scoped
- if one action affects both, emit one room event and one match event rather than inventing a blended cross-scope event

Examples:
- disconnect during active match:
  - room event: `disconnectPauseStarted`
  - no match event unless gameplay terminal state changes later
- timeout forfeit after grace expiry:
  - room event: `disconnectGraceExpired`
  - match event: `abandonmentCommitted` then `matchEnded`
- rematch acceptance by both seats:
  - room event(s): rematch consent updates
  - match event: new `matchStarted` for the new instance

## Minimum persistence and logging shape
The persistence goal is the smallest durable set that supports reconnect, auditability, and new-match-instance boundaries.

### Room snapshot record
Minimum fields:
- `roomId`
- `roomVersion`
- `roomPhase`
- `activeMatchId`
- `activeMatchInstance`
- seat occupancy/presence/resume states
- pause context
- rematch state
- active timers summary
- `lastUpdatedAtUtc`

When to write:
- seat join/leave
- presence changes that alter pause/resume eligibility
- timer start/cancel/expire
- rematch state changes
- room closure

### Match snapshot record
Minimum fields:
- `roomId`
- `matchId`
- `matchInstance`
- `stateVersion`
- `lastCommittedEventSequence`
- `phase`
- `activeSide`
- `shotIndex`
- `score`
- `pendingRestart`
- semantic entity availability snapshot
- `terminalOutcome` if match ended
- `geometryReadiness`
- `capturedAtUtc`

When to write:
- after `matchStarted`
- after every committed gameplay-changing event batch
- after abandonment/concession/match end
- at rematch/new-instance creation

### Event log record
Minimum fields:
- `roomId`
- `matchId` nullable for room-only events
- `matchInstance` nullable for room-only events
- `eventId`
- `sequence` nullable for room-only ordering if separate room sequence is used
- `roomVersionAtEmit`
- `stateVersionAtEmit` nullable for room-only events
- `scope` = `room` | `match`
- `eventType`
- `causationId`
- `payload`
- `emittedAtUtc`

Logging rule:
- preserve append-only semantics
- do not reuse a prior match event stream for a rematch
- room events may continue across match instances, but match sequences must remain attributable to exactly one match instance

### Resume/watermark checkpoint record
Minimum per-seat fields:
- `roomId`
- `seat`
- `playerId`
- `lastAckedRoomVersion`
- `lastAckedMatchId`
- `lastAckedMatchInstance`
- `lastAckedStateVersion`
- `lastAckedEventSequence`
- `lastAckedSnapshotId`
- `resumeRequired`
- `updatedAtUtc`

Reason:
- this is the minimum durable record needed to reject stale commands safely after reconnect or cross-device resume

### Timer record
Minimum fields:
- `timerId`
- `roomId`
- `matchId` nullable when timer is room-only
- `matchInstance` nullable when timer is room-only
- `scope` = `room` | `match` | `roomMatchBridge`
- `timerType`
- `seat` nullable
- `status`
- `startedAtUtc`
- `deadlineUtc`
- `expiryAction`
- `cancelReason`
- `resolvedByEventId` nullable

Scope rule:
- current MVP timers are mostly room-scoped even when their expiry causes a match outcome
- keep the timer record distinct from the match terminal record so later tuning does not rewrite match history structure

## Rematch and new-match-instance boundary rules
- a rematch must preserve room identity, room code, and seat entitlement when the room is retained
- a rematch must create a fresh match boundary with its own `matchId` or at minimum a strictly incremented `matchInstance`
- match-scoped `stateVersion`, `eventSequence`, snapshot lineage, and terminal outcome must reset for the new match instance
- seat resume checkpoints must update to point at the new active match only after the new initial snapshot is issued
- room-scoped event history may reference multiple `matchInstance` values, but match reconstruction must never need to inspect prior-match gameplay events to rebuild the new match

## Safe implementation-readiness checklist for `Core.Contracts` / `Core.Sync`
Implementation scaffolding is safe to start only if all of these remain true:
- DTOs use semantic refs and readiness metadata, not invented coordinates
- room DTOs and match DTOs are split into separate types/files/namespaces
- room-scoped events and match-scoped events are separately attributable in the log schema
- persistence records include explicit `matchInstance` boundaries for rematch/new-match cases
- resume checkpoints track last acknowledged watermark per seat
- timers are represented as authoritative records, not UI-only countdowns
- transport adapters are optional shells around contract types, not the source of domain truth
- no vendor SDK type leaks into `Core.Contracts`
- no geometry-dependent validation is required for compilation or basic serialization tests

If any proposed scaffold needs exact penalty spots, final anchor coordinates, or network-object replication state to define these contracts, stop and push that work back behind the later geometry/runtime boundary.
