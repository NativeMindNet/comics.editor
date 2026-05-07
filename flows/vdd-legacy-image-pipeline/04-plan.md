# Implementation Plan: image pipeline (import, tiling, preview)

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Specifications: [link to 03-specifications.md]

## Summary

Build a background tiling pipeline that writes tiles + manifest and a renderer that streams visible tiles with caching.

## Task Breakdown

### Phase 1: Foundation

#### Task 1.1: Choose image processing backend
- **Description**: Decide and prototype the backend for resize/crop/tile encode on target platforms.
- **Files**:
  - `flows/vdd-legacy-image-pipeline/03-specifications.md` - Modify
- **Dependencies**: None
- **Verification**: Prototype can tile a large image deterministically
- **Complexity**: High

#### Task 1.2: Finalize tile manifest + directory layout
- **Description**: Lock the on-disk layout and manifest fields used by renderer.
- **Files**:
  - `flows/vdd-legacy-image-pipeline/03-specifications.md` - Modify
- **Dependencies**: Task 1.1
- **Verification**: Renderer can resolve tiles from manifest without heuristics
- **Complexity**: Medium

### Phase 2: Core Implementation

#### Task 2.1: Implement tiling job + progress/cancel
- **Description**: Background job that generates tiles and writes outputs atomically.
- **Files**:
  - `lib/...` - Create/Modify
- **Dependencies**: Task 1.2
- **Verification**: Cancel leaves no “half-valid” state
- **Complexity**: High

#### Task 2.2: Implement tile resolver + caches
- **Description**: Resolve visible tiles for viewport/scale and cache decoded images.
- **Files**:
  - `lib/...` - Create/Modify
- **Dependencies**: Task 2.1
- **Verification**: Pan/zoom reuses cache; bounded memory
- **Complexity**: High

### Phase 3: Integration

#### Task 3.1: Connect layer rendering to tile resolver
- **Description**: Canvas draws visible tiles; no full-stitch preview step.
- **Files**:
  - `lib/...` - Modify
- **Dependencies**: Task 2.2
- **Verification**: Smooth pan/zoom on large tiled layers
- **Complexity**: High

### Phase 4: Testing & Polish

#### Task 4.1: Perf regression tests + fixtures
- **Description**: Add fixtures and measure frame time under pan/zoom workloads.
- **Files**:
  - `test/...` - Create/Modify
- **Dependencies**: Task 3.1
- **Verification**: Meets defined perf budget on representative devices
- **Complexity**: Medium

## Dependency Graph

```
Task 1.1 -> Task 1.2 -> Task 2.1 -> Task 2.2 -> Task 3.1 -> Task 4.1
```

## Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| Backend can’t handle huge images | Med | High | streaming/chunking or server-side fallback |
| Cache memory spikes | Med | High | strict LRU, tile budget per layer/viewport |

## Rollback Strategy

1. Keep legacy tile reading path intact.
2. Gate new tiling behind a flag until stable.

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
