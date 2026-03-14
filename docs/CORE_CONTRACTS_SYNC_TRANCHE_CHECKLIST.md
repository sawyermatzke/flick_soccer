# Core.Contracts / Core.Sync — First Coding Checklist and Fixture Inventory

Status: planning draft v1

This document converts the documented minimal `Core.Contracts` / `Core.Sync` implementation tranche into a code-execution checklist.
It stays strictly inside the pre-geometry, semantic-authority architecture.

## Scope guardrails
- Do not invent canonical coordinates, anchor maps, penalty spots, or exact board geometry.
- Preserve **Confirmed / Inferred / Designed-for-digital** distinctions when fields need evidence metadata.
- Keep room lifecycle authority separate from match-semantic authority.
- Do not introduce Unity runtime types, transport SDK types, or persistence SDK types into `Core.Contracts`.
- Do not start geometry-backed legality validation.

## Assembly and namespace plan

### Assembly creation order
1. `Binho.Core.Contracts`
2. `Binho.Core.Sync`
3. `Binho.Core.Contracts.Tests`
4. `Binho.Core.Sync.Tests`

### Recommended namespace map
| folder | namespace | notes |
| --- | --- | --- |
| `Assets/Binho/Core/Contracts/Shared` | `Binho.Core.Contracts.Shared` | identifiers, versions, metadata, simple result types |
| `Assets/Binho/Core/Contracts/Room` | `Binho.Core.Contracts.Room` | room lifecycle, seat, timer, rematch DTOs |
| `Assets/Binho/Core/Contracts/Match` | `Binho.Core.Contracts.Match` | match snapshot, score, turn, restart, entity DTOs |
| `Assets/Binho/Core/Contracts/Sync` | `Binho.Core.Contracts.Sync` | command/event envelopes, watermarks, replay/snapshot refs |
| `Assets/Binho/Core/Contracts/Persistence` | `Binho.Core.Contracts.Persistence` | record DTOs for snapshots/event log/checkpoints |
| `Assets/Binho/Core/Sync/Abstractions` | `Binho.Core.Sync.Abstractions` | service/store interfaces, clock/id contracts |
| `Assets/Binho/Core/Sync/Resume` | `Binho.Core.Sync.Resume` | watermark/stale-command helpers, ack helpers |
| `Assets/Binho/Core/Sync/Validation` | `Binho.Core.Sync.Validation` | command/event scope validators kept separate from resume and match builders |
| `Assets/Binho/Core/Sync/Match` | `Binho.Core.Sync.Match` | snapshot builder, event sequencer, match admissibility helpers |
| `Assets/Binho/Core/Sync/Room` | `Binho.Core.Sync.Room` | room/rematch/timer/presence policies |
| `Assets/Binho/Core/Sync/Persistence/InMemory` | `Binho.Core.Sync.Persistence.InMemory` | test-only in-memory adapters |
| `Assets/Binho/Core/Contracts/Tests` | `Binho.Core.Contracts.Tests` | DTO serialization and shape tests |
| `Assets/Binho/Core/Sync/Tests` | `Binho.Core.Sync.Tests` | watermark/rematch/scope separation tests |

### asmdef reference table
| asmdef | references | must not reference |
| --- | --- | --- |
| `Binho.Core.Contracts` | none | `UnityEngine`, `Binho.Core.Rules`, vendor SDKs |
| `Binho.Core.Sync` | `Binho.Core.Contracts` | `UnityEngine`, vendor SDKs, geometry runtime |
| `Binho.Core.Contracts.Tests` | `Binho.Core.Contracts` | production-only vendor/runtime deps |
| `Binho.Core.Sync.Tests` | `Binho.Core.Contracts`, `Binho.Core.Sync` | production-only vendor/runtime deps |

## File-by-file coding checklist

### Phase 1 — assembly shells and folders
Create these first and stop if dependency wiring tries to pull in Unity runtime or networking packages.

- [ ] `Assets/Binho/Core/Contracts/Assembly/Binho.Core.Contracts.asmdef`
- [ ] `Assets/Binho/Core/Sync/Assembly/Binho.Core.Sync.asmdef`
- [ ] `Assets/Binho/Core/Contracts/Tests/Binho.Core.Contracts.Tests.asmdef`
- [ ] `Assets/Binho/Core/Sync/Tests/Binho.Core.Sync.Tests.asmdef`
- [ ] create empty folders matching the namespace map above

Acceptance check:
- all four asmdefs compile with one-directional references only

### Phase 2 — shared identifiers and metadata
These unlock every later DTO.

- [ ] `Shared/Identifiers.cs`
  - `RoomId`, `RoomCode`, `MatchId`, `PlayerId`, `SeatId`
  - `CommandId`, `EventId`, `SnapshotId`, `TimerId`
- [ ] `Shared/Versioning.cs`
  - `RoomVersion`, `StateVersion`, `EventSequence`, `MatchInstance`
- [ ] `Shared/Metadata.cs`
  - `ResolutionState`, `GeometryReadiness`, `EvidenceStatus`, `ScopeKind`, `TimerRecordScope`
- [ ] `Shared/Result.cs`
  - small generic/non-generic result wrapper for sync helper outputs if needed

Recommended first shape choice:
- start with thin immutable wrappers if friction is low
- otherwise use named string/int properties and keep upgrade path obvious

Acceptance check:
- identifiers and versions are referenced by later DTOs instead of ad hoc strings sprinkled across files

### Phase 3 — highest-priority room and match DTO shells
Create the smallest fields necessary for snapshot-level serialization tests.

#### Room
- [ ] `Room/RoomSnapshotDto.cs`
- [ ] `Room/RoomPhase.cs`
- [ ] `Room/SeatStateDto.cs`
- [ ] `Room/SeatPresenceState.cs`
- [ ] `Room/SeatResumeStateDto.cs`
- [ ] `Room/RoomRetentionDto.cs`
- [ ] `Room/PauseContextDto.cs`
- [ ] `Room/RematchStateDto.cs`
- [ ] `Room/TimerRecordDto.cs`

Minimum required fields to stub first:
- `RoomSnapshotDto`
  - `RoomId`, `RoomCode`, `RoomPhase`, `RoomVersion`
  - `ActiveMatchId`, `ActiveMatchInstance`
  - `Seats`, `Retention`, `PauseContext`, `Rematch`, `Timers`
