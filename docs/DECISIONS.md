# DECISIONS

## 2026-03-13

### Choose Unity as default engine
Reason:
- best balance of visual quality, iteration speed, monetization flexibility, multiplayer practicality, and stability for a commercial multiplayer game.

### Live multiplayer is MVP
Reason:
- explicit product requirement from project sponsor.

### Do not assume board layout
Reason:
- project sponsor explicitly requested evidence-based layout reconstruction.

### Replace physical yellow/red-card behavior with digital equivalents
Reason:
- some official rules depend on physical-board affordances that do not translate cleanly to mobile; digital adaptation should preserve competitive intent instead of literal props.

### Branding must be swappable
Reason:
- project goal is an exact clone with an easy escape hatch to a generic/original branded product if needed.

### Pre-geometry work must target abstract contracts, not canonical coordinates
Reason:
- geometry evidence currently supports topology/zones better than exact placement, so the safest forward progress is data-model/spec work that preserves unresolved geometry instead of inventing layout constants.

### Multiplayer planning should target server-authoritative semantic room state
Reason:
- the project needs reconnectable live multiplayer, and a server-owned event/snapshot model best fits the current pre-geometry architecture by keeping authority above unresolved board coordinates.

### Multiplayer stack shortlist priority: Nakama first, Photon Fusion fallback
Reason:
- Nakama currently appears to fit the semantic-authority/event-log direction best, while Photon Fusion remains the strongest Unity-native fallback if later prototype work shows room/runtime ergonomics outweigh backend-centric purity for MVP.


### Private-room MVP should use a server-owned pause/resume lifecycle
Reason:
- lifecycle analysis shows the match must survive iOS backgrounding, reconnect from durable semantic snapshots, and resolve timeout/abandon/rematch through authoritative room timers rather than host-device continuity.

### Reconnect eligibility should be gated by snapshot acknowledgment plus sequence/version watermark
Reason:
- the sync-contract pass showed that transport reconnection alone is not strong enough for gameplay admissibility; the authoritative room needs an explicit resume request, snapshot acknowledgment, and stale-command rejection boundary.

### Room lifecycle contracts must stay separate from match-semantic contracts
Reason:
- the DTO boundary pass showed that rematch, resume, timers, and seat presence can outlive an individual match instance; keeping room-scoped and match-scoped records/events separate reduces sync debt and makes reconnect/audit behavior implementable without geometry assumptions.

### Rematch must start a fresh match instance with reset match-scoped watermarks
Reason:
- reconnect, persistence, and replay remain far clearer if room continuity is preserved while match-scoped event streams, state versions, and terminal outcomes reset per logical match.

### `Core.Contracts` should stay DTO-only and `Core.Sync` should start with store/service abstractions plus in-memory test adapters
Reason:
- the scaffolding pass showed the safest next implementation slice is assembly/package creation, DTO shells, orchestration interfaces, and serialization-flow tests without leaking Unity runtime objects, backend SDKs, or geometry assumptions into the core boundary.

### The first implementation tranche should create asmdefs in dependency order and stop at DTO/interface/test foundations
Reason:
- the tranche-spec pass showed the highest-leverage safe start is a four-assembly foundation (`Core.Contracts`, `Core.Sync`, and their test assemblies) with DTO shells, store/service interfaces, deterministic in-memory adapters, and a narrow serialization/watermark/rematch/scope-separation test matrix before any vendor wiring or Unity runtime integration.

### The first coding handoff should be driven by a dedicated checklist/fixture document rather than reopening architecture docs
Reason:
- the checklist pass showed the remaining execution ambiguity is now mostly file order, namespace placement, fixture payload shape, and exact validation cases; isolating that in a dedicated handoff doc keeps `TECH_ARCHITECTURE.md` stable while making the first coding block more mechanical and lower-risk.

### The later wider Unity bootstrap tranche must stop at import/test enablement
Reason:
- the bootstrap-boundary review showed the safest broader-project tranche is only the minimum Unity project shell needed to restore `Packages/` + `ProjectSettings/`, identify editor compatibility, import the repo, and run the first EditMode validation pass; scenes, vendor SDKs, CI, runtime assemblies, and geometry-backed work would blur tranche ownership and should remain deferred until after that first validation result exists.

### The future Unity-capable bootstrap run should follow a fixed execution checklist and validation-log shape
Reason:
- once the tranche boundary was defined, the next remaining risk was execution drift during the first real Unity import/test attempt; a fixed checklist/log template keeps that later run mechanically narrow, preserves bootstrap-vs-code failure classification, and prevents the bootstrap tranche from absorbing runtime, vendor, CI, or geometry-backed work.

### Use Unity 2022.3 LTS as the first validation editor baseline
Reason:
- the current `Assets/Binho/Core` scaffold already uses C# 10-era syntax such as file-scoped namespaces, so older LTS lines like 2021.3 are not conservative-compatible for first import/compile validation; `2022.3 LTS` is the narrowest stable family that fits the existing code without jumping to a newer major Unity line than necessary.
