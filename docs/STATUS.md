# STATUS

## Current milestone
Phase 0 — research, product definition, workflow setup, first pre-geometry contract scaffold implementation, post-scaffold review, Unity bootstrap execution tranche kickoff, minimum repo-side Unity project shell restoration, editor baseline selection, and Git bootstrap

## Human directive override
Sawyer explicitly approved the next broader tranche to:
- bootstrap Biñho into a real Unity project on this host
- choose/install the minimum host tooling needed for the first validation pass
- use the most conservative LTS-compatible Unity editor/version if a choice is needed
- initialize Git if needed and push coherent progress to `git@github.com:sawyermatzke/flick_soccer.git`
- use repo-local Git identity `Sawyer Matzke <sawyermatzke@github.com>`

This override is now being executed as a real bootstrap/tooling block rather than another doc-only review pass.

## What changed in this block
- restored the minimum repo-side Unity bootstrap shell:
  - added `Packages/manifest.json`
  - added `ProjectSettings/ProjectVersion.txt`
- chose and documented the first validation editor baseline in `docs/UNITY_EDITOR_BASELINE.md`:
  - Unity `2022.3 LTS`
  - bootstrap pin `2022.3.70f1`
  - rationale: current code already uses file-scoped namespaces / C# 10-era syntax, so older LTS lines like 2021.3 are not a conservative-compatible first validation target
- recorded the actual bootstrap/import status in `docs/UNITY_BOOTSTRAP_VALIDATION_LOG.md`
- confirmed host tooling state on this Ubuntu machine:
  - `git` and `ssh` are present
  - no `unityhub`, `unity`, `unity-editor`, `dotnet`, `mono`, or `msbuild` were found in PATH
  - Ubuntu 24.04 has `fuse3` but not `libfuse2t64`, which is the current minimum known system-package blocker for the common Unity Hub AppImage path
- completed the Git bootstrap required by this tranche:
  - added a Unity-appropriate `.gitignore`
  - initialized the repo as Git
  - set repo-local Git identity to `Sawyer Matzke <sawyermatzke@github.com>`
  - added approved remote `git@github.com:sawyermatzke/flick_soccer.git`

## What is now true
- the repo is no longer only an `Assets/` + docs partial checkout; it now has the minimum committed Unity project-shell artifacts needed for a later editor import attempt
- the intended first validation editor family is documented instead of left ambiguous
- the current blocker has been reduced from a vague “needs Unity somewhere” to a concrete host-tooling issue on this Ubuntu machine

## Confirmed facts so far
- public rules page confirms center kickoff, alternating shots, first to 7 goals, full-crossing goal rule, and score restart from center
- public rules confirm spatial concepts including midfield line, own box, goal line, sideline/baseline, and band interaction
- public listings indicate board size around 22 x 13 x 3 inches
- public listings indicate contents include 20 player pieces, 12 barriers, goals, bands, cards, and score pins
- official/public snippets support that the board uses **20 total player pieces / 10 per side**
- official visuals support that goals are centered on short ends and that setup is mirrored with fixed anchors

## Important constraints
- do not assume exact board layout without evidence
- preserve Confirmed vs Inferred vs Designed-for-digital separation
- some physical-board-only rules need digital reinterpretation
- live multiplayer is MVP
- branding must be separable from gameplay

## Current risks
- exact board geometry still unresolved
- official thumbnails and snippets are useful for topology but still too weak for precise coordinates
- row-by-row counts and barrier/player separation remain unreliable from the currently collected official visuals
- any exact anchor map inferred now would risk fabricated geometry
- IP / trade dress risk for public exact-clone positioning
- digital foul severity mapping is still not final yet
- penalty origin and in-box adjudication remain partly blocked on zone/geometry confirmation
- multiplayer stack is narrowed but not fully locked; future prototype evidence could still shift the final choice between Nakama-first and Photon-style Unity-native approaches
- first compile confidence remains provisional until Unity 2022.3 LTS can actually import the project on this host
- the repo still has no committed Unity-generated `.meta` files yet, so the first real editor import is expected to generate them
- host validation is currently blocked by missing Unity editor tooling; on this Ubuntu 24.04 host the minimum known system dependency gap is `libfuse2t64`

## Test/review status
- repo-side bootstrap execution completed for the bounded minimum shell:
  - `Packages/manifest.json` restored with only `com.unity.test-framework`
  - `ProjectSettings/ProjectVersion.txt` restored with the chosen baseline pin
- host/tooling inspection completed:
  - `git` present
  - `ssh` present
  - no Unity/editor/C# build tools in PATH
- actual Unity import/compile/EditMode execution status:
  - Unity was **not** opened yet
  - package resolution was **not** run yet
  - asmdef discovery/compile inside Unity was **not** run yet
  - EditMode tests were **not** run yet
  - blocker classification is currently **bootstrap/tooling**, not tranche-code logic
- execution record lives in:
  - `docs/UNITY_BOOTSTRAP_VALIDATION_LOG.md`

## Next best step
- install the minimum Ubuntu compatibility package needed for the local Unity Hub AppImage path:
  - `sudo apt-get update && sudo apt-get install -y libfuse2t64`
- then install/open a Unity `2022.3 LTS` editor on this host, import the project, and run the first EditMode validation pass for:
  - `Binho.Core.Contracts.Tests`
  - `Binho.Core.Sync.Tests`
- if import succeeds but tests fail, branch into pre-geometry core code correction only
- if import itself fails, keep the issue classified as bootstrap/tooling/editor compatibility rather than gameplay/runtime scope

## Stop condition for the immediate continuation
Halt if continuation would require scenes, runtime gameplay objects, vendor transport/persistence SDK wiring, CI/editor automation, geometry-backed implementation, invented coordinates, or any scope beyond Unity import/test enablement.

## Repo state
Repo now contains the first pre-geometry `Core.Contracts` / `Core.Sync` scaffold under `Assets/Binho/Core`, the minimum Unity shell under `Packages/` and `ProjectSettings/`, a documented Unity editor baseline in `docs/UNITY_EDITOR_BASELINE.md`, an execution log in `docs/UNITY_BOOTSTRAP_VALIDATION_LOG.md`, and a Unity-oriented `.gitignore`. The repository is now Git-initialized with repo-local identity configured and approved remote `origin` set to `git@github.com:sawyermatzke/flick_soccer.git`. Host-side Unity import/test execution is still pending because the editor/tooling is not yet installed on this machine.
