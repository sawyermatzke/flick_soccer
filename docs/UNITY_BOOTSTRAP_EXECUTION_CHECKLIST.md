# Unity Bootstrap Execution Checklist

Status: review planning draft v1

Purpose:
- translate `docs/UNITY_BOOTSTRAP_TRANCHE_BOUNDARY.md` into the narrowest future **execution checklist** for the first Unity-capable bootstrap/import run
- define the exact **validation-log shape** that run must fill out
- keep the eventual tranche bounded to import/test enablement only

Role in the three-doc handoff set:
- this document answers **the exact step order, stop points, and log format when the later Unity-capable run actually executes**
- `docs/UNITY_VALIDATION_HANDOFF.md` answers **the prerequisite/blocker context behind the run**
- `docs/UNITY_BOOTSTRAP_TRANCHE_BOUNDARY.md` answers **the ownership and out-of-scope line that the run must not cross**

Use this document only when a later block is explicitly authorized to:
- restore the minimum Unity bootstrap artifacts
- open/import the repo in Unity
- run the first EditMode validation pass

Do not use this checklist as permission to widen scope into runtime/gameplay/bootstrap-scene work.

## Preconditions for starting the future bootstrap run
Start only if all are true:
- the current target remains the existing pre-geometry `Assets/Binho/Core` scaffold and tests
- the goal is still limited to Unity import/test enablement
- no step in the planned run requires canonical board coordinates
- no step in the planned run requires creating `Binho.Unity.*` runtime assemblies
- no step in the planned run requires transport SDK, persistence SDK, scenes, prefabs, CI, or editor automation additions

If any of those are false, stop and re-scope before starting.

## Execution checklist

### Phase 0 — capture run metadata before touching project bootstrap
- [ ] record date/time started
- [ ] record machine/host used for bootstrap validation
- [ ] record operator/agent/session identifier if relevant
- [ ] record current repo commit or working-tree state
- [ ] record intended Unity editor version or compatible range
- [ ] record where bootstrap source-of-truth came from:
  - recovered canonical project files
  - known-good prior commit
  - minimal fresh Unity baseline
  - other documented source

Stop boundary:
- if editor target or bootstrap provenance cannot be stated clearly, stop and classify as bootstrap-planning incomplete

### Phase 1 — restore only minimum Unity bootstrap artifacts
Allowed artifact scope:
- `Packages/manifest.json`
- minimum package declarations needed for import and EditMode test discovery
- `ProjectSettings/` sufficient for Unity import
- `Packages/packages-lock.json` only if it naturally belongs to the chosen bootstrap source

Checklist:
- [ ] restore/provide `Packages/manifest.json`
- [ ] restore/provide `ProjectSettings/`
- [ ] restore/provide package lock only if naturally available
- [ ] confirm no scenes, prefabs, runtime gameplay objects, CI scripts, or vendor SDK integrations were added as part of bootstrap restoration

Stop boundary:
- if the next useful action would add scenes, gameplay bootstrap objects, `Assets/Scenes`, `Binho.Unity.*`, vendor packages, or CI/editor automation, stop and open a new tranche

### Phase 2 — import the project in Unity
Checklist:
- [ ] open the repo in the targeted Unity editor
- [ ] allow package resolution/import to finish
- [ ] note any immediate editor/package/bootstrap warnings or failures
- [ ] confirm Unity recognizes the project as openable/importable

Classification rule:
- if import fails before asmdef/test discovery, classify as **bootstrap/tooling blocker**

Stop boundary:
- do not begin gameplay scene authoring, board placement, input wiring, or package expansion while resolving import issues in this tranche

### Phase 3 — compile/discover existing assemblies only
Target assemblies:
- `Binho.Core.Contracts`
- `Binho.Core.Sync`
- `Binho.Core.Contracts.Tests`
- `Binho.Core.Sync.Tests`

Checklist:
- [ ] confirm the four current asmdefs are discovered
- [ ] confirm dependency direction still matches the planned tranche
- [ ] record compile errors, if any
- [ ] record whether compile failures are bootstrap/package/editor compatibility issues or existing tranche-code issues

Boundary rules:
- do not add new runtime assemblies here
- do not move room lifecycle authority into runtime scene objects
- do not add Unity runtime, transport SDK, or persistence SDK types to `Core.Contracts`

