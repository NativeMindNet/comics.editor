# Status: vdd-legacy-format

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

- Legacy editor is WPF (.NET 4.5.2) and stores `.comics` / `.puzzle` as a zip with `data.json` + `layers/` + `sounds/`.
- Legacy uses external CLIs: `7za.exe` for zip and ImageMagick `magick.exe` for tiling/resizing.
- Legacy culture-to-image mapping is list-index-based (`CulturesHelper.All`), which is brittle for schema evolution.

## Fork History

- N/A

## Next Actions

1. Draft `01-requirements.md` around a versioned, explicit, portable document schema.
2. Decide compatibility matrix (read legacy / write legacy / write v2).
