# Requirements: image pipeline (import, tiling, preview) for legacy parity

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08

## Problem Statement

Legacy uses Windows-only ImageMagick CLI to resize/crop images and generate tiled pyramids. The editor also builds some previews by composing many tiles into a single bitmap, which is slow and memory-heavy.

We need a **cross-platform image pipeline** that supports import, tiling, and previews efficiently on Flutter targets without requiring external executables.

## User Stories

### Primary

**As a** content creator  
**I want** to import large images and see them immediately on the canvas  
**So that** I can place and animate layers without long waits or crashes

### Secondary

- **As a** developer  
  **I want** deterministic tiling output  
  **So that** we can cache, validate, and debug asset issues reliably

- **As a** QA/ops person  
  **I want** validation for missing/corrupt tiles  
  **So that** broken bundles are detected before shipping

## Acceptance Criteria

### Must Have

1. **Given** an imported image (large, e.g. 10k×10k)  
   **When** the pipeline runs  
   **Then** it produces a tile set (and optional multi-scale pyramid) without exhausting memory

2. **Given** a tiled image set  
   **When** it is previewed on canvas  
   **Then** it renders by streaming visible tiles (no full-size bitmap assembly required)

3. **Given** a legacy `.puzzle` document with existing tiles  
   **When** it is opened  
   **Then** the loader can resolve and render tiles correctly (legacy naming supported or mapped)

### Should Have

- Background/async processing with progress and cancel.
- Optional re-tiling when tile size or pyramid strategy changes.

### Won't Have (This Iteration)

- GPU-accelerated offline tile generation (unless needed).
- Advanced formats beyond common PNG/JPEG (unless required by existing assets).

## Constraints

- **Performance**: tile decode/render must be smooth while panning/zooming.
- **Platform**: must run on iOS/Android; no Windows binaries.
- **Determinism**: same input + settings => same tile outputs and manifest.

## Open Questions

- [ ] Standard tile size (legacy appears to use 512×512). Keep or change?
- [ ] Pyramid levels (legacy puzzle uses 1.0/0.5/0.25/0.125). Keep?
- [ ] Do we store `original` full-res image in the bundle or only tiles?
- [ ] Preferred image processing backend in Flutter (pure Dart vs native plugin vs server-side)?

## References

- Legacy tiling pipeline and naming: `legacy/legacy-comics-editor-csharp/Comics.Editor/Utils/FileManager.cs`
- Legacy preview assembly: `legacy/legacy-comics-editor-csharp/Comics.Editor/ViewModel/ImagePathConverter.cs`
- Legacy ImageMagick wrapper: `legacy/legacy-comics-editor-csharp/Comics.Editor/IWS/Utils/ImageMagick.cs`

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
