# Specifications: image pipeline (import, tiling, preview)

> Version: 1.0  
> Status: DRAFT | REVIEW | APPROVED  
> Last Updated: 2026-05-08  
> Requirements: [link to 01-requirements.md]

## Overview

Design a cross-platform tiling pipeline and runtime tile renderer suitable for large canvases in Flutter, with compatibility for legacy tile conventions where needed.

## Affected Systems

| System | Impact | Notes |
|--------|--------|-------|
| Import workflow | Create | UI + background job orchestration |
| Tiling engine | Create | Generates tiles + manifest |
| Tile storage | Create/Modify | Bundle layout + cache strategy |
| Renderer | Modify/Create | Renders visible tiles only |
| Validation | Create | Detect missing tiles/manifest mismatch |

## Architecture

### Component Diagram

```
[Import UI] -> [Tiling Job] -> [Tile Store] -> [Manifest]
                     |
                     v
                [Progress/Cancel]

[Renderer] -> [Tile Resolver] -> [Decode Cache] -> [Tile Store]
```

### Data Flow

```
image bytes -> (resize/crop) -> tile encode -> write tiles -> write manifest
render frame -> compute visible tiles -> resolve paths -> decode -> draw -> cache
```

## Interfaces

### New Interfaces

```cpp
interface TilingEngine {
  TilingResult generateTiles(ImageInput input, TilingOptions options);
}

interface TileResolver {
  TileRef resolve(layerId, culture, level, x, y);
}

interface TileCache {
  Image getDecoded(TileRef ref);
}
```

## Data Models

### New Types (conceptual)

```cpp
struct TilingOptions {
  int tileSizePx;          // e.g. 512
  list<double> scales;     // e.g. 1.0, 0.5, 0.25, 0.125
  bool storeOriginal;
}

struct TileManifest {
  int tileSizePx;
  list<Level> levels;      // per-scale metadata + bounds
}
```

## Behavior Specifications

### Happy Path

1. User imports an image for a layer + culture.
2. Tiling job runs in background and emits progress.
3. Tiles + manifest written to bundle/cache.
4. Renderer loads only tiles intersecting viewport at current scale.

### Edge Cases

| Case | Trigger | Expected Behavior |
|------|---------|-------------------|
| Cancel job | user cancels | partial outputs cleaned or marked incomplete |
| Partial tiles exist | interrupted run | validator flags incomplete; UI offers resume/regenerate |
| Huge image | > memory | pipeline uses streaming/chunking; avoids full decode if possible |

### Error Handling

| Error | Cause | Response |
|-------|-------|----------|
| DiskFull | not enough space | fail with actionable message + change location |
| DecodeError | corrupt input | fail import; show diagnostics |

## Dependencies

### Requires

- A cross-platform image processing backend (decision TBD).

## Testing Strategy

### Unit Tests

- [ ] Tile index computation for viewport
- [ ] Manifest generation determinism

### Integration Tests

- [ ] Import large image -> generate tiles -> render pan/zoom without jank (budget defined)

## Migration / Rollout

- Stage 1: Read legacy tiles; new docs may write v2 tile layout.
- Stage 2: Optional tool to re-tile legacy docs into v2 layout.

## Open Design Questions

- [ ] Backend choice for image ops (pure Dart vs native vs server).
- [ ] Tile naming: legacy-compatible vs manifest-only addressing.

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
