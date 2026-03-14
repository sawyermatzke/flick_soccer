# TECH_ARCHITECTURE

Status: draft v1

## Engine choice
Recommended: **Unity**

Rationale:
- strong 2D physics iteration speed
- practical for touch-first flick gameplay
- good tooling for live multiplayer and future cross-platform expansion
- strong monetization and live-ops integration options
- appropriate for a commercial multiplayer title

## Platform target
- iOS first
- architecture should avoid iOS-only lock-in where unnecessary

## Core architecture principles
- gameplay simulation separated from presentation
- board/rules/theme data externalized where practical
- branded content isolated from generic/default content
- deterministic-enough turn resolution for multiplayer integrity
- no direct dependency between monetization/theming systems and core rules engine
- topology-safe contracts must compile and function before canonical coordinates exist
- geometry-dependent systems must sit behind an explicit adapter seam

## Pre-geometry architecture boundary
Until the board becomes coordinate-ready, architecture should be organized around four explicit boundaries:

1. **Rules/state boundary**
   - owns turn order, score, restart obligations, foul outcomes, and entity availability
   - consumes named semantic references only (`center_restart`, `north_box`, `south_goal_line`)
2. **Topology boundary**
   - owns board-side semantics, zone IDs, edge IDs, restart-origin IDs, and readiness metadata
   - may say that something is in `north_box` or behind midfield without claiming exact coordinates
3. **Geometry adapter boundary**
   - translates topology IDs and unresolved placeholders into concrete coordinates when evidence clears the gate
   - is allowed to be partially empty/provisional until then
4. **Presentation/simulation boundary**
   - Unity scenes, visuals, animation, collision authoring, and input feel
   - must not become the source of truth for rules state

The key rule is that gameplay authority lives above geometry, not inside scene placement.

## Unity package / module plan
Recommended package/module split for safe early implementation:

### `Binho.Core.Contracts`
Purpose:
- canonical DTOs and enums shared by rules, networking, persistence, and adapters

Contents:
- match snapshot contract
- event envelope contract
- foul/restart enums
- topology reference types
- evidence/readiness metadata types

Must not depend on:
- Unity scene objects
- physics objects
- branded assets

### `Binho.Core.Rules`
Purpose:
- pure rules/state machine over contract types

Contents:
- turn progression
- score/game-end logic
- restart resolution state
- foul classification/severity mapping
- abstract entity availability and suspension rules

Must not depend on:
- exact coordinates
- Unity physics
- rendering

### `Binho.Core.Topology`
Purpose:
- data definitions for named zones, restart origins, edge semantics, and board readiness

Contents:
- board topology asset/schema
- zone and edge identifiers
- mirrored-side helpers
- readiness gates (`topology-ready`, `zone-ready`, `coordinate-ready`)

Notes:
- can ship with unresolved geometry fields
- should preserve Confirmed / Inferred / Designed-for-digital metadata

### `Binho.Core.Sync`
Purpose:
- authoritative multiplayer serialization and validation boundary

Contents:
- action submission envelopes
- authoritative event log entries
- snapshot/delta payloads
- state-hash / version helpers
- reconnect/resume contracts

Must depend only on:
- contracts + rules

Additional boundary rule:
- keep room-lifecycle/session orchestration distinct from match-semantic authority even when both are serialized by the sync layer
- treat persistence adapters and transport adapters as `Core.Sync` concerns, but keep the DTO definitions they serialize inside `Binho.Core.Contracts`

### `Binho.Unity.Integration`
Purpose:
- adapters between pure core packages and Unity runtime

Contents:
- ScriptableObject or JSON loaders
- state-to-scene presenter adapters
- replay/event playback bridge
- temporary placeholder geometry provider

Notes:
- this is where topology-safe data first touches Unity objects
- should remain viable even while geometry references are unresolved

### `Binho.Unity.Gameplay`
Purpose:
- runtime gameplay scene systems once coordinates are safe

Contents:
- shot input feel
- collision setup
- board object placement
- camera framing and authored interactions

Blocked until:
- canonical geometry is evidence-safe

### `Binho.Content`
Purpose:
- swappable branding/theme/audio/localization pack

Contents:
- names, logos, board skins, audio callouts, UI visuals, store copy variants

## Contract scope split (implementation-facing)

### `Core/Contracts/Room`
Owns:
- room lifecycle DTOs
- seat occupancy/presence/resume DTOs
- room retention, pause context, rematch state, timer records

