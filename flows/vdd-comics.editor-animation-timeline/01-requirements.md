# Requirements: animation timeline (segments, preview, editing)

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08

## Problem Statement

Legacy editing relies on multiple animation types applied to layers and sounds, expressed as time segments with `Start/End` and interpolation. The Flutter rewrite needs a timeline model and UI that can create, edit, and preview these animations predictably and efficiently.

## User Stories

### Primary

**As a** creator  
**I want** to add and edit animations on a timeline  
**So that** I can control motion, rotation, scale, and opacity over time

### Secondary

- **As a** creator  
  **I want** to preview animations by scrubbing and playing  
  **So that** I can tune timing and easing quickly

- **As a** creator  
  **I want** segment editing (move/resize) to be precise  
  **So that** I can align events to specific moments

## Acceptance Criteria

### Must Have

1. **Given** a layer with animation segments  
   **When** I play the timeline  
   **Then** the layer’s transform/opacity follows the animations correctly

2. **Given** an animation segment  
   **When** I drag its start/end handles  
   **Then** the segment resizes and playback reflects the new timing immediately

3. **Given** I scrub the playhead  
   **When** I stop at time T  
   **Then** the scene shows the evaluated state at time T deterministically

### Should Have

- Easing selection per segment.
- Copy/paste segments.

### Won't Have (This Iteration)

- Advanced curve editor (Bezier) unless required.

## Constraints

- **Performance**: evaluation at time T must be fast for interactive scrubbing.
- **Consistency**: same time T yields same scene state.
- **Undo/Redo**: timeline edits must integrate with history.

## Open Questions

- [ ] Segment model vs keyframes vs hybrid?
- [ ] How to combine multiple segments of same type (overlap rules)?
- [ ] Global timeline units: seconds vs frames?

## References

- Legacy anim base + types: `legacy/legacy-comics-editor-csharp/Comics.Editor/Models/Anim.cs` and `*Anim.cs`
- Legacy UI controls: `legacy/legacy-comics-editor-csharp/Comics.Editor/Controls/*AnimControl.xaml`

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
