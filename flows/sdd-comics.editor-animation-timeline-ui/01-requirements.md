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

## Open Questions

- [ ] Single “timeline” control shared by layer + sound, or two synced rails?
- [ ] Should changing `SelectedAnim` auto-seek `Scroll` like WPF `LayerViewModel.SelectedAnim`?

## References

- `Comics.Editor/ViewModel/LayerViewModel.cs`, `SoundViewModel.cs`, `Controls/*AnimControl*`
- `UnityComicsEditor/.../ComicsEditorWindow.cs`
- VDD: `flows/vdd-legacy-animation-timeline/`

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
