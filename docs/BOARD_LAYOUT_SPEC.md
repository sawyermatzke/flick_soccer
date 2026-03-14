# BOARD_LAYOUT_SPEC

Status: evidence-gathering draft v2

This document is intentionally conservative. It records only what is currently confirmed from public sources and explicitly labels inferences, provisional reconstruction notes, and unknowns.

## Sources used in this revision
- Official site home page: https://binhoboard.com/
  - provides links to official assembly/setup and gameplay materials
- Official rules page: https://binhoboard.com/pages/rules
- Mirrored public rules text: https://binhoboard.eu/pages/rules
- Official/public product and accessory snippets surfaced by public search on binhoboard.com
  - board size around 22 in x 13 in x 3 in
  - standard set includes 20 player pieces, 12 barrier pieces, 6 exterior elastic bands, goals, balls, score pins, and cards
  - accessory/blog snippets state that each Binho Classic board features 20 total players / 10 per side
- Official video-thumbnail imagery captured locally under `research/assets/`
  - `4GBLbru8nc8.jpg` — assembly video thumbnail
  - `uvZLCZwSs3Q.jpg` — full-game thumbnail
  - `VuwbMhksj0g.jpg` — rules/gameplay-related thumbnail asset

---

## 1. Confirmed public facts

### 1.1 Board/package/component facts
- Public product listings describe the board as approximately **22 in x 13 in x 3 in**.
- Public product listings describe standard contents including:
  - 2 balls
  - 2 score pins
  - 2 red/yellow cards
  - 2 goals
  - 20 player pieces
  - 12 barrier pieces
  - 6 exterior elastic bands
- Official/public accessory/product snippets indicate a standard board uses **20 total player pieces, 10 per side**.

### 1.2 Rules-implied spatial facts
These are confirmed because the public official rules explicitly require them.
- There is a **center field / center spot** used for kickoffs and post-goal restarts.
- There is a **midfield line**.
- Each side has an **own box** / defensive box concept, because some fouls are red cards only when committed within a player's own box.
- The game has a **goal line** for each goal, and the full-crossing rule is judged relative to that line.
- A **penalty shot region behind the midfield line** exists as a rules concept, because the rules say a red card gives the opponent a penalty kick from anywhere behind the midfield line.
- The board has **out-of-bounds edges** including sideline and baseline concepts.
- The board uses **exterior bands / elastic bands** as active play boundaries, because the rules explicitly mention balls being against the bands and players moving the band back with the shooting hand.

### 1.3 Official-image-visible facts
The following are visible in official imagery/video thumbnails, but the imagery is not precise enough for exact measurement.
- Goals are mounted on the **short ends** of the board and appear **centered** on those ends.
- The pitch includes a visible **midfield line** and **center circle** marking.
- The setup is **symmetric across the midfield line** in official imagery.
- Player pieces appear as vertical posts/discs anchored into predefined field positions rather than freely placed miniatures.
- Interior placements appear to be based on a **fixed modular hole/anchor pattern** rather than arbitrary placement.
- The standard setup appears to use **multiple depth bands** from goal toward midfield, not one flat line of pieces.

---

## 2. Strong inferences from corroborated public evidence
These are not yet upgraded to Confirmed because the currently collected evidence is visual/snippet-level rather than a clean setup diagram or frame-by-frame measured source.

- The 20 player pieces are very likely the **active standard match roster**, not a box count with extras, because official snippets separately say the board features **20 total players / 10 per side**.
- The standard 1v1 field appears to use a **mirrored fixed formation per side**.
- Public listings treat **player pieces** and **barrier pieces** as separate component categories, so the current safest interpretation is:
  - 20 pieces = active player pieces
  - 12 pieces = additional barriers distinct from players
- The defensive box is likely a **printed rectangular or box-like zone** near each goal rather than a purely abstract rules concept, because rules reference it operationally and official imagery shows additional near-goal pitch markings.
- The board likely supports a lane/channel structure created by combinations of players, barriers, and side bands, rather than open continuous soccer-field spacing.
- The active formation likely uses **roughly three to four longitudinal depth bands per side**, but exact row counts are not yet recoverable with enough confidence.

