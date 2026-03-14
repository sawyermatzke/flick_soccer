# BACKLOG

## Now
- Keep canonical geometry blocked until better official evidence exists
- Define a board data/evidence schema that can represent confirmed vs inferred vs designed-for-digital geometry without locking exact coordinates
- Identify which gameplay/data systems can proceed safely before final layout coordinates exist
- Convert current layout findings into explicit implementation gates and unblock criteria
- Produce a pre-geometry work queue ordered by dependency and safety
- Finalize first-pass digital foul/card replacement direction
- Choose initial multiplayer service stack candidates against the new semantic-authority boundary
- Define Unity project/package structure and pre-geometry adapter seams

## Pre-geometry work queue (ordered)
1. Lock board/evidence schema and readiness gates
2. Lock rules taxonomy and digital foul/restart model that only depends on confirmed zones/topology
3. Define abstract board-state model (teams, pieces, barriers, zones, restart points as unresolved-capable data)
4. Define turn/state machine and match event contract
5. Define topology-safe vs coordinate-safe interface boundary / adapter seam
6. Choose multiplayer stack candidates for authoritative turn/state sync
7. Define Unity project/package structure around data-first board loading
8. Only after better evidence: promote geometry from topology-ready to coordinate-ready

## Recommended implementation tranches
1. docs/contracts only: board-state schema, foul/restart event contract, readiness-aware data types
2. pure-logic layer: score, turn order, restart resolution, abstract piece availability state
3. sync layer: authoritative event/state envelope for multiplayer
4. Unity integration shell: loaders/adapters that consume abstract state without scene coordinates
5. geometry adapter fill-in only when canonical layout is evidence-safe

## Next
- If geometry evidence improves enough: draft a measured coordinate appendix with confidence levels
- If geometry still does not clear the gate: keep implementation blocked and switch to orchestrator planning / adjacent safe prep work
- Convert lifecycle/reconnect policy into concrete sync contracts and payload schemas
- Define backend capability checklist for Nakama-first / Photon-fallback decision confirmation into an implementation-readiness checklist
- Specify private-room snapshot/resume payload contract and timeout state data model
- Turn the new sync contract into a room/match command-event matrix plus DTO package outline (`Core.Contracts` / `Core.Sync`) that stays geometry-safe [done in docs]
- Next: convert the DTO/event-boundary plan into a concrete scaffolding checklist and file/folder map for safe `Core.Contracts` / `Core.Sync` package creation without vendor lock-in [done in docs]
- Next: turn the scaffold plan into an implementation tranche/spec for assembly definitions, DTO shells, store/service interfaces, and serialization tests that can be created without geometry or backend-vendor lock-in [done in docs]
- Next: convert the tranche spec into a file-by-file coding checklist plus example fixture payloads/test-case inventory so a later coding block can execute with minimal ambiguity and no reopening of room-vs-match boundaries [done in docs]
- Next: turn the checklist/fixtures pass into a field-by-field DTO shell spec and minimal validator/helper pseudocode plan so the first coding block can create files without reopening payload semantics or scope rules [done in docs]
- Next: run one more implementation-readiness pass to pin exact file creation order, shell-vs-defer boundaries, and any remaining naming/validation ambiguities before the first safe `Core.Contracts` / `Core.Sync` coding block [done in docs]
- Next: execute the first safe `Core.Contracts` / `Core.Sync` coding tranche strictly from the clarified checklist (asmdefs, DTO shells, validators, interfaces, in-memory adapters, and tests only)
- Next: define the narrow later Unity bootstrap execution checklist and validation-log template from the new tranche boundary so future bootstrap work stays import/test-only [done in docs]
- Next: keep the future Unity-capable bootstrap/import tranche constrained to the new execution checklist/log template (`docs/UNITY_BOOTSTRAP_EXECUTION_CHECKLIST.md`) so the first import/test run stops immediately after validation recording [done in docs]
- Finish the active bounded Unity bootstrap tranche:
  - [done] restore minimum `Packages/` + `ProjectSettings/`
  - [next] install/prepare host Unity tooling on Ubuntu
  - [next] import in Unity 2022.3 LTS and run first EditMode validation
  - [stop] do not widen into scenes/runtime/vendor/geometry work
- Build board data model with confidence/source metadata fields
- Build local single-device simulation prototype
- Implement turn/state machine
- Add shot input prototype

## Later
- Live room flow
- authoritative state sync
- reconnect handling
- onboarding/tutorial
- cosmetics/theme-pack system
- App Store shell and submission prep

## Open questions
- Exact active piece placement in standard play
- Exact geometry of barriers/goals/zones
- Whether official rules differ across versions
- Best networking stack for turn-based physics resolution
- Whether official/public visuals can support a safe first coordinate reconstruction

## Blocked
- Final board coordinate spec blocked on better source extraction
- Final digital penalty consequences partially blocked on confirmed piece/formation layout
