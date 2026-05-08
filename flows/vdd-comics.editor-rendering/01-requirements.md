# Requirements: rendering & interaction engine (legacy parity)

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08

## Problem Statement

Legacy editor renders layers on a canvas with transform stacks (translate/rotate/scale/opacity), supports selection, and drives visual state from animation timelines. The Flutter rewrite needs a rendering and interaction model that matches legacy semantics while remaining performant for large, tile-based assets.

## User Stories

### Primary

**As a** creator  
**I want** to select and transform layers on the canvas (move/rotate/scale)  
**So that** I can compose scenes precisely

### Secondary

- **As a** creator  
  **I want** smooth pan/zoom and responsive selection even on huge images  
  **So that** the editor feels fast and reliable

- **As a** developer  
  **I want** deterministic hit testing and transform math  
  **So that** bugs are reproducible and fixable

## Acceptance Criteria

### Must Have

1. **Given** a document with N layers  
   **When** it is rendered  
   **Then** layers draw in correct z-order with correct transforms and opacity

2. **Given** a user taps/clicks on the canvas  
   **When** the pointer intersects a layer’s visual bounds  
   **Then** the correct layer is selected (deterministic hit testing)

3. **Given** the user drags a selected layer  
   **When** the drag completes  
   **Then** the layer’s position updates and the result is visually stable (no jitter)

4. **Given** tile-based images  
   **When** panning/zooming  
   **Then** only visible tiles are decoded/drawn and the UI stays responsive

### Should Have

- Multi-select and group transforms.
- Snapping and alignment guides (optional).

### Won't Have (This Iteration)

- Advanced vector editing (paths/shapes).

## Constraints

- **Performance**: must handle large canvases and multiple layers; avoid full bitmap composition.
- **Platform**: must work on mobile/desktop targets.
- **Correctness**: transform math must match the legacy semantics (including pivot behavior if used).

## Open Questions

- [ ] Rendering approach: retained scene graph vs `CustomPainter` redraw model?
- [ ] Hit testing: bounding-box only vs alpha-aware (per-pixel) for irregular shapes?
- [ ] How to represent pivot/anchor consistently with legacy (`PivotAnim` exists in legacy codebase)?
- [ ] Zoom model: free zoom vs discrete levels matching pyramid scales?

## References

- Legacy compositor: `legacy/legacy-comics-editor-csharp/Comics.Editor/Controls/LayersControl.xaml`
- Legacy canvas hosts: `ComicsControl.xaml`, `PuzzleControl.xaml`
- Legacy anim types: `legacy/legacy-comics-editor-csharp/Comics.Editor/Models/*Anim.cs`

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
