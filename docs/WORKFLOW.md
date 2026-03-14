# WORKFLOW

Project-local workflow for continuous autonomous iteration.

## Loop
- Research/Feature Block
- Review Block
- Orchestrator Block
- repeat until stop time or stop condition

Handoffs should be immediate by default: when one block finishes, the next should be scheduled to start as soon as practically possible.

## Canonical context read order at start of every block
1. `docs/RUN_STATE.json`
2. `docs/STATUS.md`
3. `docs/BACKLOG.md`
4. relevant spec docs for the current block
5. `docs/DECISIONS.md`

`docs/RUN_STATE.json` is the machine-readable coordination layer for autonomous scheduling, stall detection, and run-window enforcement.

## Block requirements
Every block must:
- make one coherent slice of progress
- update any affected specs/docs
- update `docs/STATUS.md`
- update `docs/RUN_STATE.json` when block completion, readiness, or scheduling state changes materially
- identify the next block
- schedule the next block unless a stop condition is hit
- if the repo is git-backed, commit coherent slices when they are in a sane state
- if the repo has an approved remote and the slice is coherent, push progress rather than leaving useful work stranded locally

Scheduling the next block should use current system time and immediate chaining by default, with only a minimal buffer unless there is a specific reason to wait.

For autonomous overnight project jobs, default cron delivery should be `none` unless there is an explicit reason to send a user-facing update to a specific channel/recipient.

## Stop conditions
- active run window ended
- not enough time remains for another useful block plus wrap-up
- continued work would require unverifiable assumptions on critical gameplay/layout facts
- safe continuation requires human input
- risks/failures make further autonomous work unwise

## Orchestrator rule
If the obvious next task is complete, choose the highest-leverage adjacent task inside scope instead of idling.

## Scheduling policy
Default policy is immediate chaining, not coarse cadence.

- Each block should schedule the next block for the earliest practical time after it finishes.
- In practice, this means using the current system time at the end of the block and scheduling the next one-shot run with only a minimal buffer (for example 1–3 minutes) unless a deliberate wait is needed.
- Do not wait for the next half hour or hour unless the work explicitly benefits from that delay.
- The orchestrator should start as soon as the prior block completes whenever possible.

## Time handling
- Autonomous agents may read the current system time/status to compute the next start time.
- They should compare current time against `docs/RUN_STATE.json` run-window limits before scheduling.
- They should prefer immediate continuation while the run is active and the project is not blocked.

## Git / remote policy for this project
- Approved remote: `git@github.com:sawyermatzke/flick_soccer.git`
- Use repo-local Git identity: `Sawyer Matzke <sawyermatzke@github.com>`
- If the broader bootstrap tranche is executing and the repo is still not a git repo, initializing git and adding the approved remote are in scope.
- Prefer coherent commits with meaningful messages over noisy micro-commits.
- Push after coherent slices land, especially when the work materially changes the repo state or unblocks validation.

## Initial project-specific priority
Before implementation, finish a disciplined board/rules evidence pass so the engine is built against a trustworthy spec.
