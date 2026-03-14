# ART_BRANDING_SYSTEM

Status: draft v0

## Primary requirement
Build the game so it can present as an exact-clone-themed product during internal development, while preserving a fast path to a generic/original identity for public or legal/commercial pivoting.

## Hard rule
No branding-sensitive asset should be required by the gameplay engine.

## Required content separation
- **Core gameplay layer**: rules, board zones, collision layout, turn logic, state machine
- **Theme layer**: logos, names, colors, board skin art, menu art, audio identity, store copy variants

## Theme-pack design direction
A theme pack should define:
- product name string set
- app icon candidates
- board surface textures
- team/side naming
- UI palette
- goal/marker cosmetic style
- legal copy / attribution notes if needed

## Fallback requirement
The project must always have a fully generic first-party theme that can ship on its own.

## Benefits
- reduces risk if exact branding becomes unacceptable
- supports monetizable cosmetic expansion
- enables A/B testing of presentation
