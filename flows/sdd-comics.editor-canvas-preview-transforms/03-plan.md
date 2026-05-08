# Implementation Plan: Unity canvas preview with composed transforms

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Specifications: [02-specifications.md](./02-specifications.md)

## Summary

Extract evaluation logic from GUI; implement stage draw path; keep list panel for metadata; add optional WPF comparison snapshots.

## Task Breakdown

#### Task 1.1: `LayerFrameEvaluator` + tests
- **Files**: New C# under `Assets/ComicsUnity/Editor/`
- **Dependencies**: None
- **Complexity**: Medium

#### Task 1.2: `StagePreviewHost` drawing in EditorWindow
- **Files**: `ComicsEditorWindow.cs`, new partial class/file
- **Dependencies**: Task 1.1
- **Complexity**: High

#### Task 1.3: Tile-friendly draw path (optional optimization)
- **Dependencies**: Task 1.2
- **Complexity**: High

## Risk Assessment

| Risk | Mitigation |
|------|------------|
| IMGUI rotation awkward | prototype Handles overlay or UIToolkit |

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
