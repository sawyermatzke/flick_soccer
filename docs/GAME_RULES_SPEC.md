# GAME_RULES_SPEC

Status: draft v2

This document separates confirmed public rules from unresolved or digitally adapted rules.

## 1. Confirmed from public official rules page
Sources:
- https://binhoboard.com/pages/rules
- mirrored public text at https://binhoboard.eu/pages/rules

Confirmed public rules currently captured:
- Game starts with the ball at center field.
- Players/teams alternate shooting from where the ball comes to a complete stop.
- In 2v2, shot order alternates by team and player sequence.
- Game is first team to 7 goals.
- Ball must completely cross the goal line to count.
- If the ball fully crosses and bounces out, it still counts as a goal.
- After each goal, the team scored on kicks off from center field.

### Publicly listed foul concepts
Official page currently lists these foul concepts:
- double touch / inadvertent touch before shot
- touching the goal crossbar with finger during a shot
- shooting ball out of bounds
- moving the board during the act of shooting
- shooter’s head crossing midfield line during shot
- special handling when ball is against bands/sideline/baseline

### Confirmed rule-space geometry implications
These affect how the rules spec should be interpreted even before exact coordinates are known:
- There is a meaningful **midfield line** used in at least two ways:
  - shot posture constraint in physical play
  - red-card penalty shot region defined as anywhere behind midfield line
- There is an **own box** / defensive box concept per side, because several fouls escalate from yellow to red when committed within that area.
- Sideline, baseline, and band interactions are first-class gameplay situations, so any digital board model must preserve edge-state semantics even if the exact physical manipulation is adapted.

## 2. Confirmed roster/setup facts that affect rules interpretation
From official/public product and accessory snippets on binhoboard.com surfaced in search results:
- A standard board uses **20 total player pieces**.
- That is described as **10 per side**.

Implications for rules work:
- The open question of whether all 20 pieces are active in standard play is now much narrower. Current best reading is that the standard field uses **10 active player pieces per side**.
- Because official visuals also support a mirrored fixed setup, digital rules and UX should assume a formation-based board state rather than free placement.
- The exact spatial arrangement of those 10 pieces per side is still unresolved, so any rule that depends on exact coordinates remains blocked on layout research.

## 3. Confirmed but physical-only or partially physical rules
These exist publicly but do not map cleanly to mobile as written:
- yellow/red cards as physical disciplinary objects
- loss of a physical player after a red determined by card flip
- touching crossbar with finger
- moving the board while shooting
- shooter head crossing midfield line
- moving band with shooting hand only

## 4. Digital adaptation direction for yellow/red
Per project direction, do not use literal physical-card behavior.

### Design goal
Preserve the competitive intent of penalties:
- punish input mistakes or illegal actions
- create escalating consequences
- remain legible and fair in multiplayer
- avoid arbitrary-feeling penalties tied to non-existent physical affordances

### Proposed digital replacement system (draft)
#### Warning / Major Foul model
Instead of physical yellow/red cards represented as cards on a table:
- **Warning** = opponent gets a center restart free shot / advantageous restart
- **Major Foul** = opponent gets a penalty shot plus a temporary team disadvantage state or targeted formation loss, depending on confirmed physical roster behavior

Because the standard side count now appears to be 10 pieces per side, a future digital disadvantage mechanic can safely assume a formation-level consequence is conceptually plausible. But the exact mechanism is still blocked on confirmed formation geometry.

### Working recommendation
Use these provisional digital consequences until layout research is complete:
- **Minor Foul**: opponent restart from center with exclusive next shot
- **Major Foul**: opponent penalty shot from designated penalty origin or penalty region + offender loses one reversible digital advantage for the next phase

This section is intentionally provisional and must be revisited before implementation.

## 5. Foul and restart taxonomy boundary
This section defines the highest-confidence pre-geometry rules boundary.

### 5.1 Taxonomy principle
Before geometry is coordinate-ready, fouls and restarts should classify by:
- event type
- side responsibility
- zone/edge semantics
- restart entitlement
- whether an additional digital penalty applies

They should not yet classify by exact spots, distances, or lane geometry.

### 5.2 Foul classes
#### Confirmed foul triggers from public rules
- inadvertent or double touch before shot
- touching goal crossbar with finger during shot
- shooting ball out of bounds
- moving the board during the act of shooting
- shooter head crossing midfield line during shot

#### Recommended digital classification boundary
- **Shot-procedure foul**
  - illegal execution of an attempted shot
  - examples: double touch, crossbar touch, illegal posture surrogate
- **Board/interaction foul**
  - illegal external interference with board state
  - examples: moving the board during shot; physical-band manipulation equivalents if retained digitally
- **Boundary foul**
  - shot result leaves field-of-play in a foul-qualified way
  - example: ball shot out of bounds when that is not merely a neutral dead-ball condition
- **Major foul**
  - any foul the final design escalates to penalty-shot entitlement and/or temporary disadvantage

This is a classification boundary, not yet a final severity table.

### 5.3 Restart classes
Use restart names that are semantically stable even before exact geometry exists.

- **centerKickoffRestart**
  - used at game start and after goals
  - source basis: confirmed
- **placeOfRestTurnContinuation**
  - normal alternating play from where ball stops
  - source basis: confirmed
- **advantageRestart**
  - opponent receives exclusive next shot from a named restart origin because of a minor foul
  - source basis: designed-for-digital
- **penaltyRestart**
  - opponent receives a penalty shot from a penalty-qualified origin/region
  - source basis: confirmed in concept, digitally adapted in execution
- **deadBallBoundaryRestart**
  - used if digital rules need an explicit controlled restart after edge/out-of-bounds states
  - source basis: designed-for-digital

### 5.4 Event contract boundary
A foul/restart event contract can be finalized now if it contains semantic references only.

Suggested minimum fields:
```json
{
  "eventType": "foulCalled",
  "foulClass": "shotProcedure",
  "severity": "minor",
  "offendingSide": "north",
  "awardedRestart": {
    "restartType": "advantageRestart",
    "beneficiarySide": "south",
    "originRef": "center_restart",
    "resolutionState": "provisional"
  },
  "source": {
    "status": "designedForDigital",
    "notes": "Maps physical foul consequence into digital restart without final coordinates."
  }
}
```

### 5.5 Boundary between safe now vs blocked until geometry
Safe to finalize now:
- foul categories
- restart category names
- who benefits from a foul
- whether possession/shot exclusivity changes
- score/reset semantics after goals
- whether a consequence removes/locks/suspends an entity in abstract terms

Still blocked on geometry:
- exact penalty origin point
- exact own-box polygon / foul-inside-box checks
- exact piece-removal or formation-loss consequences tied to a specific coordinate slot
- exact boundary restart placement rules

## 6. Open rule questions
- Exact standard starting formation for the 10 active pieces per side
- Exact penalty-shot placement constraints in digital form
- Whether official rules differ across product versions
- Whether a digital game should preserve strict turn alternation or support specific restart exceptions
- Which physical edge/band rules should become simplified digital collision/restart logic rather than literal hand-placement restrictions
- Which confirmed foul concepts should map to minor vs major severity in the first digital ruleset

## 7. Implementation rule policy
- Only mark a rule as final once linked to a source or deliberate design decision.
- Distinguish `Confirmed`, `Inferred`, and `Designed for digital` in all later revisions.
- Do not let physical-only officiating language leak directly into touch/mobile UX without an explicit digital reinterpretation step.
- Do not finalize penalty placement or formation-loss behavior until board geometry clears the implementation gate in `BOARD_LAYOUT_SPEC.md`.
