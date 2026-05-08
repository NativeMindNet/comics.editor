# Implementation Plan: Canvas Preview with Transforms

> Version: 2.0
> Status: DRAFT
> Last Updated: 2026-05-08
> Specifications: [02-specifications.md](./02-specifications.md)

## Prerequisites

- [x] `sdd-comics.engine-shared-core` implemented (FolderSource, IComicsSource)

## Phase 1: AnimationProcessor Integration

### Task 1.1: Add GetLayerTransforms to ComicsViewer
- [ ] Add method to expose transform state from AnimationProcessor
- [ ] Return array of LayerTransform (position, rotation, scale, alpha)
- **Files**: `unity_comics.engine/Runtime/ComicsViewer.cs`

### Task 1.2: Create StagePreview component
- [ ] Create `Editor/Preview/ComicsStagePreview.cs`
- [ ] Initialize FolderSource from session temp path
- [ ] Cache AnimationProcessor instance
- **Files**: New file in `UnityComicsEditor/Assets/ComicsUnity/Editor/Preview/`

## Phase 2: IMGUI Rendering

### Task 2.1: Basic layer rendering
- [ ] Draw layers with GUI.DrawTexture
- [ ] Apply position offset
- [ ] Apply alpha via GUI.color
- **Files**: `ComicsStagePreview.cs`

### Task 2.2: Transform matrix support
- [ ] Use GUI.matrix for rotation/scale
- [ ] Calculate pivot-relative transforms
- [ ] Restore matrix after each layer
- **Files**: `ComicsStagePreview.cs`

### Task 2.3: Integrate into ComicsEditorWindow
- [ ] Add preview panel area
- [ ] Connect scroll slider to preview
- [ ] Refresh on document changes
- **Files**: `ComicsEditorWindow.cs`

## Phase 3: Texture Management

### Task 3.1: Layer texture caching
- [ ] Load textures from FolderSource
- [ ] Cache by layer + culture
- [ ] Invalidate on layer change
- **Files**: `ComicsStagePreview.cs`

### Task 3.2: Tile composition (optional)
- [ ] Composite tiles into layer texture if needed
- [ ] Or use existing PreviewTextureBuilder
- **Files**: `PreviewTextureBuilder.cs` or new

## Phase 4: Polish

### Task 4.1: Viewport scaling
- [ ] Zoom to fit document in preview area
- [ ] Maintain aspect ratio
- [ ] Optional zoom control

### Task 4.2: Visual indicators
- [ ] Selected layer highlight
- [ ] Layer bounds (optional)
- [ ] Grid overlay (optional)

## Verification

- [ ] Transforms match runtime viewer
- [ ] 60 FPS during scroll scrub
- [ ] Preview updates on document edit
- [ ] Memory stable on repeated edits

---

## Approval

- [x] Plan reviewed by: Anton
- [x] Plan approved on: 2026-05-08