---

## 3. Unknown / not yet confirmed

Critical unknowns still blocking exact implementation:
- Exact internal playable field dimensions
- Exact goal mouth dimensions
- Exact depth/width of each own box
- Exact location and geometry of any printed penalty area markings
- Exact position of every active player piece in the standard starting formation
- Exact position of every non-player barrier piece
- Exact number of distinct rows/lanes used by starting pieces
- Exact counts per depth band on each side
- Exact distances from side walls/bands to nearest playable anchors
- Whether all current official board versions use identical geometry
- Whether official 2.0 / Classic / OG boards differ in active layout

---

## 4. Evidence policy
- Do not derive canonical coordinates from memory or fan recreations.
- Do not promote thumbnail eyeballing to exact geometry.
- Prefer, in order:
  1. official setup/assembly video frames
  2. official gameplay footage from stable camera angles
  3. official board photos with minimal perspective distortion
  4. repeated corroboration across multiple official/public sources
- If exact coordinates cannot be recovered, keep the main spec topological/provisional and place any reconstruction in a clearly labeled appendix.

---

## 5. First-pass topology and coordinate-reconstruction appendix

This appendix is useful for planning research and future data structures, but it is **not yet safe** to implement as exact coordinates.

### 5.1 Confidence legend
- **High**: directly stated in rules/product copy or plainly visible in official imagery
- **Medium**: repeated implication across multiple official/public sources but not yet numerically measured
- **Low**: plausible reconstruction from official visuals, still unsafe as canonical geometry

### 5.2 Reconstructed board topology
- **Centered short-edge goal on each end** — High
- **Center spot and center circle at midfield** — High
- **One mirrored defensive box near each goal** — High for existence, Low for exact shape/size
- **Ten active player anchors per side** — High for count, Low for exact coordinates
- **Fixed mirrored formation for starting setup** — Medium
- **Additional non-player barrier anchors/pieces separate from players** — Medium
- **Play bounded by side/baseline band system rather than rigid full-height walls alone** — High for gameplay relevance, Low for exact segment geometry
- **Multiple depth bands from goal to midfield on each side** — Medium

### 5.3 First-pass coordinate reconstruction status
A first-pass coordinate reconstruction is only partially justified.

#### Safe to state
- Coordinate system origin should probably be board center.
- Long axis should run goal-to-goal.
- Starting formation appears mirrored about the midfield line.
- Piece positions appear grouped into a small number of depth bands from goal to midfield rather than evenly continuous spacing.

#### Not safe to state yet
- exact coordinates
- exact spacing ratios
- exact counts per band
- exact assignment of which visible posts are active players vs distinct barrier pieces in thumbnail-level imagery

### 5.4 Provisional reconstruction with confidence labels
This is the furthest safe summary at the moment:
- **Near-goal defensive group per side exists** — Medium
- **At least one more advanced group between own box and midfield exists** — Medium
- **Potential third/fourth depth grouping visible in some official thumbnails** — Low
- **Row-by-row anchor count recoverable from currently collected thumbnails alone** — Low / not yet established

### 5.5 Review verdict on currently collected official/public evidence
After reviewing the currently collected official thumbnails/assets, a **safe first canonical coordinate/anchor reconstruction is still not justified**.

Why the gate stays closed:
- the clearest available official views are still perspective-distorted rather than top-down
- some anchors/posts are partially occluded by rails, branding, hands, or thumbnail text treatment
- the currently collected images do not safely separate active player anchors from distinct barrier-piece anchors in every visible row
- row-by-row counts and spacing ratios cannot be recovered with enough confidence to avoid fabricated geometry
- current evidence supports topology and symmetry better than measurement

What is supportable right now:
- mirrored fixed setup topology
- centered short-edge goals
- center line / center circle
- ten active player pieces per side
- multiple depth bands per side

What is still unsafe:
- exact coordinates
- exact row counts per band
- exact barrier coordinates
- exact own-box dimensions
- any canonical coordinate appendix intended for implementation