### `Core/Contracts/Match`
Owns:
- match snapshot DTOs
- score/turn/restart/terminal-outcome DTOs
- semantic entity availability/state records

### `Core/Contracts/Sync`
Owns:
- command envelopes
- domain event envelopes
- watermark/version/hash DTOs
- replay/resume payload helpers

### `Core/Sync` runtime split
Recommended internal seams:
- `RoomLifecycleService`
- `MatchSyncService`
- `ResumeService`
- `SnapshotProjector`
- `EventLogStore` / `SnapshotStore` / `TimerStore` interfaces
- vendor-specific transport adapters behind those services

## Concrete safe scaffolding plan for `Core.Contracts` and `Core.Sync`
This is the highest-confidence package scaffold that can be created now without assuming canonical coordinates, transport-vendor SDKs, or Unity geometry/runtime authority.

### Target folder / assembly map
Recommended initial source tree:

```text
Assets/
  Binho/
    Core/
      Contracts/
        Assembly/
          Binho.Core.Contracts.asmdef
        Shared/
          Identifiers.cs
          Versioning.cs
          Metadata.cs
          Result.cs
        Room/
          RoomSnapshotDto.cs
          RoomPhase.cs
          SeatStateDto.cs
          SeatPresenceState.cs
          SeatResumeStateDto.cs
          RoomRetentionDto.cs
          PauseContextDto.cs
          RematchStateDto.cs
          TimerRecordDto.cs
        Match/
          MatchSnapshotDto.cs
          MatchPhase.cs
          ScoreStateDto.cs
          TurnStateDto.cs
          PendingRestartDto.cs
          SemanticEntityStateDto.cs
          EntityAvailabilityState.cs
          MatchTerminalOutcomeDto.cs
        Sync/
          CommandEnvelopeDto.cs
          DomainEventEnvelopeDto.cs
          SyncStateDto.cs
          ClientWatermarkDto.cs
          AcknowledgedWatermarkDto.cs
          ReplayWindowDto.cs
          SnapshotReferenceDto.cs
        Persistence/
          RoomSnapshotRecordDto.cs
          MatchSnapshotRecordDto.cs
          EventLogRecordDto.cs
          SeatResumeCheckpointDto.cs
          TimerRecordScope.cs
      Sync/
        Assembly/
          Binho.Core.Sync.asmdef
        Abstractions/
          IRoomLifecycleService.cs
          IMatchSyncService.cs
          IResumeService.cs
          ISnapshotProjector.cs
          IRoomSnapshotStore.cs
          IMatchSnapshotStore.cs
          IEventLogStore.cs
          ITimerStore.cs
          IResumeCheckpointStore.cs
          IClock.cs
          IIdGenerator.cs
        Room/
          RoomLifecycleOrchestrator.cs
          SeatPresencePolicy.cs
          RematchPolicy.cs
          RoomTimerPolicy.cs
        Match/
          MatchCommandGate.cs
          MatchEventSequencer.cs
          MatchSnapshotBuilder.cs
          MatchResumePolicy.cs
        Resume/
          ResumeSessionHandler.cs
          SnapshotAckHandler.cs
          WatermarkComparer.cs
          StaleCommandPolicy.cs
        Persistence/
          InMemory/
            InMemoryRoomSnapshotStore.cs
            InMemoryMatchSnapshotStore.cs
            InMemoryEventLogStore.cs
            InMemoryTimerStore.cs
            InMemoryResumeCheckpointStore.cs
        TransportAdapters/
          README.md
        Tests/
          Contracts/
          Sync/
```

Folder rule:
- `Contracts` contains only stable shapes, enums, small value objects, and record-like DTOs.
- `Sync` contains orchestration/services/store abstractions plus temporary in-memory implementations for serialization and flow testing.
- `TransportAdapters` may exist as an empty seam or README-only placeholder until a backend choice is formally locked.

### Package/service split rules
- `Binho.Core.Contracts`
  - dependency-free except for base .NET libraries
  - safe for reuse by rules, sync, persistence, replay, and Unity integration
  - contains no vendor SDK types, Unity objects, or task-loop behavior
- `Binho.Core.Sync`
  - depends on `Binho.Core.Contracts`
  - may later depend on `Binho.Core.Rules`, but should not require geometry-backed validation to compile
  - owns command admissibility, sequencing, snapshot assembly, checkpoint progression, and timer orchestration
- transport and persistence vendors stay behind interfaces and should be introduced only as adapter implementations, never as DTO field types

