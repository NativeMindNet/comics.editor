# Requirements: Unity canvas preview with composed transforms

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08

## Problem Statement

The Unity editor currently shows **per-layer previews** without applying **rotate/scale/pivot/alpha** in a single shared “stage” matching WPF. Authors cannot trust WYSIWYG for layout while scrubbing `Scroll`.

## User Stories

**As a** author  
**I want** one preview canvas that shows all layers composed like the runtime/WPF view  
**So that** scroll and animation timing match expectations  

**As a** author  
**I want** layer **z-order** to match the list order  
**So that** overlaps look correct  

## Acceptance Criteria

### Must Have

1. **Given** multiple layers with non-identity transform at current `Scroll`  
   **When** the preview repaints  
   **Then** layers draw in list order with translate, rotate, scale, and alpha applied per evaluated `Anim` state  

2. **Given** puzzle vs comics canvas size fields  
   **When** viewing preview  
   **Then** viewport uses document `Width`×`Height` (or zoom-to-fit) with letterboxing rules documented  

3. **Given** a tiled texture for a layer  
   **When** composing  
   **Then** use the same effective pixels as current preview path (or better: direct tile draw without full composite texture if perf allows)  

### Should Have

- Optional grid / safe-area overlay.  
- (Later) selection rectangle and hit-testing for layer pick.  

### Won’t Have (This Iteration)

- Full WPF adorners for resize handles inside Unity IMGUI (may move to UIToolkit).

## Constraints

- Target 60 FPS interactive scrub on “typical” documents; degrade gracefully on huge tile counts.
- Editor-only; no runtime player dependency.

## Open Questions

- [ ] Exact pivot semantics: normalized vs pixel pivot in `PivotAnim` for WPF — confirm against `LayersControl`.
- [ ] Match WPF “preview half-res” (`TileScale = 2` in `ImagePathConverter`) or always full-res in editor?

## References

- `Comics.Editor/Controls/LayersControl.xaml`
- `UnityComicsEditor/.../ComicsEditorWindow.cs`, `PreviewTextureBuilder.cs`
- VDD: `flows/vdd-legacy-rendering/`

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
