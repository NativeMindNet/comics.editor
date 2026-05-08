# Implementation Plan: Unity asset pipeline fidelity

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Specifications: [02-specifications.md](./02-specifications.md)

## Summary

Add validation layer + tests; align tile/resize math with legacy where needed; implement Convert UX.

## Task Breakdown

#### Task 1.1: Fixture archives + comparison harness
- **Files**: `UnityComicsEditor/Assets/.../Tests/` or `Tests/Editor/` (TBD by Unity test setup)
- **Complexity**: Medium

#### Task 1.2: Document delta matrix (ImageMagick vs Unity) in repo doc comment or `flows/` appendix
- **Complexity**: Low

#### Task 1.3: Implement Convert-equivalent command in Editor window
- **Files**: `ComicsEditorWindow.cs`, new helper
- **Complexity**: Medium

#### Task 1.4: Pre-save validator (missing tiles, orphan sounds)
- **Complexity**: Medium

## Dependency Graph

```
Task 1.1 → Task 1.4 → Task 1.3
Task 1.2 (parallel)
```

## Rollback

Feature-flag Convert and strict validation if blocking users.

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