- `SeatStateDto`
  - `Seat`, `PlayerId`, `OccupancyState`, `PresenceState`, `ResumeState`
- `SeatResumeStateDto`
  - `LastAckedRoomVersion`, `LastAckedMatchId`, `LastAckedMatchInstance`, `LastAckedStateVersion`, `LastAckedEventSequence`, `ResumeRequired`, `ResumeToken`
- `TimerRecordDto`
  - `TimerId`, `TimerType`, `Scope`, `Seat`, `Status`, `StartedAtUtc`, `DeadlineUtc`, `ExpiryAction`, `CancelReason`

#### Match
- [ ] `Match/MatchSnapshotDto.cs`
- [ ] `Match/MatchPhase.cs`
- [ ] `Match/ScoreStateDto.cs`
- [ ] `Match/TurnStateDto.cs`
- [ ] `Match/PendingRestartDto.cs`
- [ ] `Match/SemanticEntityStateDto.cs`
- [ ] `Match/EntityAvailabilityState.cs`
- [ ] `Match/MatchTerminalOutcomeDto.cs`

Minimum required fields to stub first:
- `MatchSnapshotDto`
  - `MatchId`, `MatchInstance`, `Phase`, `StateVersion`, `LastCommittedEventSequence`
  - `Score`, `Turn`, `PendingRestart`, `Entities`, `TerminalOutcome`, `GeometryReadiness`
- `TurnStateDto`
  - `ActiveSide`, `ShotIndex`
- `PendingRestartDto`
  - `RestartType`, `BeneficiarySide`, `OriginRef`, `ResolutionState`, `SourceBasis`
- `SemanticEntityStateDto`
  - `EntityId`, `EntityClass`, `Team`, `Availability`, `TopologyRef`, `GeometryRef`, `ResolutionState`

Acceptance check:
- room DTOs can exist without match payload details being populated
- match DTOs can exist without exact coordinates or geometry refs

### Phase 4 — sync envelope DTO shells
- [ ] `Sync/ClientWatermarkDto.cs`
- [ ] `Sync/AcknowledgedWatermarkDto.cs`
- [ ] `Sync/CommandEnvelopeDto.cs`
- [ ] `Sync/DomainEventEnvelopeDto.cs`
- [ ] `Sync/SyncStateDto.cs`
- [ ] `Sync/ReplayWindowDto.cs`
- [ ] `Sync/SnapshotReferenceDto.cs`

Recommended `CommandEnvelopeDto<TPayload>` first fields:
- `CommandId`
- `CommandType`
- `RoomId`
- `MatchId` nullable
- `Seat`
- `ClientWatermark`
- `Payload`
- `SentAtUtc`

Recommended `DomainEventEnvelopeDto<TPayload>` first fields:
- `EventId`
- `EventType`
- `Scope`
- `RoomId`
- `MatchId` nullable
- `MatchInstance` nullable
- `Sequence` nullable for room-only events
- `RoomVersionAtEmit`
- `StateVersionAtEmit` nullable
- `CausationId` nullable
- `Payload`
- `EmittedAtUtc`

Acceptance check:
- room-only event envelope remains legal with null match attribution
- match-scoped envelope cannot pass validation helpers without match attribution

### Phase 5 — persistence record DTO shells
- [ ] `Persistence/RoomSnapshotRecordDto.cs`
- [ ] `Persistence/MatchSnapshotRecordDto.cs`
- [ ] `Persistence/EventLogRecordDto.cs`
- [ ] `Persistence/SeatResumeCheckpointDto.cs`

Exact separation rules to encode:
- room snapshot record owns room lifecycle, seat, retention, rematch, timers summary
- match snapshot record owns one logical match instance only
- event log record must preserve `Scope`, nullable `MatchId`, and nullable `MatchInstance`
- resume checkpoint must be per seat and watermark-specific

Acceptance check:
- no persistence DTO blurs room and match histories into a single unscoped blob

### Phase 6 — sync abstractions
Create interfaces before helpers or stores.

- [ ] `Abstractions/IRoomLifecycleService.cs`
- [ ] `Abstractions/IMatchSyncService.cs`
- [ ] `Abstractions/IResumeService.cs`
- [ ] `Abstractions/ISnapshotProjector.cs`
- [ ] `Abstractions/IRoomSnapshotStore.cs`
- [ ] `Abstractions/IMatchSnapshotStore.cs`
- [ ] `Abstractions/IEventLogStore.cs`
- [ ] `Abstractions/ITimerStore.cs`
- [ ] `Abstractions/IResumeCheckpointStore.cs`
- [ ] `Abstractions/IClock.cs`
- [ ] `Abstractions/IIdGenerator.cs`

Signature rules:
- prefer contract DTOs and named ID types
- keep sync-only and testable; no transport callbacks or SDK types in signatures
- synchronous signatures are acceptable in the first tranche if they keep helpers simple and pure

Acceptance check:
- it is possible to build in-memory tests without inventing transport code

### Phase 7 — minimal concrete helpers and validators
#### Allowed pure helpers
- [ ] `Resume/WatermarkComparer.cs`
- [ ] `Resume/StaleCommandPolicy.cs`
- [ ] `Validation/CommandScopeValidator.cs`
- [ ] `Validation/DomainEventScopeValidator.cs`
- [ ] `Match/MatchSnapshotBuilder.cs`
- [ ] optional: `Resume/SnapshotAckHandler.cs`
- [ ] optional: `Match/MatchEventSequencer.cs`

#### Allowed in-memory adapters
- [ ] `Persistence/InMemory/InMemoryRoomSnapshotStore.cs`
- [ ] `Persistence/InMemory/InMemoryMatchSnapshotStore.cs`
- [ ] `Persistence/InMemory/InMemoryEventLogStore.cs`
- [ ] `Persistence/InMemory/InMemoryTimerStore.cs`
- [ ] `Persistence/InMemory/InMemoryResumeCheckpointStore.cs`
- [ ] deterministic test doubles for `IClock` and `IIdGenerator`

Do not create in this tranche:
- SQLite adapters
- Nakama adapters
- Photon adapters
- Unity `ScriptableObject` loaders
- background workers or retry loops

Acceptance check:
- helpers are enough to support the tests below and no broader fake runtime is created

