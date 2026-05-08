# Status: vdd-legacy-image-pipeline

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

- Legacy editor generates tiles via ImageMagick CLI (`magick.exe`) and expects specific naming patterns.
- Legacy preview sometimes assembles a full bitmap in memory (WPF `RenderTargetBitmap`), which is expensive for large assets.
- Puzzle uses multi-scale pyramids; comics often use scale 1.0 only.

## Fork History

- N/A

## Next Actions

1. Define cross-platform tiling pipeline requirements and perf budgets.
2. Lock tile layout + manifest approach and how preview is rendered without full assembly.
