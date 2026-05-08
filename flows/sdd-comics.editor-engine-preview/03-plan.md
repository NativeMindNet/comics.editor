# Plan: Comics Editor Engine Preview

> Version: 1.0
> Status: DRAFT
> Last Updated: 2026-05-08

## Prerequisites

- [x] `sdd-comics.engine-shared-core` implemented (FolderSource)
- [ ] `sdd-comics.editor-audio-preview` at least partially implemented (can defer audio)

## Phase 1: Basic Preview Window

### Task 1.1: Create PreviewWindow
- [ ] Create `Editor/ComicsPreviewWindow.cs`
- [ ] Add menu item "Comics/Preview as Player"
- [ ] Basic window layout with viewport rect

### Task 1.2: Integrate ComicsViewer
- [ ] Create hidden GameObject with ComicsViewer
- [ ] Connect to FolderSource from editor session
- [ ] Setup Camera + RenderTexture

### Task 1.3: Render viewport
- [ ] Draw RenderTexture to EditorWindow
- [ ] Handle aspect ratio scaling

## Phase 2: Scroll Interaction

### Task 2.1: Scroll slider
- [ ] Add scroll slider to toolbar
- [ ] Connect to ComicsViewer.SetScrollOffset()

### Task 2.2: Mouse wheel
- [ ] Handle ScrollWheel events in viewport
- [ ] Smooth scrolling animation (optional)

### Task 2.3: Keyboard navigation
- [ ] Arrow keys for step scroll
- [ ] Home/End for jump

## Phase 3: Viewport Configuration

### Task 3.1: Size presets
- [ ] Add preset buttons (1080x1920, etc.)
- [ ] Resize RenderTexture on change

### Task 3.2: Custom size
- [ ] Width/Height input fields
- [ ] Aspect ratio lock toggle

## Phase 4: Audio Integration

### Task 4.1: Enable sound in preview
- [ ] Call `_viewer.SetSoundEnabled(true)`
- [ ] Use EditorSoundManager for playback

### Task 4.2: Volume control
- [ ] Add volume slider
- [ ] Mute button

## Phase 5: Polish

### Task 5.1: Keyboard shortcuts
- [ ] Ctrl+Shift+P to open
- [ ] Escape to close
- [ ] Space for auto-scroll toggle

### Task 5.2: State persistence
- [ ] Remember last viewport size
- [ ] Remember window position

### Task 5.3: Error handling
- [ ] Handle missing document
- [ ] Handle viewer initialization failures

## Verification

- [ ] Preview matches runtime viewer behavior
- [ ] Audio plays at correct scroll positions
- [ ] All viewport sizes render correctly
- [ ] No memory leaks on open/close cycles

---

## Approval

- [x] Plan reviewed by: Anton
- [x] Plan approved on: 2026-05-08