## Fixture inventory
Use fixture files or inline fixtures in tests, but keep payloads typed and human-readable.

### Recommended fixture filenames
- `Fixtures/room_snapshot_paused_for_disconnect.json`
- `Fixtures/match_snapshot_restart_pending.json`
- `Fixtures/command_resume_session.json`
- `Fixtures/command_acknowledge_snapshot.json`
- `Fixtures/event_room_disconnect_pause_started.json`
- `Fixtures/event_match_restart_awarded.json`
- `Fixtures/timer_disconnect_grace_room_scope.json`
- `Fixtures/rematch_room_continuity_new_match_instance.json`

## Example fixture payloads

### 1. Room snapshot fixture
```json
{
  "roomId": "room_123",
  "roomCode": "ABCD12",
  "roomPhase": "pausedForDisconnect",
  "roomVersion": 42,
  "activeMatchId": "match_123",
  "activeMatchInstance": 1,
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
        "lastAckedRoomVersion": 41,
        "lastAckedMatchId": "match_123",
        "lastAckedMatchInstance": 1,
        "lastAckedStateVersion": 18,
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
        "lastAckedRoomVersion": 42,
        "lastAckedMatchId": "match_123",
        "lastAckedMatchInstance": 1,
        "lastAckedStateVersion": 19,
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
  "timers": [
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
  ]
}
```

### 2. Match snapshot fixture
```json
{
  "matchId": "match_123",
  "matchInstance": 1,
  "phase": "restartPending",
  "stateVersion": 19,
  "lastCommittedEventSequence": 128,
  "score": {
    "north": 2,
    "south": 3
  },
  "turn": {
    "activeSide": "north",
    "shotIndex": 7
  },
  "pendingRestart": {
    "restartType": "penaltyRestart",
    "beneficiarySide": "north",
    "originRef": "north_penalty_region",
    "resolutionState": "provisional",
    "sourceBasis": "confirmed-rule-concept"
  },
  "entities": [
    {
      "entityId": "north_piece_01",
      "entityClass": "player",
      "team": "north",
      "availability": "active",
      "topologyRef": "north_defensive_group_a",
      "geometryRef": null,
      "resolutionState": "unresolved"
    },
    {
      "entityId": "ball_01",
      "entityClass": "ball",
      "team": null,
      "availability": "active",
      "topologyRef": "center_restart",
      "geometryRef": null,
      "resolutionState": "provisional"
    }
  ],
  "terminalOutcome": null,
  "geometryReadiness": "topology-ready"
}
```

