# Status: sdd-unity-undo-redo

## Current Phase

REQUIREMENTS

## Phase Status

DRAFTING

## Last Updated

2026-05-08

## Blockers

- Command taxonomy from `sdd-unity-animation-timeline-ui` should stabilize first.

## Progress

- [ ] Requirements drafted
- [ ] Requirements approved
- [ ] Specifications drafted
- [ ] Specifications approved
- [ ] Plan drafted
- [ ] Plan approved
- [ ] Implementation started
- [ ] Implementation complete

## Context Notes

- Legacy WPF had no undo; Unity backlog treats undo as **new product quality**, not parity.
- Must cover filesystem side-effects (layers/sounds) in temp workspace.

## Next Actions

1. Define transaction boundaries for drag/scrub (if any drag UI appears).
2. Choose command stack vs selective snapshots for large tiles.
