# Requirements: Unity animation & layer inspector UI parity

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08

## Problem Statement

Animation authoring in Unity is **far behind** WPF: users cannot add/remove all anim types, select segments, edit parameters, or manage **per-culture images and popups** through the UI.

## User Stories

**As a** author  
**I want** the same animation operations as in WPF (translate, rotate, scale, alpha, sound segments)  
**So that** I can recreate scenes without editing JSON  

**As a** author  
**I want** to change images and popups per culture  
**So that** localization matches WPF workflow  

## Acceptance Criteria

### Must Have

1. **Given** a selected layer  
   **When** I add each `AnimTypes` entry (except where not applicable)  
   **Then** a new segment appears and affects evaluation at `Scroll` identically to WPF add rules  

2. **Given** a selected animation segment  
   **When** I edit start/end and type-specific fields  
   **Then** `data.json` reflects changes on save and preview updates  

3. **Given** a selected layer  
   **When** I choose “Change image” / “Change popup” for current culture  
   **Then** files update under `layers/` with conflict rules like WPF (`CheckFile`)  

4. **Given** sounds  
   **When** I add/delete `SoundAnim` segments  
   **Then** segment list matches WPF model patterns  

### Should Have

- Reorder layers with drag-drop (WPF list UX parity).  
- Duplicate segment / copy keyframe range.  

### Won’t Have (This Iteration)

- Bézier curve editor for custom easing (WPF used fixed `Factor()` only unless extended elsewhere).

## Constraints

- Must integrate with future undo/redo (`sdd-unity-undo-redo`) — commands should be structured.

## Design Decisions

1. **Two synced rails** - Layer animations and sound animations displayed as separate but scroll-synced rails in the timeline.

2. **Auto-seek ON with toggle** - Selecting an animation segment auto-seeks scroll to segment start. Toggle "Sync scroll to selection" allows disabling when comparing across scroll positions.

3. **IMGUI implementation** - Consistent with existing `ComicsEditorWindow`, faster to implement, bounded complexity. Can migrate to UIToolkit later if needed.

## References

- `Comics.Editor/ViewModel/LayerViewModel.cs`, `SoundViewModel.cs`, `Controls/*AnimControl*`
- `UnityComicsEditor/.../ComicsEditorWindow.cs`
- VDD: `flows/vdd-legacy-animation-timeline/`

---

## Approval

- [x] Reviewed by: Anton
- [x] Approved on: 2026-05-08