### 5.6 Implementation gate
Do **not** convert this appendix into gameplay constants until one of these happens:
- official setup footage is frame-extracted and measured well enough to identify anchor pattern, or
- an official/public diagram is found that clearly shows starting layout, or
- multiple independent official images provide consistent recoverable coordinates.

---

## 6. Geometry-safe board data/evidence model
Board layout should eventually be represented as data, not hardcoded scene placement.

### 6.1 Core principle
Until geometry is confirmed, the project should be able to store:
- what is **Confirmed** from public evidence
- what is **Inferred** from repeated official/public signals
- what is **Designed for digital** where the mobile game needs an explicit rule/region before the physical geometry is fully known

The data model should therefore support incomplete geometry without pretending unknown values are settled.

### 6.2 Planned structure
- board bounds
- goal bounds
- wall/band segments
- barrier pieces with coordinates and rotation
- player pieces with team, role, and starting coordinates
- zone polygons / lines
- restart spawn points
- penalty origin points or penalty-origin rule region

### 6.3 Recommended evidence metadata for every geometry fact
Minimum metadata:
- `status` — `confirmed` | `inferred` | `designedForDigital`
- `confidence` — `high` | `medium` | `low`
- `sourceRefs[]`
- `notes`

Recommended per-record structure:

```json
{
  "id": "south_goal",
  "kind": "goal",
  "status": "confirmed",
  "confidence": "high",
  "sourceRefs": ["rules-page", "official-thumbnail-uvZLCZwSs3Q"],
  "geometry": {
    "type": "rect",
    "x": null,
    "y": null,
    "width": null,
    "height": null,
    "rotation": 0
  },
  "topology": {
    "edge": "south-short-end",
    "centeredOnEdge": true,
    "mirroredWith": "north_goal"
  },
  "notes": "Centered short-edge goal is well supported; exact mouth dimensions remain unknown."
}
```

### 6.4 Entity categories to keep separate
Do not collapse these categories in planning or future implementation:
- printed field markings
- active player pieces
- distinct barrier pieces
- wall/band boundaries
- restart/penalty regions

That separation is necessary because current evidence supports their existence at different confidence levels.

### 6.5 Geometry readiness levels
Use explicit readiness gates so implementation can tell whether a system is safe to build.

- **Topology-ready**
  - safe facts: mirrored sides, centered goals on short edges, midfield/center circle existence, ten active players per side, multiple depth bands
  - safe uses: turn/state planning, abstract board graph planning, evidence-aware data structures
- **Zone-ready**
  - requires reasonably supported own-box, goal-line, out-of-bounds, and restart-region definitions
  - safe uses: foul taxonomy, restart taxonomy, scoring semantics
- **Coordinate-ready**
  - requires measured or clearly diagrammed anchor positions and barrier positions
  - safe uses: canonical scene placement, physics constants tied to exact layout, shot-line validation against final geometry

### 6.6 Implementation gates / unblock criteria
Do **not** start canonical board implementation until all coordinate-ready criteria are met:
- player-anchor counts per depth band are recoverable without guesswork
- barrier-piece locations are distinguishable from player locations with high enough confidence
- own-box dimensions/shape are supported well enough for foul and penalty logic
- official/public evidence is strong enough to derive mirrored coordinates without invented spacing
- at least one of the evidence conditions in section 5.6 is satisfied

Work may proceed before that gate only if it depends on topology, zones, or abstract rules rather than exact coordinates.

### 6.7 Safe pre-geometry implementation candidates
These systems appear buildable before canonical coordinates if kept abstract/data-driven:
- board/evidence schema and loaders
- rules taxonomy and foul-state model
- turn/state machine
- restart event model
- score/game-end logic
- multiplayer action/state envelope design
- separation between branded assets and generic gameplay model

These systems should stay blocked until coordinate-ready:
- canonical board scene placement
- final spawn/anchor constants
- shot aiming and collision tuned to final board geometry
- exact barrier placement and rebound behavior authored against a fixed layout

