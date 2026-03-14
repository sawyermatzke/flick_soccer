# Unity Bootstrap Tranche Boundary

Status: review planning draft v1

Purpose:
- define the **narrowest later broader-project bootstrap tranche** that can turn this partial checkout into a runnable Unity project
- keep that tranche limited to repo/project bootstrap needed for import and the **first EditMode validation pass**
- prevent future work from quietly expanding into scenes, gameplay runtime wiring, vendor integrations, CI, or geometry-backed implementation

Role in the three-doc handoff set:
- this document answers **what the later Unity-capable bootstrap tranche owns and where it must stop**
- `docs/UNITY_VALIDATION_HANDOFF.md` answers **the prerequisite/bootstrap blocker context the operator should already know**
- `docs/UNITY_BOOTSTRAP_EXECUTION_CHECKLIST.md` answers **the mechanical execution order and validation-log format for that later tranche**

## Why this tranche exists
The current checkout already contains a pre-geometry `Assets/Binho/Core` scaffold and test asmdefs, but it is still only a **partial Unity-shaped repo**.
Without baseline Unity project bootstrap (`Packages/`, `ProjectSettings/`, compatible editor context), the next meaningful validation step cannot happen.

This later tranche exists only to restore the minimum Unity project shell required to:
- open/import the repo in Unity
- resolve package dependencies
- let asmdefs compile in a normal Unity project context
- run the first EditMode validation pass for the existing `Core.Contracts` / `Core.Sync` tests

## Tranche objective
Produce a Unity-openable project shell that is sufficient for the first EditMode validation pass and nothing more.

Success means:
- the repo contains the minimum Unity project bootstrap directories/files
- a compatible Unity Editor can import the project
- the current four asmdefs can be discovered/compiled in Unity
- the current EditMode tests can be run and classified as pass/fail/bootstrap-blocked

Success does **not** mean:
- gameplay scenes exist
- runtime board placement exists
- geometry is authored
- networking/persistence vendors are chosen or wired
- CI automation exists

## Minimum artifacts this tranche must own
These belong inside the later bootstrap tranche.

### 1. Unity project bootstrap restoration
Required repo artifacts:
- `Packages/manifest.json`
- any immediately required Unity package declarations for asmdef/test discovery
- `ProjectSettings/` sufficient for Unity to import the project

Optional-but-reasonable if they naturally come with the bootstrap source of truth:
- `Packages/packages-lock.json`
- editor-version note/documentation if not already captured elsewhere

Constraint:
- add only what is needed for project import/test discovery; do not use bootstrap restoration as a backdoor for gameplay/runtime work.

### 2. Bootstrap provenance and version note
The tranche must record:
- where the restored bootstrap came from (existing canonical project source, recovered commit, fresh minimal Unity baseline, etc.)
- which Unity editor version/range is expected to open it
- whether package versions are provisional or canonical

This can live in docs rather than code.

### 3. First-pass Unity validation execution record
The tranche must own the first validation record for:
- whether Unity imports successfully
- whether package resolution succeeds
- whether the four asmdefs compile
- whether `Binho.Core.Contracts.Tests` and `Binho.Core.Sync.Tests` are discovered and run
- whether any failures are bootstrap/tooling issues versus tranche-code issues

This is a validation/reporting artifact, not a request to widen scope into fixing all downstream failures.

## Minimum decisions this tranche must own
These decisions are required for the bootstrap tranche to stay bounded.

### Required decision A — editor compatibility target
The tranche must identify the Unity editor version or narrow compatible range to use for the first import.

Reason:
- package resolution and asmdef/test behavior are not meaningfully reviewable without a concrete editor target

### Required decision B — package baseline only, not package strategy expansion
The tranche may decide the **minimum package baseline** needed for import and EditMode tests.
It must not expand into broader package strategy work beyond that immediate need.

Allowed examples:
- Unity Test Framework presence if needed
- packages required for normal asmdef/test discovery in this repo

Not allowed examples:
- analytics packages
- monetization packages
- multiplayer SDK packages
- persistence/database SDK packages
- optional editor tooling unrelated to current tests

### Required decision C — validation host classification
The tranche must explicitly classify the environment used for the first validation:
- machine/editor used
- whether it is authoritative for future validation or only a one-off bootstrap host

Reason:
- avoids repeating vague "Unity-capable machine" phrasing after the first actual bootstrap attempt

## Explicit stop boundary for this tranche
Stop once the project can be imported and the first EditMode validation pass has been attempted/documented.

Do **not** continue this tranche into:
- creating `Assets/Scenes/`
- authoring a playable bootstrap scene
- adding cameras, input systems, prefabs, or placeholder board GameObjects
- moving room/match authority into Unity runtime objects
- adding Nakama, Photon, UGS, SQLite, Firebase, or other vendor SDKs
- adding CI/editor automation scripts
- generating a standalone non-Unity test harness unless separately approved/documented as a new tranche
- fixing geometry-dependent gameplay issues
- inventing canonical board coordinates or penalty spots

If useful continuation after import/test would require any of the above, end the tranche and open a new scoped block.

## What belongs immediately after this tranche
If the first EditMode validation pass succeeds or yields only tranche-code issues, the next work should branch into one of two later tracks:

### Track 1 — pre-geometry code correction inside existing core tranche
Use if:
- Unity import works
- asmdefs/tests run
- failures are in the current `Core.Contracts` / `Core.Sync` code/tests

This track may:
- fix DTO/helper/test issues inside the already-defined pre-geometry scope
- keep room lifecycle authority separate from match-semantic authority
- avoid Unity runtime/vendor leakage into `Core.Contracts`

### Track 2 — broader Unity integration planning after first validation
Use only after the first validation pass is complete.

This later track may begin to plan:
- Unity integration shell boundaries
- how placeholder presentation should consume pre-geometry state
- how future scenes/bootstrap objects should stay downstream of core authority

But even that track still remains outside this bootstrap tranche.

## What remains out of scope until after the first EditMode validation pass
These items should remain explicitly deferred until after the first validation pass completes and is documented.

### Outside scope before first validation
- `Packages/` additions beyond minimum import/test needs
- gameplay scenes and scene hierarchy decisions
- `ProjectSettings/` tuning unrelated to import/test viability
- any `Binho.Unity.*` runtime assembly creation
- topology/geometry runtime adapters
- transport adapter selection/implementation
- persistence adapter selection/implementation
- CI/editor command-line automation
- standalone C# test harness decisions
- visual placeholder board layout
- input/physics/collision authoring

## Ownership checklist for the future bootstrap tranche
Use this as the narrow acceptance checklist.

- [ ] restore/provide `Packages/manifest.json`
- [ ] restore/provide `ProjectSettings/`
- [ ] note expected Unity editor version/range
- [ ] import the project in Unity
- [ ] allow package resolution to complete
- [ ] confirm four asmdefs compile/discover cleanly
- [ ] run EditMode tests for the two current test assemblies
- [ ] record failures as bootstrap/tooling vs tranche-code
- [ ] stop without adding scenes, vendor SDKs, CI, or geometry/runtime work

## Tranche handoff sentence
If a future block starts creating project skeleton content beyond what Unity import and first EditMode validation require, that block has crossed the tranche boundary and should stop or be re-scoped.
