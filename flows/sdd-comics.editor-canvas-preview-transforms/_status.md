# Status: sdd-unity-canvas-preview-transforms

## Current Phase

REQUIREMENTS

## Phase Status

DRAFTING

## Last Updated

2026-05-08

## Blockers

- Depends on `sdd-comics.engine-shared-core` for FolderSource/AnimationProcessor
- Transform composition order clarified in ADR-006

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

- WPF: `LayersControl.xaml` composes translate/rotate/scale/alpha on a canvas.
- Unity v1: vertical list of textures + scroll numbers; not a single composed stage.

## Next Actions

1. Capture transform composition order and pivot normalization from WPF.
2. Define hit-testing scope (phase 2) if selection handles are in-scope.