## 6.8 Abstract board-state spec boundary
The board-state contract should separate **match semantics** from **final geometry**.

### Purpose
Allow rules, multiplayer sync, replays, and turn validation to proceed before canonical coordinates are known.

### Safe boundary for pre-geometry state
Pre-geometry state may represent:
- teams and turn ownership
- piece identity and availability
- topology-level location references
- zone membership and edge-state semantics
- restart obligations and pending resolution state
- evidence/readiness metadata for layout-derived references

Pre-geometry state must not require:
- canonical world coordinates
- exact lane widths or anchor spacing
- final barrier collision authoring
- any claim that unresolved placement is measured/final

### Recommended layers
1. **Match layer**
   - score
   - period/phase if needed
   - side-to-move
   - shot sequence / possession turn context
2. **Entity layer**
   - teams
   - player pieces
   - barrier pieces
   - balls
3. **Topology reference layer**
   - named zones
   - named restart origins
   - named edges / bands / goal lines
   - optional anchor-group identifiers that do not imply final coordinates
4. **Geometry layer**
   - coordinates, sizes, rotations, polygons
   - may be partially null until coordinate-ready
5. **Evidence layer**
   - confirmed vs inferred vs designed-for-digital metadata
   - source references and confidence

### Example boundary-friendly shape
```json
{
  "match": {
    "score": { "north": 0, "south": 0 },
    "turn": {
      "activeSide": "north",
      "shotIndex": 1,
      "restartState": null
    }
  },
  "entities": {
    "pieces": [
      {
        "id": "north_piece_01",
        "team": "north",
        "entityClass": "player",
        "availability": "active",
        "topologyRef": "north_defensive_group_a",
        "geometryRef": null,
        "status": "inferred"
      }
    ],
    "balls": [
      {
        "id": "ball_01",
        "state": "atRest",
        "topologyRef": "center_spot",
        "geometryRef": null
      }
    ]
  },
  "topology": {
    "zones": ["north_box", "south_box", "midfield", "field_of_play"],
    "restartOrigins": ["center_restart", "north_penalty_region", "south_penalty_region"]
  },
  "geometryReadiness": "topology-ready"
}
```

### Interface boundary between topology-safe and coordinate-safe systems
Topology-safe systems may consume:
- zone IDs
- side-of-midfield checks
- in/out-of-bounds semantics
- goal-scored events
- restart origin IDs
- piece availability / team membership

Coordinate-safe systems should be the first consumers of:
- precise anchor positions
- collision meshes / rebound tuning
- shot-line obstruction based on final authored geometry
- exact placement serialization for scene rendering

### Unresolved-capable references
Any pre-geometry location reference should support unresolved placeholders such as:
- `topologyRef` — stable named semantic location
- `geometryRef` — optional pointer to later canonical coordinate record
- `resolutionState` — `unresolved` | `provisional` | `final`

This lets implementation begin on loaders/state sync without laundering guesswork into constants.

## 6.9 Pre-geometry implementation ordering
Recommended safe tranche order:
1. evidence-aware board/rules data contracts
2. abstract board-state schema and serialization
3. foul/restart taxonomy and event contract
4. turn/state machine using topology and named restart origins
5. multiplayer authoritative envelope for state/event sync
6. coordinate adapter layer that remains empty/provisional until geometry is cleared
7. only then canonical layout authoring and geometry-backed gameplay tuning

The critical rule is that tranches 1-5 should compile and be testable even if all exact coordinates remain null.

---

## 7. Next research steps
1. Extract higher-signal evidence from official setup/gameplay videos.
2. Capture screenshots from stable frames showing full-board top view or near-top perspective.
3. Try a row-by-row anchor/piece count from official imagery only.
4. Separate three things cleanly in notes:
   - printed field markings
   - active player pieces
   - distinct barrier pieces
5. Only attempt measured coordinates if multiple official frames agree.
6. Halt implementation if geometry remains too uncertain for safe canonical layout constants.
7. If geometry remains blocked, continue with topology-safe specs and data/rules planning rather than forcing board implementation.