### Phase 4 — run the first EditMode validation pass only
Target test assemblies:
- `Binho.Core.Contracts.Tests`
- `Binho.Core.Sync.Tests`

Checklist:
- [ ] run EditMode tests for `Binho.Core.Contracts.Tests`
- [ ] run EditMode tests for `Binho.Core.Sync.Tests`
- [ ] record discovery issues, compile failures, failing tests, and pass counts as available
- [ ] classify each failure as one of:
  - bootstrap/tooling
  - editor/package compatibility
  - current tranche code/test issue

Boundary rules:
- do not add new tests outside the existing tranche merely to improve pass rate in this run
- do not widen into standalone non-Unity harness work
- do not proceed into scene/runtime integration after tests finish

### Phase 5 — stop and hand off
The future bootstrap run is complete once the first import/test attempt has been recorded.

Checklist:
- [ ] write/update repo docs with the validation result
- [ ] state whether the tranche ended in:
  - import blocked
  - compile blocked
  - tests executed with failures
  - tests executed successfully
- [ ] state the exact next recommended track:
  - bootstrap correction only
  - pre-geometry core code correction only
  - later Unity integration planning
- [ ] explicitly stop without adding broader project/runtime content

## Validation log template
Copy/fill this structure during the future Unity-capable tranche.

```text
Unity Bootstrap Validation Log

Run metadata
- started_at:
- finished_at:
- timezone:
- host/machine:
- operator/session:
- repo_commit_or_state:
- unity_editor_version:
- bootstrap_source:
- bootstrap_scope_note:

Bootstrap artifacts restored
- Packages/manifest.json: yes/no
- Packages/packages-lock.json: yes/no/not-applicable
- ProjectSettings/: yes/no
- notes:

Out-of-scope guardrail check
- scenes added: yes/no
- prefabs/gameobjects added: yes/no
- Binho.Unity.* assemblies added: yes/no
- transport SDK added: yes/no
- persistence SDK added: yes/no
- CI/editor automation added: yes/no
- geometry-backed implementation attempted: yes/no
- guardrail_notes:

Import result
- import_completed: yes/no
- package_resolution_completed: yes/no
- import_blockers:
- bootstrap_vs_code_classification:

Asmdef discovery/compile result
- contracts_asmdef_discovered: yes/no
- sync_asmdef_discovered: yes/no
- contracts_tests_discovered: yes/no
- sync_tests_discovered: yes/no
- compile_status:
- compile_errors:
- compile_issue_classification:

EditMode test result
- contracts_tests_ran: yes/no
- sync_tests_ran: yes/no
- contracts_test_summary:
- sync_test_summary:
- failing_tests:
- failure_classification:

Conclusion
- tranche_outcome:
- recommended_next_track:
- stop_reason:
```

## Failure classification rubric
Use these labels only.

### `bootstrap/tooling`
Use when failure is caused by missing/invalid project shell or missing host capability.
Examples:
- missing `Packages/manifest.json`
- missing `ProjectSettings/`
- Unity cannot open/import the project at all
- package resolution cannot begin because bootstrap is incomplete

### `editor/package compatibility`
Use when the project shell exists, but the chosen editor/package baseline appears incompatible.
Examples:
- package version conflicts
- Unity version mismatch
- test framework/package incompatibility preventing discovery/compile

### `tranche code/test issue`
Use when bootstrap/import succeeds and the remaining problem is in the existing `Core.Contracts` / `Core.Sync` code or tests.
Examples:
- compile error in existing DTO/helper/test code
- failing watermark/rematch/scope-separation tests
- asmdef reference drift inside the existing tranche

## Explicit non-goals for the future bootstrap run
Do not do any of the following under this checklist:
- create `Assets/Scenes/`
- create cameras, prefabs, board objects, input systems, or placeholder layouts
- define canonical coordinates, goal positions, penalty spots, or exact geometry
- add Nakama, Photon, UGS, SQLite, Firebase, or other vendor SDKs
- add standalone non-Unity test harnesses
- add CI/editor command-line automation
- introduce Unity runtime types, transport SDK types, or persistence SDK types into `Core.Contracts`

## Ready-to-exit rule
If the future run has:
- restored the minimum bootstrap
- attempted Unity import
- attempted asmdef discovery/compile
- attempted the first EditMode validation pass or clearly documented why that could not proceed
- recorded the result in the validation-log shape above

then the tranche is done and should stop.