### Initial type inventory and naming policy
#### Identifier/value-object layer
Prefer thin immutable wrappers or record structs where Unity/.NET constraints allow:
- `RoomId`, `RoomCode`, `MatchId`, `PlayerId`, `SeatId`
- `CommandId`, `EventId`, `SnapshotId`, `TimerId`
- `StateVersion`, `RoomVersion`, `EventSequence`, `MatchInstance`

If wrapper types create friction in the first scaffold, fallback to strings/ints is acceptable only if kept behind named DTO properties rather than generic dictionaries.

#### Enum and state naming rules
- use nouns for persisted states: `RoomPhase`, `MatchPhase`, `PresenceState`, `TimerStatus`
- use `Dto` suffix for serializable object graphs crossing package boundaries
- use `Record` suffix for persistence-specific shapes
- use `Policy`, `Handler`, `Builder`, `Comparer`, `Store`, `Service`, `Projector` suffixes in `Core.Sync`
- do not encode vendor or transport names into shared contracts

#### Command/event payload split
Recommended `CommandEnvelopeDto<TPayload>` fields:
- `CommandId`
- `CommandType`
- `RoomId`
- optional `MatchId`
- `Seat`
- `ClientWatermark`
- `Payload`
- `SentAtUtc`

Recommended `DomainEventEnvelopeDto<TPayload>` fields:
- `EventId`
- `EventType`
- `Scope`
- `RoomId`
- optional `MatchId`
- optional `MatchInstance`
- optional `Sequence`
- `RoomVersionAtEmit`
- optional `StateVersionAtEmit`
- optional `CausationId`
- `Payload`
- `EmittedAtUtc`

### First scaffold interfaces
Recommended first-pass service interfaces:
- `IRoomLifecycleService`
  - joins/leaves seats
  - applies presence transitions
  - starts/cancels room timers
  - manages rematch state and room closure
- `IMatchSyncService`
  - validates command admissibility
  - appends match-scoped events
  - advances match snapshot and version watermarks
- `IResumeService`
  - issues resume snapshots/tokens
  - accepts snapshot acknowledgments
  - updates seat eligibility checkpoints
- `ISnapshotProjector`
  - projects ordered room/match events into current room/match snapshot records

Recommended first-pass store interfaces:
- `IRoomSnapshotStore`
- `IMatchSnapshotStore`
- `IEventLogStore`
- `ITimerStore`
- `IResumeCheckpointStore`

Store rule:
- keep room snapshot and match snapshot persistence separate even if the first implementation is the same in-memory dictionary provider.

### Safe first implementation order
1. create assembly definitions and empty namespaces
2. scaffold shared identifiers/enums/DTO shells with XML doc comments describing room-vs-match ownership
3. scaffold store/service interfaces
4. add in-memory stores only for contract-flow tests
5. add serialization tests for DTOs/records/envelopes
6. add sync tests for watermark comparison, stale-command rejection, rematch instance reset, and room/match scope separation
7. leave transport adapters as placeholders until backend selection is formally elevated beyond shortlist status

### Serialization and testing checklist
Minimum safe tests for the first scaffold:
- round-trip serialize/deserialize all top-level room, match, sync, and persistence DTOs
- assert room-only events can be logged without `MatchId`/`MatchInstance`
- assert match events require `MatchId` plus `MatchInstance`
- assert rematch creates a new match snapshot lineage and resets match-scoped watermark fields
- assert seat resume checkpoints reject older `StateVersion` / `EventSequence`
- assert timer records survive serialization without UI-only fields
- assert unresolved semantic refs (`topologyRef`, `geometryRef = null`, `resolutionState`) remain legal in snapshots
- assert no DTO references UnityEngine, vendor SDK namespaces, or coordinate-only types

### Deliberate non-goals for the first scaffold
Do not include yet:
- canonical board coordinates
- physics/collision payloads tied to exact geometry
- network object identifiers from a transport SDK
- live backend client/server wiring
- foul validation that depends on exact in-box polygons or anchor slots

If any proposed file/type needs those details, it belongs in a later geometry/runtime/vendor adapter tranche rather than this scaffold.

### Persistence ownership rule
- room snapshots, match snapshots, timer records, and resume checkpoints should be stored separately even if they share a physical database
- explicit scope columns/fields are required so rematch/new-match-instance history never collapses into one undifferentiated stream

### Minimal first implementation tranche spec (`Core.Contracts` / `Core.Sync`)
This section narrows the earlier scaffold map into the smallest code tranche that should actually be created first.
It is intentionally limited to shapes, abstractions, and tests that remain valid before canonical geometry and before backend/vendor lock-in.

