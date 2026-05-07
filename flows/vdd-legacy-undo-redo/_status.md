# Status: vdd-legacy-undo-redo

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

- Legacy editor appears to mutate model/files directly and does not have a dedicated undo/redo system.
- Flutter rewrite should add undo/redo early; it affects tool design (drag = one action) and asset operations.
- Must cover both model edits (transforms/anims) and asset operations (import/delete/retile).

## Fork History

- N/A

## Next Actions

1. Draft requirements around action granularity, transactions, and asset-aware history.
2. Decide architecture: command pattern vs snapshot/diff (or hybrid).
