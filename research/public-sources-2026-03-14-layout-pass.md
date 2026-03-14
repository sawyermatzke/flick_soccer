# Public Sources — Layout Research Pass — 2026-03-14

Goal of this pass:
- gather evidence for board layout and active setup
- avoid inventing exact geometry
- separate confirmed facts from inferences and unknowns
- see whether a first-pass coordinate/topology appendix is justified

## Sources checked

### Official site and rules
- https://binhoboard.com/
  - official home page links assembly/setup and gameplay material
- https://binhoboard.com/pages/rules
- https://binhoboard.eu/pages/rules

### Official/public product snippets surfaced by search
- official product/listing snippets repeatedly surfaced:
  - board size around **22 x 13 x 3 inches**
  - standard contents include **20 player pieces**, **12 barrier pieces**, **6 exterior elastic bands**, **2 goals**, **2 balls**, score pins, and cards
- official accessory/blog snippets surfaced wording that each Binho Classic board features **20 total players (10 per side)**

### Official video-linked imagery collected locally
Saved under `research/assets/`:
- `4GBLbru8nc8.jpg` — official assembly video thumbnail
- `uvZLCZwSs3Q.jpg` — official full-game thumbnail
- `VuwbMhksj0g.jpg` — rules/gameplay-related official thumbnail asset

## Confirmed facts extracted this pass
- Board/package dimensions publicly listed around **22 x 13 x 3 inches**.
- Standard set includes **20 player pieces**, **12 barrier pieces**, **6 exterior elastic bands**, **2 goals**, **2 balls**, score pins, and cards.
- Official/public snippets indicate the board uses **10 player pieces per side**.
- Rules confirm these spatial concepts exist:
  - center field
  - midfield line
  - own box
  - goal line
  - sideline and baseline
  - bands / exterior elastic bands
  - penalty shot from anywhere behind midfield line after a red

## Official-image-visible facts
These observations come from official thumbnails only, so they are useful for topology but not for exact measurement.

- Goals are mounted on the short edges and appear centered.
- The printed field clearly includes a midfield line and center circle.
- The setup appears mirrored across midfield.
- Player pieces appear mounted to fixed anchor positions rather than free placement.
- The board appears to use a regular anchor grid / repeated lanes rather than arbitrary spacing.
- In the clearest official board thumbnail views, each side appears to cluster pieces into **multiple depth bands** between goal and midfield rather than one straight line.
- The official views support a **near-goal defensive group**, a **mid-depth group**, and an **advanced group** per side, but current imagery is still too perspective-distorted and partially obscured to confirm counts per row.

## Stronger inferences after this pass
Not promoted to confirmed because the available evidence is still thumbnail-grade and not a clean overhead setup diagram.

- The 20 player pieces are very likely the active standard lineup, not extras.
- Standard setup likely uses a mirrored fixed formation per side.
- The 12 barrier pieces are likely distinct from the 20 active player pieces, because official listing language separates the two categories.
- The field likely uses a small number of discrete depth bands / rows for anchor placement.
- A cautious topological reconstruction is now justified, but a canonical coordinate reconstruction is still **not** justified.

## First-pass reconstruction notes (non-canonical)
This section is strictly for research planning.

### What can be said with medium confidence
- There are **10 active player anchors per side**.
- Those anchors are arranged in a **mirrored formation**.
- The formation likely occupies **roughly three to four longitudinal depth bands** from goal to midfield.
- The overall topology looks more like a structured lane puzzle than a free soccer pitch.

### What remains too uncertain
- exact x/y coordinates of player anchors
- exact row counts per depth band
- exact coordinates and roles of the 12 barrier pieces
- exact dimensions of the own box / penalty-related printed areas
- exact playable bounds relative to the outer package dimensions
- whether all current product variants share identical geometry

## Review outcome after inspecting the currently collected official visuals
Result: **no safe first canonical coordinate/anchor reconstruction yet**.

Why this review did not clear the gate:
- the available official images are thumbnail-grade and perspective-distorted
- visible posts cannot be cleanly partitioned into player anchors vs separate barrier pieces with high confidence in every area of the board
- row-by-row counts are still vulnerable to occlusion and angle error
- any exact anchor map derived from the current set would require invented spacing or invented counts

What the current official/public set does support:
- 20 total active player pieces / 10 per side
- mirrored starting topology
- centered short-edge goals
- center line and center circle
- multiple depth bands per side

What it still does not support safely:
- exact coordinates
- exact counts per depth band
- exact barrier positions
- a canonical coordinate appendix for implementation

## Bottom line from this block
Useful research progress was made:
- active side count is much better supported
- topology is better supported
- exact geometry is **still blocked**

Implementation should remain blocked on canonical layout work.
The highest-leverage next move is adjacent planning/spec work that stays geometry-safe: define the board data/evidence schema, isolate which systems can proceed without final coordinates, and prepare the orchestrator path for implementation once geometry eventually clears.