#### Tranche objective
Produce a compilable, testable foundation that proves:
- room-scoped vs match-scoped ownership is enforced in code structure
- contract DTOs serialize cleanly without Unity or backend SDK leakage
- sync abstractions can project snapshots and compare watermarks using in-memory adapters only
- rematch/new-match-instance boundaries are testable before any live transport exists

#### Assembly/package creation order
Create in this order so dependencies stay one-directional and safe:
1. `Binho.Core.Contracts`
2. `Binho.Core.Sync`
3. `Binho.Core.Contracts.Tests`
4. `Binho.Core.Sync.Tests`

Dependency rule for this tranche:
- `Binho.Core.Contracts` -> base .NET only
- `Binho.Core.Sync` -> `Binho.Core.Contracts`
- `Binho.Core.Contracts.Tests` -> `Binho.Core.Contracts`
- `Binho.Core.Sync.Tests` -> `Binho.Core.Contracts`, `Binho.Core.Sync`

Do not create dependencies yet on:
- `UnityEngine`
- `Binho.Core.Rules`
- networking SDKs
- persistence SDKs
- geometry/topology runtime packages

#### Minimal asmdef matrix
Recommended first-pass asmdefs:

| asmdef | references | include platforms | notes |
| --- | --- | --- | --- |
| `Binho.Core.Contracts` | none | Any | DTO/value-object only |
| `Binho.Core.Sync` | `Binho.Core.Contracts` | Any | abstractions + pure helpers + in-memory stores |
| `Binho.Core.Contracts.Tests` | `Binho.Core.Contracts` | Editor/Test assemblies as project standard allows | serialization/value-shape tests |
| `Binho.Core.Sync.Tests` | `Binho.Core.Contracts`, `Binho.Core.Sync` | Editor/Test assemblies as project standard allows | flow/projection/watermark tests |

If the repo later adds `Core.Rules`, it should slot between `Contracts` and `Sync`; do not pre-wire that dependency in this tranche.

#### File creation order inside `Core.Contracts`
Create only the shells needed to define the safe contract surface.

##### Step 1 — shared identifiers and metadata
Create first because all later DTOs depend on them:
- `Identifiers.cs`
  - `RoomId`, `RoomCode`, `MatchId`, `PlayerId`, `SeatId`
  - `CommandId`, `EventId`, `SnapshotId`, `TimerId`
- `Versioning.cs`
  - `RoomVersion`, `StateVersion`, `EventSequence`, `MatchInstance`
- `Metadata.cs`
  - `ResolutionState`, `GeometryReadiness`, `EvidenceStatus`, optional `ScopeKind`
- `Result.cs`
  - minimal non-transport result/error wrapper if needed by `Core.Sync`

##### Step 2 — room DTO shells
Create next because room lifecycle outlives any one match instance:
- `RoomSnapshotDto`
- `RoomLifecycleState`
- `SeatStateDto`
- `SeatPresenceState`
- `SeatResumeStateDto`
- `RoomRetentionDto`
- `PauseContextDto`
- `RematchStateDto`
- `TimerRecordDto`

Minimum room-shell rule:
- enough fields to express room phase, room version, seat occupancy/presence/resume, pause state, retention, rematch state, and active match pointer
- no UI formatting fields
- no transport-specific connection objects

##### Step 3 — match DTO shells
Create after room DTOs:
- `MatchSnapshotDto`
- `MatchPhase`
- `ScoreStateDto`
- `TurnStateDto`
- `PendingRestartDto`
- `SemanticEntityStateDto`
- `EntityAvailabilityState`
- `MatchTerminalOutcomeDto`

Minimum match-shell rule:
- enough fields to express phase, active side, score, shot index, pending restart, entity availability, terminal outcome, and geometry readiness
- semantic refs may be strings/enums initially; exact coordinates remain absent/null

##### Step 4 — sync envelope DTO shells
Create after room/match payloads exist:
- `CommandEnvelopeDto<TPayload>`
- `DomainEventEnvelopeDto<TPayload>`
- `SyncStateDto`
- `ClientWatermarkDto`
- `AcknowledgedWatermarkDto`
- `ReplayWindowDto`
- `SnapshotReferenceDto`

Minimum sync-shell rule:
- encode scope, causation, room/match attribution, versions, sequences, and timestamps
- preserve nullable match attribution for room-only events
- avoid generic dictionary payloads where a typed payload shell is feasible

