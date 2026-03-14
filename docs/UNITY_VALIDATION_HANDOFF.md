# Unity Validation Handoff — Core.Contracts / Core.Sync

Status: review handoff draft v1

Purpose:
- capture the smallest exact prerequisites for the **first real Unity asmdef/EditMode validation pass** of the current pre-geometry `Core.Contracts` / `Core.Sync` tranche
- avoid re-discovering the same bootstrap blockers on the next Unity-capable machine
- stay strictly inside doc/process handoff scope

Role in the three-doc handoff set:
- this document answers **what must already be true before validation can start**
- `docs/UNITY_BOOTSTRAP_TRANCHE_BOUNDARY.md` answers **how far the later bootstrap tranche is allowed to go**
- `docs/UNITY_BOOTSTRAP_EXECUTION_CHECKLIST.md` answers **the exact step order and logging shape once that tranche actually runs**

## What is already present in this checkout
Confirmed in the current repo:
- `Assets/Binho/Core/Contracts` exists with `Binho.Core.Contracts.asmdef`
- `Assets/Binho/Core/Sync` exists with `Binho.Core.Sync.asmdef`
- `Assets/Binho/Core/Contracts/Tests` exists with `Binho.Core.Contracts.Tests.asmdef`
- `Assets/Binho/Core/Sync/Tests` exists with `Binho.Core.Sync.Tests.asmdef`
- DTO shells, validators, in-memory stores, and EditMode-style test source files are present
- asmdef dependency direction remains correct for the tranche:
  - `Binho.Core.Contracts` -> no project assembly refs
  - `Binho.Core.Sync` -> `Binho.Core.Contracts`
  - test asmdefs -> production asmdefs only

## What is not yet present in this checkout
Confirmed missing from the current repo root after the bounded bootstrap restore:
- generated Unity solution / csproj context
- any Unity-generated `.meta` files
- any Unity-import result proving package resolution / asmdef discovery on this host

Implication:
- the repo now has the minimum committed project-shell artifacts (`Packages/manifest.json` and `ProjectSettings/ProjectVersion.txt`), but it is still **not yet validated as open-and-run on this host**
- the current test asmdefs are Unity Editor test assemblies, so they still need a real Unity import before standard EditMode execution can happen

## Minimum prerequisites before validation can really run

### 1. Repo/bootstrap prerequisites
These are required before a Unity-capable host can do anything meaningful with the current asmdefs/tests.

Required:
- repo checkout containing the existing `Assets/` tree
- committed `Packages/manifest.json` (now restored)
- committed `ProjectSettings/ProjectVersion.txt` / minimum `ProjectSettings/` shell (now restored)

Strongly preferred:
- `Packages/packages-lock.json` if the project intends reproducible package resolution
- a short note stating the intended Unity editor version once known (now documented in `docs/UNITY_EDITOR_BASELINE.md`)

Not required for this first validation pass:
- canonical board geometry
- gameplay scenes
- transport adapters
- persistence vendor adapters
- broader project gameplay implementation beyond the current core assemblies/tests

### 2. Host tooling prerequisites
Required on the validation machine:
- installed Unity Editor compatible with the project bootstrap
- ability to open the project and let Unity import/regenerate project files
- Unity Test Framework support via the restored Unity package set

Helpful but not strictly required if Unity can run tests directly:
- Unity Hub or equivalent launcher
- generated `.sln` / `.csproj` files for IDE inspection after first project open

Not sufficient by itself:
- plain `dotnet` / `csc` / `msbuild` without Unity project bootstrap

Why:
- the current tests are Unity Editor test asmdefs (`optionalUnityReferences: ["TestAssemblies"]`), not a standalone non-Unity xUnit/NUnit harness

## First validation steps on a Unity-capable machine
Run these in order.

1. Confirm the checkout includes:
   - `Assets/`
   - `Packages/`
   - `ProjectSettings/`
2. Open the repo as a Unity project.
3. Allow package resolution and project import to finish.
4. Let Unity regenerate solution/csproj context if the editor/project settings are configured to do so.
5. Confirm the four asmdefs appear without unexpected extra dependencies.
6. Open the Test Runner and run the EditMode tests for:
   - `Binho.Core.Contracts.Tests`
   - `Binho.Core.Sync.Tests`
7. Record:
   - Unity editor version used
   - package-resolution/import issues if any
   - compile errors if any
   - failing tests if any
   - whether failures are bootstrap-related or tranche-logic-related

## Expected first-pass validation outcomes
The first real validation pass should answer only these questions:
- does the repo import as a Unity project now that the minimum `Packages/` and `ProjectSettings/` shell exists?
- do the four asmdefs compile in Unity?
- do the current EditMode tests execute?
- are any failures caused by:
  - missing project bootstrap
  - Unity/package compatibility
  - actual tranche code/test issues

Do **not** expand the validation goal yet into:
- scene/runtime integration
- geometry-backed gameplay
- multiplayer transport wiring
- persistence SDK wiring

## Documentation boundary: what should be documented now vs later
### Document now in repo docs
These are immediate blockers to the current tranche and should be explicit now:
- the repo now has the minimum committed Unity project shell, but still has no proven import result on this host
- Unity Editor remains a prerequisite for normal asmdef/EditMode validation
- plain standalone C# tooling is not an equivalent validation path for the current test assemblies
- the next validation host should perform import + EditMode test execution first, before any broader implementation work

### Leave for the later wider project-skeleton tranche
These are real follow-up tasks, but broader than this handoff block:
- refining the provisional `Packages/manifest.json` if the first real import shows additional minimum package needs
- changing the chosen Unity 2022.3 LTS baseline unless real import evidence forces it
- creating the broader Unity project skeleton beyond what is needed to explain the blocker
- deciding whether the repo should also gain a parallel standalone non-Unity contract test harness
- adding scenes, gameplay bootstrap objects, CI editor automation, or vendor adapters

## Minimal pass/fail gate for the next environment
Validation is ready to begin only when all are true:
- `Assets/`, `Packages/`, and `ProjectSettings/` are present in the checkout
- a Unity Editor compatible with that project bootstrap is available
- Unity import completes without bootstrap-level failure
- the Test Runner can discover the two current EditMode test assemblies

If any of those are false, stop and classify the blocker as **project bootstrap/tooling**, not `Core.Contracts` / `Core.Sync` logic.