### 3. Command envelope fixture
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
  "payload": {
    "resumeToken": null
  },
  "sentAtUtc": "2026-03-14T03:36:03Z"
}
```

### 4. Domain event envelope fixture — room scope
```json
{
  "eventId": "evt_room_001",
  "eventType": "disconnectPauseStarted",
  "scope": "room",
  "roomId": "room_123",
  "matchId": null,
  "matchInstance": null,
  "sequence": null,
  "roomVersionAtEmit": 42,
  "stateVersionAtEmit": null,
  "causationId": null,
  "payload": {
    "requiredSeat": "north",
    "reason": "seatDisconnected",
    "timerId": "timer_disconnect_north"
  },
  "emittedAtUtc": "2026-03-14T03:34:00Z"
}
```

### 5. Domain event envelope fixture — match scope
```json
{
  "eventId": "evt_match_128",
  "eventType": "restartAwarded",
  "scope": "match",
  "roomId": "room_123",
  "matchId": "match_123",
  "matchInstance": 1,
  "sequence": 128,
  "roomVersionAtEmit": 42,
  "stateVersionAtEmit": 19,
  "causationId": "cmd_000057",
  "payload": {
    "restartType": "penaltyRestart",
    "beneficiarySide": "north",
    "originRef": "north_penalty_region",
    "resolutionState": "provisional"
  },
  "emittedAtUtc": "2026-03-14T03:24:00Z"
}
```

### 6. Timer record fixture
```json
{
  "timerId": "timer_disconnect_north",
  "roomId": "room_123",
  "matchId": null,
  "matchInstance": null,
  "scope": "room",
  "timerType": "disconnectGrace",
  "seat": "north",
  "status": "running",
  "startedAtUtc": "2026-03-14T03:34:00Z",
  "deadlineUtc": "2026-03-14T03:39:00Z",
  "expiryAction": "commitAbandonmentForfeit",
  "cancelReason": null,
  "resolvedByEventId": null
}
```

### 7. Rematch transition fixture
```json
{
  "roomId": "room_123",
  "roomVersion": 55,
  "roomPhase": "startingMatch",
  "activeMatchId": "match_124",
  "activeMatchInstance": 2,
  "retainedSeats": ["north", "south"],
  "priorMatch": {
    "matchId": "match_123",
    "matchInstance": 1,
    "terminalReason": "normalWin",
    "finalScore": {
      "north": 7,
      "south": 5
    }
  },
  "newMatch": {
    "matchId": "match_124",
    "matchInstance": 2,
    "stateVersion": 1,
    "lastCommittedEventSequence": 1,
    "phase": "activeTurnPendingInput"
  }
}
```

## Day-one DTO shell spec (field-by-field)

The goal here is not to fully describe the long-term contract surface.
It is to pin down the exact minimum shell each DTO should expose in the first coding tranche so implementation does not reopen semantics, overreach into geometry, or accidentally couple room and match scope.

### Shared value types — day one

| type | day-one fields / cases | defer for later | why day one is enough |
| --- | --- | --- | --- |
| `RoomId`, `RoomCode`, `MatchId`, `PlayerId`, `SeatId`, `CommandId`, `EventId`, `SnapshotId`, `TimerId` | single wrapped string value or equivalent named property | parsing/format policy, format validation regex, custom converters | lets contracts avoid magic strings without vendor/runtime coupling |
| `RoomVersion`, `StateVersion`, `EventSequence`, `MatchInstance` | single wrapped integer value or equivalent named property | overflow policy, arithmetic helpers beyond compare/increment | enough for serialization and watermark ordering |
| `ResolutionState` | `unresolved`, `provisional`, `final` | richer provenance sub-states | already matches unresolved-capable architecture |
| `GeometryReadiness` | `topologyReady`, `zoneReady`, `coordinateReady` | finer-grained authoring/debug stages | enough to preserve pre-geometry gate |
| `EvidenceStatus` | `confirmed`, `inferred`, `designedForDigital` | multi-source confidence scoring objects | preserves semantic-authority distinctions now |
| `ScopeKind` | `room`, `match` | bridge/custom scope variants unless clearly needed later | enough for first-pass log and validator rules |

### Room DTOs — day one vs defer

#### `RoomSnapshotDto`
| field | day one | defer | notes |
| --- | --- | --- | --- |
| `RoomId` | yes | no | canonical room identity |
| `RoomCode` | yes | no | needed for room continuity tests |
| `RoomPhase` | yes | no | lifecycle authority lives at room scope |
| `RoomVersion` | yes | no | room watermark separate from match state |
| `ActiveMatchId` | yes nullable | no | nullable for pre-match rooms |
| `ActiveMatchInstance` | yes nullable | no | nullable for pre-match rooms |
| `Seats` | yes | no | minimum seat occupancy/presence/resume map |
| `Retention` | yes nullable | no | allows post-match retention without transport logic |
| `PauseContext` | yes nullable | no | needed for disconnect-grace snapshots |
| `Rematch` | yes nullable | no | rematch is room-scoped |
| `Timers` | yes | no | supports room-owned timer visibility |
| `ServerTimeUtc` | no | yes | useful later, not required for first shell tests |
| embedded `MatchSnapshotDto` | no | yes | keep room shell light and scope-separated in first tranche |

#### `SeatStateDto`
| field | day one | defer | notes |
| --- | --- | --- | --- |
| `Seat` | yes | no | seat identity is required |
| `PlayerId` | yes nullable | no | nullable keeps empty-seat shape possible |
| `OccupancyState` | yes | no | needed for room lifecycle |
| `PresenceState` | yes | no | needed for reconnect/pause logic |
| `ResumeState` | yes nullable | no | needed for watermark gating |
| display/profile metadata | no | yes | presentation concern, not contract minimum |

#### `SeatResumeStateDto`
| field | day one | defer | notes |
| --- | --- | --- | --- |
| `LastAckedRoomVersion` | yes nullable | no | supports stale room-state gating |
| `LastAckedMatchId` | yes nullable | no | needed across rematch boundaries |
| `LastAckedMatchInstance` | yes nullable | no | needed across rematch boundaries |
| `LastAckedStateVersion` | yes nullable | no | gameplay watermark |
| `LastAckedEventSequence` | yes nullable | no | replay/admissibility watermark |
| `LastAckedSnapshotId` | yes nullable | yes if omitted initially | recommended now because tests already care about snapshot identity |
| `ResumeRequired` | yes | no | core gating flag |
| `ResumeToken` | yes nullable | no | opaque entitlement only |
| token expiry / issuer metadata | no | yes | adapter/service concern later |

#### `RoomRetentionDto`
| field | day one | defer | notes |
| --- | --- | --- | --- |
| `Status` | yes | no | retained / expired / notRetained style enum |
| `ExpiresAtUtc` | yes nullable | no | enough for post-match retention cases |
| retention reason/history | no | yes | audit detail later |

#### `PauseContextDto`
| field | day one | defer | notes |
| --- | --- | --- | --- |
| `Reason` | yes | no | e.g. `seatDisconnected` |
| `RequiredSeat` | yes nullable | no | who must return / act |
| `GraceDeadlineUtc` | yes nullable | no | minimal pause timer view |
| `AbandonmentOutcome` | yes nullable | no | semantic timeout consequence |
| transport diagnostics | no | yes | not part of stable contract |

#### `RematchStateDto`
| field | day one | defer | notes |
| --- | --- | --- | --- |
| `Status` | yes | no | notAvailable / pending / accepted style enum |
| `RequestedBy` | yes | no | enough to model bilateral consent |
| `DeclinedBy` | no | yes | explicit decline can come later if needed |
| deadlines / retention linkage | no | yes | can stay in timer records first |

#### `TimerRecordDto`
| field | day one | defer | notes |
| --- | --- | --- | --- |
| `TimerId` | yes | no | durable timer identity |
| `RoomId` | yes on persistence record, optional on room snapshot child item | yes on nested shell if omitted initially | record identity matters more than nested duplication |
| `MatchId` | yes nullable | no | needed for scope separation |
| `MatchInstance` | yes nullable | no | needed for rematch boundaries |
| `Scope` | yes | no | room vs match |
| `TimerType` | yes | no | disconnectGrace / retention etc. |
| `Seat` | yes nullable | no | timer may be seat-specific |
| `Status` | yes | no | running / cancelled / expired |
| `StartedAtUtc` | yes | no | deterministic record state |
| `DeadlineUtc` | yes | no | authoritative timing |
| `ExpiryAction` | yes nullable | no | semantic timer outcome |
| `CancelReason` | yes nullable | no | explains cancellation |
| `ResolvedByEventId` | no on shell, yes later on persistence record | yes | not required for first DTO serialization tranche |

### Match DTOs — day one vs defer

#### `MatchSnapshotDto`
| field | day one | defer | notes |
| --- | --- | --- | --- |
| `MatchId` | yes | no | match identity required |
| `MatchInstance` | yes | no | rematch boundary required |
| `Phase` | yes | no | core semantic authority |
| `StateVersion` | yes | no | primary gameplay watermark |
| `LastCommittedEventSequence` | yes | no | authoritative event ordering |
| `Score` | yes | no | minimum competitive state |
| `Turn` | yes | no | active side / shot window state |
| `PendingRestart` | yes nullable | no | restart obligations are central pre-geometry |
| `Entities` | yes | no | semantic entity availability/topology refs |
| `TerminalOutcome` | yes nullable | no | needed for match end and rematch lineage tests |
| `GeometryReadiness` | yes | no | preserves pre-geometry contract boundary |
| state hash | no | yes | useful later, not required for current tests |
| participants metadata | no | yes | room owns seats first |

#### `ScoreStateDto`
| field | day one | defer | notes |
| --- | --- | --- | --- |
| `North` | yes | no | minimal explicit score shape |
| `South` | yes | no | minimal explicit score shape |
| win target / overtime metadata | no | yes | rules/presentation detail later |

#### `TurnStateDto`
| field | day one | defer | notes |
| --- | --- | --- | --- |
| `ActiveSide` | yes | no | command admissibility depends on it |
| `ShotIndex` | yes | no | authoritative turn progression watermark |
| `CommandWindowOpen` | no | yes | helper/service can infer from phase for now |

#### `PendingRestartDto`
| field | day one | defer | notes |
| --- | --- | --- | --- |
| `RestartType` | yes | no | stable semantic restart category |
| `BeneficiarySide` | yes | no | who gets the restart |
| `OriginRef` | yes | no | semantic ref only, never coordinates |
| `ResolutionState` | yes | no | unresolved/provisional/final |
| `SourceBasis` | yes | no | preserves confirmed vs designed-for-digital distinction |
| exact coordinate payload | no | yes | explicitly blocked |

#### `SemanticEntityStateDto`
| field | day one | defer | notes |
| --- | --- | --- | --- |
| `EntityId` | yes | no | entity identity |
| `EntityClass` | yes | no | player / ball / barrier semantic kind |
| `Team` | yes nullable | no | balls may be null |
| `Availability` | yes | no | active / suspended / removed style state |
| `TopologyRef` | yes nullable | no | semantic position only |
| `GeometryRef` | yes nullable | no | legal null until coordinate-ready |
| `ResolutionState` | yes | no | unresolved-capable refs |
| exact coordinates / transforms | no | yes | blocked by geometry gate |
| collision/runtime state | no | yes | Unity/simulation concern later |

#### `MatchTerminalOutcomeDto`
| field | day one | defer | notes |
| --- | --- | --- | --- |
| `TerminalReason` | yes | no | normalWin / timeoutForfeit / concession |
| `WinningSeat` | yes nullable | no | enough for result semantics |
| `LosingSeat` | yes nullable | no | enough for result semantics |
| `FinalScore` | yes nullable | no | convenience for persistence/readback |
| `TriggerTimerId` | yes nullable | no | needed for timeout lineage |
| richer audit trail | no | yes | event log covers it later |

### Sync DTOs — day one vs defer

#### `ClientWatermarkDto`
| field | day one | defer | notes |
| --- | --- | --- | --- |
| `LastKnownStateVersion` | yes nullable | no | stale command input |
| `LastKnownEventSequence` | yes nullable | no | stale command input |
| `LastSnapshotId` | yes nullable | no | idempotent resume/ack comparisons |
| state hash | no | yes | later if hashing is introduced |

#### `AcknowledgedWatermarkDto`
| field | day one | defer | notes |
| --- | --- | --- | --- |
| `SnapshotId` | yes | no | snapshot identity |
| `RoomVersion` | yes nullable | no | room-layer ack support |
| `StateVersion` | yes nullable | no | match-layer ack support |
| `EventSequence` | yes nullable | no | event ordering ack |
| `AcknowledgedAtUtc` | no | yes | nice audit field, not required for first logic |

#### `CommandEnvelopeDto<TPayload>`
| field | day one | defer | notes |
| --- | --- | --- | --- |
| `CommandId` | yes | no | command identity |
| `CommandType` | yes | no | semantic intent |
| `RoomId` | yes | no | always required |
| `MatchId` | yes nullable | no | room-only commands remain legal |
| `Seat` | yes | no | actor attribution |
| `ClientWatermark` | yes nullable | no | stale command gate input |
| `Payload` | yes | no | typed payload only |
| `SentAtUtc` | yes | no | deterministic envelope metadata |
| auth/session transport metadata | no | yes | belongs in adapters later |

#### `DomainEventEnvelopeDto<TPayload>`
| field | day one | defer | notes |
| --- | --- | --- | --- |
| `EventId` | yes | no | event identity |
| `EventType` | yes | no | semantic event name |
| `Scope` | yes | no | room vs match separation |
| `RoomId` | yes | no | every event belongs to a room |
| `MatchId` | yes nullable | no | nullable for room-only events |
| `MatchInstance` | yes nullable | no | nullable for room-only events |
| `Sequence` | yes nullable | no | nullable for room-only events in first tranche |
| `RoomVersionAtEmit` | yes | no | room snapshot progression |
| `StateVersionAtEmit` | yes nullable | no | match watermark when applicable |
| `CausationId` | yes nullable | no | command linkage |
| `Payload` | yes | no | typed payload only |
| `EmittedAtUtc` | yes | no | deterministic ordering metadata |
| publisher/vendor metadata | no | yes | adapter concern later |

#### `SyncStateDto`, `ReplayWindowDto`, `SnapshotReferenceDto`
Use only the smallest fields needed to support tests and helper signatures.

| dto | day-one fields | defer |
| --- | --- | --- |
| `SyncStateDto` | `SnapshotId`, `RoomVersion`, `StateVersion`, `LastCommittedEventSequence`, `ResumeRequiresAck` | replay policy details, hashes, diff metadata |
| `ReplayWindowDto` | `AvailableFromSequence`, `AvailableToSequence` nullable, `SnapshotId` nullable | page sizing, retention source metadata |
| `SnapshotReferenceDto` | `SnapshotId`, `RoomId`, `MatchId` nullable, `MatchInstance` nullable, `StateVersion` nullable, `RoomVersion` nullable | storage locator/provider details |

### Persistence DTOs — day one vs defer

| dto | day-one fields | defer | notes |
| --- | --- | --- | --- |
| `RoomSnapshotRecordDto` | `RoomId`, `RoomVersion`, `RoomPhase`, `ActiveMatchId`, `ActiveMatchInstance`, `Snapshot`, `LastUpdatedAtUtc` | storage partition keys/provider metadata | explicit room record boundary |
| `MatchSnapshotRecordDto` | `RoomId`, `MatchId`, `MatchInstance`, `StateVersion`, `LastCommittedEventSequence`, `Snapshot`, `CapturedAtUtc` | hashes/provider metadata | explicit match-instance boundary |
| `EventLogRecordDto` | `RoomId`, `MatchId` nullable, `MatchInstance` nullable, `EventId`, `Sequence` nullable, `RoomVersionAtEmit`, `StateVersionAtEmit` nullable, `Scope`, `EventType`, `CausationId` nullable, `PayloadJson` or typed payload holder, `EmittedAtUtc` | partition/stream metadata | enough to test scope separation |
| `SeatResumeCheckpointDto` | `RoomId`, `Seat`, `PlayerId`, `LastAckedRoomVersion`, `LastAckedMatchId`, `LastAckedMatchInstance`, `LastAckedStateVersion`, `LastAckedEventSequence`, `LastAckedSnapshotId`, `ResumeRequired`, `UpdatedAtUtc` | token issue history | stale-command and rematch-safe resume state |
| `TimerRecordScope` | `room`, `match`, optional `roomMatchBridge` only if clearly needed | custom scope variants | keep enum intentionally small |

## Deferred fields / behaviors table

These are the most likely places a later coding block might accidentally overreach.
Keep them out of the first tranche unless a test becomes impossible without them.

| area | defer now | reason |
| --- | --- | --- |
| geometry | exact coordinates, anchor slots, penalty spots, zone polygons, board dimensions | blocked by evidence gate |
| sync hardening | transport/session metadata, auth claims, connection IDs, socket presence IDs | vendor/adapter concerns |
| hashing | canonical state hash generation, cryptographic compare policy | not required for current serialization and watermark tranche |
| UI/presentation | display names, avatars, countdown formatting, human-readable labels | not core contract authority |
| analytics/audit | verbose provenance trails, source tracing per event, debug diagnostics blobs | useful later, not minimal shell work |
| replay richness | diff payloads, pagination policies, archive retention metadata | current tranche only needs watermark and sequence boundaries |
| runtime integration | Unity object refs, ScriptableObject pointers, scene placement refs, physics state | violates package boundary |
| backend wiring | Nakama/Photon/UGS adapter types, RPC request DTOs, DB provider options | must stay out of `Core.Contracts` |
| advanced validation | exact foul legality against geometry, collision-derived adjudication, final penalty placement legality | blocked by pre-geometry rule |

## Minimal validator / helper pseudocode plan

The first coding tranche should only implement pure helper rules needed to support serialization, stale-command gating, rematch lineage boundaries, and scope validation.
Do not grow these into a full sync runtime yet.

## Final implementation-readiness ambiguity sweep

These are the last contract-level decisions meant to prevent the first coding block from improvising semantics.

### Naming collision / ownership decisions
- Use `RoomPhase` as the room lifecycle enum/type name. Do not keep both `RoomLifecycleState` and `RoomPhase`; they describe the same concept and would create unnecessary drift.
- Keep `MatchPhase` as the match lifecycle enum/type name.
- Keep `ScopeKind` for command/event envelope scope only.
- Keep `TimerRecordScope` for timer records only. It may later add `roomMatchBridge`, so it should not be aliased to `ScopeKind` even when both currently include `room` / `match`.
- Keep `RoomSnapshotDto.ActiveMatchId` / `ActiveMatchInstance` naming as the canonical room pointer. Do not introduce competing names like `CurrentMatchId` in the first tranche.
- Keep `SeatResumeStateDto.LastAcked*` as the seat-facing in-snapshot shape, and `SeatResumeCheckpointDto.LastAcked*` as the persistence mirror. Do not create a third synonym family like `LatestAcked*`.

### Shell-vs-defer boundary corrections
- `acknowledgeSnapshot` is a room/resume command, not a gameplay command. It may include match watermark fields, but it must remain legal with `MatchId = null` for lobby/pre-match resume.
- `startMatch` is a room command that activates a new match boundary. It must remain legal before the new `MatchId` or `MatchInstance` is known client-side.
- `ServerTimeUtc` stays deferred on the first DTO shell unless a concrete serialization test truly needs it. Do not pull clock/UI concerns into the first contract pass unnecessarily.
- Keep `RoomSnapshotDto` free of embedded `MatchSnapshotDto` in the first tranche; use the active match pointer plus separate match snapshot records instead of a nested all-in-one payload.
- Keep `TimerRecordDto.ResolvedByEventId` deferred on the snapshot shell but allow it later on persistence records if audit tests need it.

### Validator ownership matrix
| concern | first owner | why |
| --- | --- | --- |
| watermark ordering | `Binho.Core.Sync.Resume/WatermarkComparer` | pure resume/idempotence helper |
| stale gameplay command gating | `Binho.Core.Sync.Resume/StaleCommandPolicy` | depends on seat checkpoint + watermark comparison |
| command room-vs-match attribution | `Binho.Core.Sync.Validation/CommandScopeValidator` | separate from resume and match snapshot building |
| event room-vs-match attribution | `Binho.Core.Sync.Validation/DomainEventScopeValidator` | keeps envelope rules centralized and mechanical |
| match snapshot field mutation for tests | `Binho.Core.Sync.Match/MatchSnapshotBuilder` | pure match helper, not a validator |
| seat checkpoint idempotent ack update | optional `Binho.Core.Sync.Resume/SnapshotAckHandler` | resume concern, not store concern |

### File-order corrections
- Create `TimerRecordScope` alongside the other shared metadata types before `TimerRecordDto`; otherwise `TimerRecordDto` depends on a later file and the checklist invites out-of-order creation.
- Create validation helpers in a dedicated `Validation` namespace after envelope DTOs exist and before in-memory stores, because store tests rely on those validators.
- Keep in-memory stores after interfaces and validators. They should consume already-settled DTO/validator semantics, not define them.

### Final ready-for-coding deltas
- Add `TimerRecordScope` to `Shared/Metadata.cs`, not as a later persistence-only file.
- Add `Core.Sync.Validation` namespace/folder for scope validators.
- Treat `acknowledgeSnapshot` and `startMatch` as room/lifecycle commands for first-pass command validation.
- Standardize on `RoomPhase` naming everywhere the first tranche references room lifecycle enums.

### `WatermarkComparer`
Purpose:
- compare acknowledged or client-supplied watermarks without transport/vendor concerns

Minimal pseudocode:

```text
Compare(a, b):
  if both null -> Equal
  if only one null -> non-null is newer

  compare StateVersion first when both present
  if different -> larger StateVersion wins

  compare EventSequence next when both present
  if different -> larger EventSequence wins

  if SnapshotId both present and equal -> Equal
  if SnapshotId both present and different while versions equal -> treat as SameWatermarkDifferentSnapshotRef

  otherwise -> Equal
