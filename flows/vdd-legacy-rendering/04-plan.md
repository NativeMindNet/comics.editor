# Implementation Plan: rendering & interaction engine (legacy parity)

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Specifications: [link to 03-specifications.md]

## Summary

Implement a scene graph and renderer that supports transforms, selection/hit-testing, and tile-backed images, with a small tool state machine for interactions.

## Task Breakdown

### Phase 1: Foundation

#### Task 1.1: Define scene graph types + transform math
- **Description**: Create core types and ensure transform composition matches intended semantics.
- **Files**:
  - `lib/...` - Create/Modify
- **Dependencies**: None
- **Verification**: Unit tests for transform composition pass
- **Complexity**: High

#### Task 1.2: Define hit testing rules + z-order policy
- **Description**: Implement deterministic hit testing based on transformed bounds.
- **Files**:
  - `lib/...` - Create/Modify
  - `test/...` - Create/Modify
- **Dependencies**: Task 1.1
- **Verification**: Repeatable hit test results for overlap cases
- **Complexity**: Medium

### Phase 2: Core Implementation

#### Task 2.1: Implement renderer with selection overlay
- **Description**: Draw layers in z-order; draw selection bounds and handles.
- **Files**:
  - `lib/...` - Create/Modify
- **Dependencies**: Task 1.1
- **Verification**: Manual: select layer, see correct overlay alignment
- **Complexity**: High

#### Task 2.2: Integrate tile resolver for image drawables
- **Description**: Render tiled images by requesting visible tiles only.
- **Files**:
  - `lib/...` - Modify
- **Dependencies**: vdd-legacy-image-pipeline (tile resolver/cache)
- **Verification**: Large tiled image pans smoothly
- **Complexity**: High

### Phase 3: Interaction Tools

#### Task 3.1: Select + move tool
- **Description**: Implement pointer events mapping to select/move with commit semantics.
- **Files**:
  - `lib/...` - Create/Modify
- **Dependencies**: Task 1.2, Task 2.1
- **Verification**: Drag moves selected layer; no jitter
- **Complexity**: Medium

#### Task 3.2: Rotate/scale handles (basic)
- **Description**: Add rotate/scale via handles with clear UX rules.
- **Files**:
  - `lib/...` - Create/Modify
- **Dependencies**: Task 3.1
- **Verification**: Handles behave correctly at various zoom levels
- **Complexity**: High

### Phase 4: Testing & Polish

#### Task 4.1: Integration + perf tests
- **Description**: Establish performance budget and verify on representative devices.
- **Files**:
  - `test/...` - Create/Modify
- **Dependencies**: Task 2.2–3.2
- **Verification**: Meets budget under pan/zoom + selection operations
- **Complexity**: Medium

## Dependency Graph

```
Task 1.1 -> Task 1.2 -> Task 2.1 -> Task 3.1 -> Task 3.2 -> Task 4.1
                     \-> Task 2.2 -----^
```

## Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| Render jank on tile-heavy scenes | Med | High | strict tile budgets + caching + incremental draw |
| Handle math complexity | Med | Med | strong unit tests + visual debug overlay |

## Rollback Strategy

1. Start with select+move only; gate rotate/scale behind flag.
2. Keep renderer modular; can replace internals without changing document model.

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
