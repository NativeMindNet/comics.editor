# Implementation Plan: Unity animation & layer inspector UI

> Version: 2.0
> Status: DRAFT
> Last Updated: 2026-05-08
> Specifications: [02-specifications.md](./02-specifications.md)

## Summary

Implement inspector + dual-rail timeline in IMGUI, building incrementally from data layer to UI.

## Task Breakdown

### Phase 1: Data Layer & Session APIs

#### Task 1.1: Add animation CRUD to ComicsEditorSession
- **Files**: `ComicsEditorSession.cs`
- **Changes**:
  - `AddAnim(int layerIndex, AnimTypes type)` → returns new Anim
  - `RemoveAnim(int layerIndex, Anim anim)`
  - `SelectedAnim` property with change notification
  - `SyncScrollToSelection` bool property
- **Complexity**: Low

#### Task 1.2: Add sound animation CRUD to ComicsEditorSession
- **Files**: `ComicsEditorSession.cs`
- **Changes**:
  - `AddSoundAnim(int soundIndex)` → returns new SoundAnim
  - `RemoveSoundAnim(int soundIndex, Anim anim)`
  - `SelectedSoundAnim` property
- **Complexity**: Low

#### Task 1.3: Add image/popup change APIs
- **Files**: `ComicsEditorSession.cs`, `LayerModel.cs`
- **Changes**:
  - `SetLayerImage(int layerIndex, Cultures culture, string filePath)`
  - `SetLayerPopup(int layerIndex, Cultures culture, string filePath)`
  - Use existing `FileManagerUnity` for file operations
- **Complexity**: Medium

### Phase 2: Animation Inspector Component

#### Task 2.1: Create AnimationInspector base
- **Files**: New `Editor/Inspector/AnimationInspector.cs`
- **Changes**:
  - `Draw(Rect rect, Anim anim)` method
  - Base fields: Start, End (int fields)
  - Header with anim type name
- **Complexity**: Low

#### Task 2.2: Type-specific inspectors
- **Files**: `AnimationInspector.cs`
- **Changes**:
  - Switch on `anim.Type` to draw type-specific fields:
    - Translate: X, Y
    - Rotate: PivotX, PivotY, Angle
    - Scale: PivotX, PivotY, ScaleX, ScaleY
    - Alpha: Alpha slider (0-1)
    - Sound: (base only)
  - Delete button with confirmation
- **Complexity**: Medium

#### Task 2.3: Layer inspector (non-anim fields)
- **Files**: `AnimationInspector.cs` or new `LayerInspector.cs`
- **Changes**:
  - Change Image button per culture
  - Change Popup button per culture
  - Layer position (X, Y, Width, Height) - read-only or editable
- **Complexity**: Medium

### Phase 3: Timeline Component

#### Task 3.1: Create AnimationTimeline shell
- **Files**: New `Editor/Timeline/AnimationTimeline.cs`
- **Changes**:
  - `Draw(Rect rect, ComicsEditorSession session)` method
  - Draw ruler with scroll scale
  - Draw playhead at current scroll
  - Calculate pixels-per-scroll-unit based on zoom
- **Complexity**: Medium

#### Task 3.2: Draw layer animation segments
- **Files**: `AnimationTimeline.cs`
- **Changes**:
  - For selected layer, iterate `Layer.Animations`
  - Draw colored rect for each segment
  - Highlight selected segment
  - Segment colors by type
- **Complexity**: Medium

#### Task 3.3: Draw sound animation segments
- **Files**: `AnimationTimeline.cs`
- **Changes**:
  - Second rail below layer rail
  - For selected sound (or all sounds?), iterate `Sound.Animations`
  - Draw red segments for SoundAnim
- **Complexity**: Low

#### Task 3.4: Segment click selection
- **Files**: `AnimationTimeline.cs`
- **Changes**:
  - Hit-test mouse click against segment rects
  - Set `session.SelectedAnim` on click
  - Implement auto-seek logic (if enabled)
- **Complexity**: Medium

#### Task 3.5: Segment drag resize/move
- **Files**: `AnimationTimeline.cs`
- **Changes**:
  - Detect drag on segment edges → resize Start or End
  - Detect drag on segment middle → move (adjust both)
  - Clamp to valid ranges
  - Update model on drag end
- **Complexity**: High