```

Rules:
- newer `StateVersion` dominates older even if snapshot IDs differ
- same `StateVersion` but newer `EventSequence` counts as newer
- equal versions plus equal snapshot is idempotent/equal
- never infer anything from wall-clock timestamps in this helper

### `StaleCommandPolicy`
Purpose:
- reject commands whose client watermark is older than the seat's acknowledged checkpoint

Minimal pseudocode:

```text
Evaluate(command, checkpoint):
  if checkpoint is null -> Accept (no prior ack baseline)
  if checkpoint.ResumeRequired == true and command.CommandType is gameplay command -> RejectResumeRequired
  if command.ClientWatermark is null and checkpoint has any acked state -> RejectMissingWatermark

  comparison = WatermarkComparer.Compare(command.ClientWatermark, checkpoint.ToAcknowledgedWatermark())

  if comparison is Older -> RejectStale
  if comparison is Equal -> Accept
  if comparison is Newer -> AcceptWithCaution or Accept depending on command type
```

Day-one simplification:
- treat room-management commands like `resumeSession` or `leaveRoom` as exempt from gameplay stale checks
- treat gameplay commands as requiring non-stale acknowledged watermark
- do not add hash validation yet

### `DomainEventEnvelope` scope validator
Purpose:
- enforce room-vs-match attribution without transport/runtime code

Minimal pseudocode:

```text
Validate(eventEnvelope):
  require RoomId
  require Scope

  if Scope == room:
    require MatchId == null
    require MatchInstance == null
    require StateVersionAtEmit == null
    allow Sequence == null

  if Scope == match:
    require MatchId not null
    require MatchInstance not null
    require StateVersionAtEmit not null
    require Sequence not null

  return success/failure list
