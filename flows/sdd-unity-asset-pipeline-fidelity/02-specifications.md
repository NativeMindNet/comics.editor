# Specifications: Unity asset pipeline fidelity

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Requirements: [01-requirements.md](./01-requirements.md)

## Overview

Define **compatibility rules** for archives and **implementation constraints** for Unity’s zip and tiling pipeline, including explicit **known deltas** vs WPF.

## Affected Systems

| System | Impact |
|--------|--------|
| `ZipUtility` | Specify extraction/create semantics, temp cleanup |
| `TileGeneratorUnity` | Resize/crop rules vs ImageMagick `-resize … ^ -extent` |
| `ComicsJson` | Confirm Newtonsoft settings parity |
| Tests / fixtures | Add golden archives |

## Architecture

```
[Legacy archive] --> [Extract] --> [Load JSON + assets] --> [Validate manifest rules]
[Import image] --> [Scale pyramid] --> [Tile split 512] --> [Write layers/]
[Save] --> [Write data.json] -->      [Zip create] --> [.comics/.puzzle]
```

## Behavior Specifications

### Zip

- Extraction must recreate folder layout: `data.json`, `layers/`, `sounds/` (as present).
- Creation must include same logical paths; compression may differ from `7za` if no consumer depends on raw bytes.

### Tiling

- `scaleInt = (int)(scale * 1000)` for scales in `ComicsScales` / `PuzzleScales`.
- Tile size 512; partial tiles at edges allowed; file naming must allow glob `string.Format(fileTemplate, scaleInt, "*", "*")` as in WPF preview logic.

### Convert (from WPF)

- Recreate semantics: copy non-tiled layer files to staging, call same update/tile path as “import”.

### Error handling

| Condition | Response |
|-----------|----------|
| Missing tile on disk | Block save or mark document invalid; list paths |
| JSON missing `$type` for anim | Fallback or migration policy documented |

## Testing Strategy

- [ ] Fixture: minimal comics + puzzle archives checked into `Tests/` or external LFS.
- [ ] Round-trip: extract → Unity save → re-extract → compare file sets (allow compression diffs).
- [ ] Visual diff sample: one large PNG through WPF vs Unity tiling (SSIM or perceptual threshold TBD).

## Open Design Questions

- [ ] Central `IArchiveIO` abstraction for future non-zip containers?

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
