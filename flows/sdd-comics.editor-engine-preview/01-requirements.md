# Requirements: Comics Editor Engine Preview ("Preview as Player")

> Version: 1.0
> Status: DRAFT
> Last Updated: 2026-05-08

## Problem Statement

The editor's inline preview (`sdd-comics.editor-canvas-preview-transforms`) is optimized for editing: instant updates, selection handles, no audio. Authors need a way to validate the **full runtime experience** before publishing, including:

- Scroll behavior matching runtime viewer
- Audio playback at correct scroll positions
- Full viewport rendering (not cropped to editor panel)

## User Stories

**As a** content author
**I want** to preview my comics exactly as players will see it
**So that** I can catch timing/audio issues before publishing

**As a** QA tester
**I want** one-click preview without building an archive
**So that** I can iterate quickly on feedback

**As a** sound designer
**I want** to hear sound triggers during preview
**So that** I can adjust start/end scroll values

## Acceptance Criteria

### Must Have

1. **Given** a document loaded in editor
   **When** user clicks "Preview as Player"
   **Then** a preview window opens showing the document with full scroll + audio

2. **Given** preview is running
   **When** user scrolls (wheel/touch/slider)
   **Then** animations play with same timing as runtime viewer

3. **Given** preview is running with sounds
   **When** scroll passes sound trigger range
   **Then** audio plays (same as runtime)

4. **Given** preview window
   **When** user presses Escape or closes window
   **Then** preview stops and editor regains focus

### Should Have

- Keyboard shortcut (Ctrl+P or similar)
- Viewport size selector (phone/tablet/desktop presets)
- Scroll position indicator

### Won't Have (This Iteration)

- Touch simulation (pinch-zoom)
- Performance profiling overlay
- Export to standalone build

## Constraints

- Must work in Unity Editor (not Play mode required)
- Audio playback via Editor audio API
- Cannot modify document during preview (read-only mode)

## Dependencies

- `sdd-comics.engine-shared-core`: FolderSource implementation
- `sdd-comics.engine-csharp-unity`: ComicsViewer MonoBehaviour
- `sdd-comics.editor-audio-preview`: Audio playback in editor

## Open Questions

- [ ] Modal window (blocks editor) vs floating window (parallel editing)?
- [ ] Game view simulation vs custom EditorWindow?

## References

- `app/unity_comics.engine/Runtime/ComicsViewer.cs` - runtime viewer
- `app/unity_comics.editor/UnityComicsEditor/` - editor window

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