```

Day-one note:
- keep validation semantic, not reflection-heavy
- use simple result/error codes rather than exception-driven control flow in helpers/tests

### `CommandEnvelope` scope/admissibility validator
Purpose:
- keep room-lifecycle commands legal without `MatchId`, while requiring explicit match attribution for gameplay commands

Minimal pseudocode:

```text
Validate(commandEnvelope):
  require CommandId, CommandType, RoomId, Seat

  if CommandType in { resumeSession, acknowledgeSnapshot, requestRematch, declineRematch, leaveRoom, startMatch }:
    MatchId may be null

  if CommandType in { submitShotIntent, concedeMatch }:
    MatchId required

  return success/failure
```

Important constraint:
- treat `acknowledgeSnapshot` as a resume/lifecycle command, not a gameplay command; it may carry match watermark fields in payload without being match-owned
- treat `startMatch` as a room command that creates or activates the next match boundary, so it must remain legal before a new `MatchId` exists
- no vendor auth/session checks here
- no phase legality checks here beyond what is needed for stale-command tests

### `MatchSnapshotBuilder`
Purpose:
- tiny pure helper for rebuilding a current snapshot from a prior snapshot plus already-authoritative semantic mutations/events in tests

Minimal pseudocode:

```text
Apply(snapshot, mutation):
  clone or copy snapshot
  apply only explicit semantic field updates:
    score
    turn
    pendingRestart
    terminalOutcome
    entities
    stateVersion
    lastCommittedEventSequence
  return updated snapshot
