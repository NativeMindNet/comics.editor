# Status: vdd-legacy-rendering

## Current Phase

REQUIREMENTS

## Phase Status

DRAFTING

## Last Updated

2026-05-08 by GPT-5.2

## Blockers

- None

## Progress

- [ ] Requirements drafted
- [ ] Requirements approved
- [ ] Visual mockups drafted
- [ ] Visual approved
- [ ] Specifications drafted
- [ ] Specifications approved
- [ ] Plan drafted
- [ ] Plan approved
- [ ] Implementation started
- [ ] Implementation complete
- [ ] Documentation drafted
- [ ] Documentation approved

## Context Notes

Key decisions and context for resuming:

- Legacy renderer is WPF `Canvas`/`ItemsControl` with transforms (translate/rotate/scale/opacity) and selection overlays.
- Layer state is driven by animation segments (start/end) and interpolations.
- Tile-based images are previewed by stitching in legacy; new renderer should stream tiles.

## Fork History

- N/A

## Next Actions

1. Define scene graph + hit-testing model in Flutter.
2. Define performance budgets and caching strategy for large scenes.
