# Requirements: legacy document format v2 (comics/puzzle)

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08

## Problem Statement

Legacy `.comics` / `.puzzle` documents are zipped bundles with a `data.json` plus assets. The legacy editor relies on Windows-only tooling (7zip + ImageMagick) and has no explicit schema versioning, making cross-platform evolution (Flutter) fragile.

We need a **portable, versioned document format** that can:
- Open legacy documents reliably
- Evolve safely (schema migrations)
- Support large images via tiling and multiple scales
- Preserve editor semantics (layers, animations, sounds, per-culture assets)

## User Stories

### Primary

**As a** comics/puzzle editor user  
**I want** documents to open, edit, and save consistently across platforms  
**So that** I can author content without format breakage or platform-specific steps

### Secondary

- **As a** developer  
  **I want** explicit schema versioning and migrations  
  **So that** we can add features without breaking older content

- **As a** localization/content manager  
  **I want** per-culture assets to be mapped explicitly by culture code  
  **So that** adding/removing cultures doesn’t silently remap images

## Acceptance Criteria

### Must Have

1. **Given** a legacy `.comics` or `.puzzle` archive  
   **When** it is opened by the new editor/loader  
   **Then** it loads layers/sounds/animations and resolves referenced assets identically (within defined tolerances)

2. **Given** a document in the new format (v2)  
   **When** it is saved and re-opened  
   **Then** the content is byte-stable or semantically equivalent (no accidental loss of fields/assets)

3. **Given** the schema changes (new fields / renamed fields)  
   **When** older documents are opened  
   **Then** migrations are applied deterministically and `schemaVersion` is updated accordingly

4. **Given** a document with localized assets  
   **When** cultures are added/removed/reordered in the app  
   **Then** existing documents still map to the correct assets (no index-based coupling)

### Should Have

- A “compatibility mode” that can export back to legacy `.comics/.puzzle` if needed for existing pipelines.
- Checksums/manifest to detect missing assets early.

### Won't Have (This Iteration)

- Server-side validation/signing of documents.
- End-to-end encryption of document contents.

## Constraints

- **Technical**: Must support large canvases; legacy uses tiling (e.g., 512×512) and multi-scale pyramids for puzzles.
- **Performance**: Loading should not require assembling a full mega-bitmap in memory; should support incremental tile loading.
- **Platform**: Must be feasible on iOS/Android/desktop without Windows executables.
- **Dependencies**: Must define an explicit mapping for cultures and assets.

## Open Questions

- [ ] Do we keep zip as the container, or move to a different container (still must bundle assets + JSON)?
- [ ] Do we require lossless round-trip to legacy, or only “read legacy, write v2”?
- [ ] What is the canonical tile naming convention in v2: keep legacy pattern or introduce a manifest-driven layout?
- [ ] How do we represent animations: keep legacy `Start/End` segments with interpolation, or move to keyframes?
- [ ] What is the policy for missing assets (fail open vs fail closed)?

## References

- Legacy editor projects: `legacy/legacy-comics-editor-csharp/Comics.Editor` and models in `Comics.Editor/Models/*`.
- Legacy container behavior: zip via `Comics.Editor/Utils/ZipUtils.cs` (7zip CLI).
- Legacy tiling conventions: `Comics.Editor/Utils/FileManager.cs` and preview assembly in `Comics.Editor/ViewModel/ImagePathConverter.cs`.

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
