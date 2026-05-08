# Requirements: Unity asset pipeline fidelity (zip, tiling, JSON, Convert)

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08

## Problem Statement

Unity uses different tooling than WPF (`ZipFile` vs `7za`, Texture2D pipeline vs ImageMagick). Without explicit requirements, **saved bundles may diverge** from what legacy editors/runtimes expect, or **previews may lie** about final pixels.

## User Stories

**As a** creator  
**I want** documents saved from Unity to open in the old editor and in players  
**So that** I don’t break existing pipelines  

**As a** QA engineer  
**I want** measurable tolerances for image/zip differences  
**So that** we know regressions from real bugs vs accepted deltas  

## Acceptance Criteria

### Must Have

1. **Given** a representative set of legacy `.comics`/`.puzzle` archives  
   **When** Unity opens and saves them without edits  
   **Then** extracted contents remain functionally equivalent (layout, `data.json` semantics, required assets present) per defined checklist  

2. **Given** a new image imported as a layer  
   **When** Unity generates tiles  
   **Then** file naming matches legacy pattern `name_{scaleInt}_{col}_{row}` and puzzle placeholder `name_ph_0_0` when applicable  

3. **Given** `data.json` with polymorphic `animations`  
   **When** deserialized in Unity  
   **Then** round-trip serialization preserves type tags and fields compatible with WPF Newtonsoft settings intent  

### Should Have

- Port or replace WPF `Convert()` workflow (flatten → retile) with explicit UX.  

### Won’t Have (This Iteration)

- Pixel-identical output vs ImageMagick for all edge-case color profiles.

## Constraints

- Unity Editor 2022.3 LTS baseline; no Windows-only binaries in pipeline.
- Large images must not require loading full-resolution single texture for *tile generation* (streaming/chunk strategy acceptable).

## Open Questions

- [ ] Are zip entry order and compression level observable by any consumer?
- [ ] Must non-PNG sources stay as JPEG in archive, or is PNG tile output acceptable if runtime accepts it?

## References

- `Comics.Editor/Utils/ZipUtils.cs`, `FileManager.cs`, `IWS/Utils/ImageMagick.cs`
- `UnityComicsEditor/.../ZipUtility.cs`, `TileGeneratorUnity.cs`, `PreviewTextureBuilder.cs`
- Parent: `flows/sdd-unity-parity-gaps-overview/`

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