##### Step 5 — persistence record DTO shells
Create last within contracts:
- `RoomSnapshotRecordDto`
- `MatchSnapshotRecordDto`
- `EventLogRecordDto`
- `SeatResumeCheckpointDto`
- `TimerRecordScope`

Persistence-shell rule:
- persistence DTOs may mirror contract DTOs, but must preserve explicit scope and match-instance boundaries

#### First service/store interfaces to create in `Core.Sync`
Create interfaces before any concrete orchestration logic.

##### Services
- `IRoomLifecycleService`
- `IMatchSyncService`
- `IResumeService`
- `ISnapshotProjector`

Minimum interface responsibility:
- `IRoomLifecycleService`: room seat/presence/rematch/timer state transitions
- `IMatchSyncService`: admissibility, event append intent, snapshot advancement
- `IResumeService`: resume payload issuance + snapshot acknowledgment handling
- `ISnapshotProjector`: ordered event log -> room/match snapshot projection

##### Stores
- `IRoomSnapshotStore`
- `IMatchSnapshotStore`
- `IEventLogStore`
- `ITimerStore`
- `IResumeCheckpointStore`
- `IClock`
- `IIdGenerator`

Store/interface rule:
- signatures should prefer contract DTOs and named IDs over transport-native primitives
- no async transport semantics need to be forced yet if pure sync signatures are clearer for the first tranche

#### In-memory adapter scope for this tranche
In-memory implementations are allowed only to prove contract-flow behavior and testability.
Create:
- `InMemoryRoomSnapshotStore`
- `InMemoryMatchSnapshotStore`
- `InMemoryEventLogStore`
- `InMemoryTimerStore`
- `InMemoryResumeCheckpointStore`
- simple deterministic test doubles for `IClock` and `IIdGenerator`

Do not create yet:
- SQLite/file persistence adapters
- Nakama/Photon transport adapters
- Unity `ScriptableObject` loaders
- background worker loops or retry infrastructure

#### Minimal concrete helper classes allowed in `Core.Sync`
Keep orchestration concrete code small in this tranche.
Safe first helpers:
- `WatermarkComparer`
- `StaleCommandPolicy`
- `MatchSnapshotBuilder`

Optional if needed by tests, but still small and pure:
- `SnapshotAckHandler`
- `MatchEventSequencer`

Defer broader orchestrators unless tests require them.
The goal is a narrow thin slice, not a fake-complete sync runtime.

#### DTO-shell priority order
Highest-value shell order for immediate coding:
1. identifiers/versioning metadata
2. `RoomSnapshotDto`
3. `MatchSnapshotDto`
4. `ClientWatermarkDto` + `AcknowledgedWatermarkDto`
5. `CommandEnvelopeDto<TPayload>`
6. `DomainEventEnvelopeDto<TPayload>`
7. `EventLogRecordDto`
8. `SeatResumeStateDto` + `SeatResumeCheckpointDto`
9. `TimerRecordDto`
10. `PendingRestartDto` + `RematchStateDto`

Why this order:
- it enables the first serialization tests and room-vs-match boundary tests as early as possible
- it proves resume/watermark semantics without needing the whole surface area at once

#### Minimum test matrix for the tranche
The first code tranche should ship with tests for four themes only.

##### 1. Serialization boundary tests
Cover:
- `RoomSnapshotDto`
- `MatchSnapshotDto`
- sync envelopes/watermarks
- persistence record DTOs

Assertions:
- round-trip serialization preserves IDs, versions, nullability, and scope attribution
- unresolved semantic refs remain legal (`geometryRef = null`, `resolutionState != final`)
- no DTO requires Unity/vendor-specific converters

##### 2. Watermark / stale-command tests
Cover:
- watermark comparison ordering
- stale acknowledgment rejection/acceptance rules
- stale command detection from older `StateVersion` / `EventSequence`

Assertions:
- newer watermark dominates older watermark even if only one dimension advances
- duplicate acknowledgment for same snapshot is idempotent
- older command watermark is rejected when a later acknowledged snapshot exists

##### 3. Rematch / match-instance boundary tests
Cover:
- fresh match lineage under retained room
- reset of match-scoped snapshot/event watermarks on rematch
- preservation of room-scoped continuity across rematch

Assertions:
- room identity and seat identity persist
- match instance increments or match ID changes according to chosen shape
- prior match event log is not reused as the active match stream

