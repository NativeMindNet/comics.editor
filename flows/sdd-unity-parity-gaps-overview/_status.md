# Status: sdd-unity-parity-gaps-overview

## Current Phase

REQUIREMENTS

## Phase Status

DRAFTING

## Last Updated

2026-05-08

## Blockers

- None

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

- Unity port lives under `app/unity_comics.editor/UnityComicsEditor/` (EditorWindow + core models).
- WPF reference remains `app/unity_comics.editor/Comics.Editor/`.
- Child SDD flows split missing vs simplified concerns for parallel work.

## Next Actions

1. Review gap matrix in `01-requirements.md`.
2. Prioritize child flows for implementation order.

## Child flows (deep specs)

| Flow | Topic |
|------|--------|
| `sdd-unity-canvas-preview-transforms` | One canvas, z-order, pivot/rotate/scale/alpha |
| `sdd-unity-animation-timeline-ui` | Full anim CRUD + segment UX |
| `sdd-unity-audio-preview` | MP3 preview, SoundAnim, mute |
| `sdd-unity-undo-redo` | History for model + assets |
| `sdd-unity-asset-pipeline-fidelity` | Tiling/zip/json vs legacy tools |