```

Day-one rule:
- this is a test support helper, not a full event projector
- no geometry projection, no rules engine, no transport coupling

### `SnapshotAckHandler` (optional only if tests need it)
Purpose:
- update a seat checkpoint idempotently from an acknowledged snapshot

Minimal pseudocode:

```text
HandleAck(checkpoint, ack):
  if checkpoint exists and ack watermark is older -> ignore or keep ResumeRequired true
  if same snapshot already acked -> return unchanged success
  otherwise update:
    LastAckedRoomVersion
    LastAckedMatchId
    LastAckedMatchInstance
    LastAckedStateVersion
    LastAckedEventSequence
    LastAckedSnapshotId
    ResumeRequired = false
```

## Exact proposed first coding-tranche test names and order

Prefer a narrow deterministic order so failures point to contract shape mistakes before helper logic mistakes.

### 1. Contracts shared/value tests
File:
- `Assets/Binho/Core/Contracts/Tests/SharedValueTypesSerializationTests.cs`

Suggested tests:
1. `Identifiers_RoundTripSerialize_PreserveWrappedValues`
2. `Versioning_RoundTripSerialize_PreserveNumericValues`
3. `MetadataEnums_SerializeWithoutCustomConverters`

### 2. Room contract tests
File:
- `Assets/Binho/Core/Contracts/Tests/RoomSnapshotSerializationTests.cs`

Suggested tests:
1. `RoomSnapshot_RoundTripSerialize_AllowsNullActiveMatchForPreMatchRoom`
2. `RoomSnapshot_RoundTripSerialize_PreservesSeatResumeWatermarks`
3. `RoomSnapshot_RoundTripSerialize_PreservesPauseAndTimerData`

### 3. Match contract tests
File:
- `Assets/Binho/Core/Contracts/Tests/MatchSnapshotSerializationTests.cs`

Suggested tests:
1. `MatchSnapshot_RoundTripSerialize_AllowsNullGeometryRef`
2. `MatchSnapshot_RoundTripSerialize_PreservesPendingRestartSemanticRefs`
3. `MatchSnapshot_RoundTripSerialize_PreservesTerminalOutcomeWhenPresent`

### 4. Sync envelope and persistence contract tests
Files:
- `Assets/Binho/Core/Contracts/Tests/SyncEnvelopeSerializationTests.cs`
- `Assets/Binho/Core/Contracts/Tests/PersistenceRecordSerializationTests.cs`

Suggested tests:
1. `CommandEnvelope_RoundTripSerialize_PreservesNullableMatchIdForRoomOnlyCommand`
2. `DomainEventEnvelope_RoundTripSerialize_PreservesRoomScopeNullMatchAttribution`
3. `DomainEventEnvelope_RoundTripSerialize_PreservesMatchScopeAttribution`
4. `SeatResumeCheckpoint_RoundTripSerialize_PreservesAllWatermarkFields`
5. `TimerRecord_RoundTripSerialize_PreservesScopeAndNullableMatchFields`

### 5. Watermark helper tests
File:
- `Assets/Binho/Core/Sync/Tests/WatermarkComparerTests.cs`

Suggested tests:
1. `Compare_WhenStateVersionAdvances_ReturnsNewer`
2. `Compare_WhenStateVersionEqualAndEventSequenceAdvances_ReturnsNewer`
3. `Compare_WhenWatermarksEqual_ReturnsEqual`
4. `Compare_WhenOnlySnapshotReferenceDiffersAtSameVersions_ReturnsEquivalentWatermark`

### 6. Snapshot ack / stale command tests
Files:
- `Assets/Binho/Core/Sync/Tests/SnapshotAckHandlerTests.cs`
- `Assets/Binho/Core/Sync/Tests/StaleCommandPolicyTests.cs`

Suggested tests:
1. `HandleAck_WhenSameSnapshotAckedTwice_IsIdempotent`
2. `HandleAck_WhenOlderSnapshotAckArrivesAfterNewer_KeepsResumeRequired`
3. `Evaluate_WhenGameplayCommandWatermarkOlderThanCheckpoint_RejectsAsStale`
4. `Evaluate_WhenGameplayCommandMissingWatermarkAndCheckpointExists_Rejects`
5. `Evaluate_WhenResumeSessionCommandMissingWatermark_AllowsResumeFlow`

### 7. Scope validation tests
File:
- `Assets/Binho/Core/Sync/Tests/ScopeValidationTests.cs`

Suggested tests:
1. `DomainEventValidator_WhenRoomScopeAndMatchFieldsNull_Accepts`
2. `DomainEventValidator_WhenMatchScopeMissingMatchId_Rejects`
3. `DomainEventValidator_WhenMatchScopeMissingMatchInstance_Rejects`
4. `DomainEventValidator_WhenMatchScopeMissingStateVersion_Rejects`
5. `CommandValidator_WhenRoomOnlyCommandOmitsMatchId_Accepts`
6. `CommandValidator_WhenGameplayCommandOmitsMatchId_Rejects`

### 8. In-memory persistence separation tests
File:
- `Assets/Binho/Core/Sync/Tests/InMemoryStoreSeparationTests.cs`

Suggested tests:
1. `RoomSnapshotStore_Save_DoesNotOverwriteMatchSnapshot`
2. `MatchSnapshotStore_Save_DoesNotMutateRoomRetentionState`
3. `EventLogStore_AppendRoomScope_PreservesNullMatchColumns`
4. `EventLogStore_AppendMatchScope_RequiresMatchAttribution`
5. `TimerStore_Save_PreservesDistinctRoomAndMatchScopeRecords`

### 9. Rematch/new-match-instance tests
File:
- `Assets/Binho/Core/Sync/Tests/RematchLineageTests.cs`

Suggested tests:
1. `RematchTransition_PreservesRoomIdentityAndSeatOwnership`
2. `RematchTransition_CreatesFreshMatchInstanceBoundary`
3. `RematchTransition_ResetsMatchScopedWatermarks`
4. `RematchTransition_DoesNotReusePriorMatchEventStreamAsActiveHistory`

## Validation checklist

### Serialization validation
- [ ] `RoomSnapshotDto` round-trips with nullable `ActiveMatchId` legal for pre-match rooms
- [ ] `MatchSnapshotDto` round-trips with `geometryRef = null`
- [ ] `CommandEnvelopeDto<TPayload>` preserves nullable `MatchId` where command is room-only
- [ ] `DomainEventEnvelopeDto<TPayload>` preserves `Scope`, nullable `MatchId`, and nullable `MatchInstance`
- [ ] `TimerRecordDto` round-trips without UI formatting or local countdown fields
- [ ] `SeatResumeCheckpointDto` round-trips all watermark fields exactly
- [ ] unresolved semantic references remain legal after serialization

### Room-vs-match scope validation
Exact cases to test:

#### Room-only allowed cases
- [ ] `disconnectPauseStarted` event with `Scope = room`, `MatchId = null`, `MatchInstance = null` is valid
- [ ] `snapshotAcknowledged` room event with updated room version and null match attribution is valid
- [ ] `rematchRequested` room event is valid even before new match exists
- [ ] room timer record with `Scope = room` and null match fields is valid

#### Match-required cases
- [ ] `matchStarted` without `MatchId` fails validation
- [ ] `restartAwarded` without `MatchId` fails validation
- [ ] `goalScored` without `MatchInstance` fails validation
- [ ] `matchEnded` without `StateVersionAtEmit` fails validation

#### Separation and persistence cases
- [ ] saving a room snapshot does not overwrite the active match snapshot record
- [ ] saving a match snapshot does not mutate room retention/rematch state
- [ ] event log append for room scope preserves null match columns
- [ ] event log append for match scope requires non-null match columns
- [ ] timer store preserves both room-scope and match-scope timer records without key collision

### Watermark and stale-command validation
- [ ] newer `StateVersion` wins when event sequence also advances
- [ ] newer `EventSequence` with same `StateVersion` is treated as newer
- [ ] duplicate snapshot acknowledgment is idempotent
- [ ] acknowledgment for an older snapshot after a newer one exists leaves seat in `resumeRequired`
- [ ] stale command with lower acknowledged watermark is rejected

### Rematch / new-match-instance validation
- [ ] room ID and room code persist across rematch
- [ ] seat identities persist across rematch
- [ ] new match uses new `MatchId` or strictly incremented `MatchInstance`
- [ ] new match resets match-scoped watermark lineage
- [ ] old match terminal outcome remains attached to prior match snapshot only
- [ ] new match reconstruction never reads prior match gameplay events as active stream

## Suggested test execution order
1. asmdef compile sanity
2. shared identifier/version serialization tests
3. room snapshot serialization tests
4. match snapshot serialization tests
5. envelope and persistence record serialization tests
6. watermark comparison tests
7. stale-command tests
8. room-vs-match validation tests
9. rematch/new-match-instance tests

## Out-of-scope for the coding block that follows
- geometry constants
- board coordinate data
- final foul severity tables that depend on own-box polygons
- transport adapter implementation
- backend vendor lock-in
- Unity scene integration

## Exit criteria for the eventual coding block
The first implementation block is done when:
- all listed files exist with compiling minimal shells
- tests cover serialization, watermarks, rematch, and scope separation
- room-vs-match boundaries are enforced in DTOs, validators, and record shapes
- no runtime or vendor SDK leakage enters the assemblies
- no file claims canonical geometry or exact coordinate legality
