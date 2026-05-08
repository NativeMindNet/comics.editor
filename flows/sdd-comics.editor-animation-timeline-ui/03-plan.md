# Implementation Plan: Unity animation & layer inspector UI

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Specifications: [02-specifications.md](./02-specifications.md)

## Summary

Implement inspector + timeline incrementally: translate → full anim types → sound → culture image/popup.

## Task Breakdown

#### Task 1.1: Inspector shell for selected layer (fields read-only then editable)
- **Complexity**: Medium

#### Task 1.2: Anim list CRUD + parameter editors per type
- **Complexity**: High

#### Task 1.3: Sound anim list on `SoundModel`
- **Complexity**: Medium

#### Task 1.4: Culture image + popup pickers (reuse `EditorUtility.OpenFilePanel`)
- **Complexity**: Medium

#### Task 1.5: Command wrappers (prepare for undo)
- **Complexity**: Medium

## Dependency Graph

```
Task 1.1 → 1.2 → 1.3
Task 1.1 → 1.4
Tasks 1.2–1.4 → 1.5
```

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