##### 4. Room-vs-match scope separation tests
Cover:
- room-only events without `MatchId`
- match events requiring `MatchId` and `MatchInstance`
- persistence store separation between room snapshots and match snapshots
- timer scope survival through serialization

Assertions:
- room presence/rematch/timer events remain valid without match attribution
- gameplay events fail validation if match attribution is omitted
- timer records do not collapse room-scope and match-scope histories together

#### Explicit non-goals for this first code tranche
Do not include in code yet:
- board coordinates or anchor constants
- exact penalty origin positions
- shot physics payloads
- foul adjudication requiring in-box polygons
- runtime scene projection
- vendor selection or adapter code beyond placeholder docs

#### Ready-to-code exit criteria for this tranche
This tranche is complete when:
- all four asmdefs compile
- DTO shells exist for room/match/sync/persistence minimum surfaces
- store/service interfaces exist
- in-memory adapters support the planned tests
- serialization, watermark, rematch, and room-vs-match separation tests pass
- no assembly references Unity runtime or backend SDK namespaces

## Planned code domains
- `Core/Contracts` — stable event/state DTOs and shared identifiers
- `Core/Rules` — turn system, scoring, fouls, match state
- `Core/Topology` — topology/zones/restart references, readiness-aware board data
- `Core/Sync` — room/session state envelopes, hashes, reconnect contracts
- `Core/BoardGeometry` — canonical geometry records and coordinate adapters once unblocked
- `Core/Simulation` — flick resolution, physics tuning wrappers, shot lifecycle
- `Unity/Integration` — loaders/adapters/presenters from abstract state into runtime
- `UI` — menus, HUD, tutorial, lobby
- `Content` — themes, branding packs, localization, audio
- `Services` — analytics, purchases, crash reporting, config

## Authoritative state/event model
Recommended pre-geometry authority chain:
- **client** submits intent/action envelope
- **authority** validates against current match phase, topology-safe rules, and version/hash
- **authority** emits accepted domain events
- **authority** publishes resulting snapshot or delta
- **clients** render/present the accepted result

This keeps multiplayer, replay, and reconnect aligned around the same contracts.

### Event-first implication
Prefer an append-only authoritative event log plus reconstructable snapshots over scene-driven state mutation.

Why:
- geometry is unresolved, but semantic match events are already stable enough to define
- reconnect/resume and replay are easier if state changes are expressed as events
- later coordinate-backed playback can consume the same event stream without changing match authority

## Adapter seam between abstract state and later coordinates
The seam should be explicit and narrow.

Recommended interfaces:
- `ITopologyCatalog`
  - resolves named zones, edges, and restart origins
- `IGeometryCatalog`
  - resolves canonical coordinates only when available
- `IStateProjector`
  - maps authoritative match state to view-facing placement intents
- `IRuntimeBoardAdapter`
  - instantiates/moves Unity objects from placement intents without owning rules

Pre-geometry contract rule:
- `IStateProjector` may output semantic placement intents such as `center_restart` or `north_defensive_group_a`
- `IRuntimeBoardAdapter` must tolerate unresolved geometry and render placeholders, hidden objects, or non-final debug layout instead of fabricating coordinates

## Multiplayer architecture direction
Live multiplayer is MVP, so networking must be designed early.

Working direction:
- authoritative or semi-authoritative room host/service for official state progression
- clients submit shot input/intention
- match state updates validated and broadcast
- reconnect support required
- private rooms first; matchmaking can come later
- authority contract must be valid before final board coordinates are authored

Exact service selection TBD in `MULTIPLAYER_SPEC.md`.

## Tranche plan
1. contracts only
   - define shared DTOs/enums/interfaces in `Core/Contracts`
2. pure rules/topology
   - implement rules engine and topology/readiness assets with null/unresolved geometry allowed
3. sync shell
   - implement authoritative event/snapshot envelopes and validation/versioning rules
4. Unity integration shell
   - load topology-safe state into placeholder presentation without canonical placement
5. coordinate-backed gameplay
   - enable final geometry adapter, authored board placement, and tuned collision only after evidence gate clears

## Test strategy
- unit tests for rules engine
- contract serialization tests for snapshots/events
- projector/adapter tests that confirm unresolved geometry does not crash integration
- sync/state tests for multiplayer edge cases
- manual feel testing for shot controls and collisions once geometry is safe

## Asset/branding isolation
- all names, logos, board skins, audio callouts, team art, store copy variants should be stored in a swappable content layer
- core gameplay must still run with a completely generic fallback pack