#### Task 3.6: Timeline zoom
- **Files**: `AnimationTimeline.cs`
- **Changes**:
  - Mouse wheel in timeline area adjusts zoom level
  - Zoom centered on mouse position
  - Min/max zoom limits
- **Complexity**: Medium

### Phase 4: Integration into ComicsEditorWindow

#### Task 4.1: Restructure window layout
- **Files**: `ComicsEditorWindow.cs`
- **Changes**:
  - Split right panel: Preview (top), Timeline (middle), Inspector (bottom)
  - Use `EditorGUILayout.BeginVertical` with flex heights
  - Add splitter handles (optional)
- **Complexity**: Medium

#### Task 4.2: Wire up inspector
- **Files**: `ComicsEditorWindow.cs`
- **Changes**:
  - Instantiate `AnimationInspector`
  - Pass `session.SelectedAnim` to inspector
  - Handle inspector field changes → update model → invalidate preview
- **Complexity**: Low

#### Task 4.3: Wire up timeline
- **Files**: `ComicsEditorWindow.cs`
- **Changes**:
  - Instantiate `AnimationTimeline`
  - Pass session to timeline
  - Handle timeline selection → update inspector
- **Complexity**: Low

#### Task 4.4: Add animation buttons to left panel
- **Files**: `ComicsEditorWindow.cs`
- **Changes**:
  - Dropdown/buttons: Add Translate/Rotate/Scale/Alpha
  - Add Sound Anim button (when sound selected)
  - Delete selected anim button
- **Complexity**: Low

#### Task 4.5: Auto-seek toggle
- **Files**: `ComicsEditorWindow.cs`
- **Changes**:
  - Checkbox "Sync scroll to selection"
  - Wire to `session.SyncScrollToSelection`
- **Complexity**: Low

### Phase 5: Polish & Edge Cases

#### Task 5.1: Context menu on timeline
- **Files**: `AnimationTimeline.cs`
- **Changes**:
  - Right-click segment → GenericMenu with Delete, Duplicate
  - Implement duplicate (clone anim, offset Start/End)
- **Complexity**: Medium

#### Task 5.2: Double-click to add
- **Files**: `AnimationTimeline.cs`
- **Changes**:
  - Double-click empty space → popup to choose anim type
  - Add at clicked scroll position
- **Complexity**: Low

#### Task 5.3: Keyboard shortcuts
- **Files**: `ComicsEditorWindow.cs`
- **Changes**:
  - Delete key → delete selected anim
  - Arrow keys → nudge Start/End
- **Complexity**: Low

## Dependency Graph

```
Phase 1 (Data) ─────────────────────────────────────────┐
  1.1 ──┬──► 1.2                                        │
        └──► 1.3                                        │
                                                        ▼
Phase 2 (Inspector) ◄───────────────────────────────────┤
  2.1 ──► 2.2 ──► 2.3                                   │
                                                        │
Phase 3 (Timeline) ◄────────────────────────────────────┤
  3.1 ──► 3.2 ──► 3.3                                   │
        │                                               │
        └──► 3.4 ──► 3.5                                │
        │                                               │
        └──► 3.6                                        │
                                                        ▼
Phase 4 (Integration) ◄─────────────────────────────────┤
  4.1 ──► 4.2 ──► 4.3 ──► 4.4 ──► 4.5                   │
                                                        ▼
Phase 5 (Polish)
  5.1, 5.2, 5.3 (parallel)
```

## File Summary

| File | Action |
|------|--------|
| `ComicsEditorSession.cs` | Modify - add animation/image APIs |
| `ComicsEditorWindow.cs` | Modify - new layout, wire components |
| `Editor/Inspector/AnimationInspector.cs` | New |
| `Editor/Timeline/AnimationTimeline.cs` | New |

## Risk Assessment

| Risk | Mitigation |
|------|------------|
| IMGUI drag handling complexity | Start with click-only, add drag in 3.5 |
| Performance with many segments | Cull segments outside visible range |
| Layout conflicts with existing UI | Test incrementally, preserve left panel |

## Estimated Effort

- Phase 1: ~1 hour
- Phase 2: ~2 hours
- Phase 3: ~4 hours (drag is complex)
- Phase 4: ~1 hour
- Phase 5: ~1 hour

**Total: ~9 hours**

---

## Approval

- [x] Reviewed by: Anton
- [x] Approved on: 2026-05-08
