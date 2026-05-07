# Requirements: workspace & packaging (import/export, atomic save)

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08

## Problem Statement

Legacy editing unzips a document into a temp folder and repacks it via a Windows 7zip executable. The Flutter rewrite needs a cross-platform approach for:
- opening bundles into a workspace
- managing assets and derived artifacts (tiles)
- saving/exporting atomically and reliably

## User Stories

### Primary

**As a** creator  
**I want** saves to be reliable and not corrupt files  
**So that** I don’t lose work even if something crashes mid-save

### Secondary

- **As a** creator  
  **I want** large documents to open and save reasonably fast  
  **So that** iteration time stays low

- **As a** developer  
  **I want** a clear workspace layout and atomic operations  
  **So that** asset pipelines and undo/redo can be implemented safely

## Acceptance Criteria

### Must Have

1. **Given** a document bundle  
   **When** it is opened  
   **Then** it is mapped to a workspace with predictable paths and metadata

2. **Given** the user saves  
   **When** the save operation completes  
   **Then** the bundle is either fully updated or unchanged (atomic save)

3. **Given** an unexpected interruption during save  
   **When** the user re-opens the document  
   **Then** the editor can recover safely (no silent corruption)

### Should Have

- Background saving with progress for large bundles.
- Optional “Save As…” exporting to alternate formats/containers if needed.

### Won't Have (This Iteration)

- Full multi-user collaboration and merging.

## Constraints

- **Platform**: must work on iOS/Android/desktop.
- **Correctness**: asset references must always remain valid.
- **Performance**: avoid unnecessary full re-packaging when only small metadata changes (if possible).

## Open Questions

- [ ] Keep zip container or choose a different single-file container?
- [ ] Do we operate on an extracted workspace folder, or read/write streams directly?
- [ ] What are the limits on bundle size and number of files (tiles)?
- [ ] How do we handle partial derived artifacts (tiles) during save?

## References

- Legacy workspace: `legacy/legacy-comics-editor-csharp/Comics.Editor/Utils/FileManager.cs`
- Legacy zip tooling: `legacy/legacy-comics-editor-csharp/Comics.Editor/Utils/ZipUtils.cs` and bundled `Utils/7za.exe`

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
