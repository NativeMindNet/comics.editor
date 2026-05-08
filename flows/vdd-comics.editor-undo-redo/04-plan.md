# Implementation Plan: undo/redo + transactional editing

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Specifications: [link to 03-specifications.md]

## Summary

Add a history controller with transactions and a set of undoable actions. Integrate it with tools (move/rotate/scale, timeline edits) and then extend to asset-aware operations.

## Task Breakdown

### Phase 1: Foundation

#### Task 1.1: Implement history controller + transaction API
- **Description**: Undo/redo stacks, begin/add/commit/rollback, action labels.
- **Files**:
  - `lib/...` - Create/Modify
  - `test/...` - Create/Modify
- **Dependencies**: None
- **Verification**: Unit tests cover basic undo/redo and branching behavior
- **Complexity**: Medium

#### Task 1.2: Define action types for model edits
- **Description**: Move/rotate/scale, add/remove layer, edit anim segment, etc.
- **Files**:
  - `lib/...` - Create/Modify
- **Dependencies**: Task 1.1
- **Verification**: Undo returns exact previous model state
- **Complexity**: High

### Phase 2: Tool Integration

#### Task 2.1: Integrate select+move tool transactions
- **Description**: Drag groups into one transaction; coalesces intermediate updates.
- **Files**:
  - `lib/...` - Modify
- **Dependencies**: Task 1.2
- **Verification**: One undo step per drag
- **Complexity**: Medium

#### Task 2.2: Integrate rotate/scale + timeline edits
- **Description**: Similar grouping rules for handles and scrub/slider interactions.
- **Files**:
  - `lib/...` - Modify
- **Dependencies**: Task 2.1
- **Verification**: Undo granularity feels natural
- **Complexity**: High

### Phase 3: Asset-aware undo (optional staged)

#### Task 3.1: Make import/delete/retile reversible
- **Description**: Introduce journaling or copy-on-write for assets.
- **Files**:
  - `lib/...` - Modify
- **Dependencies**: workspace/packaging decisions
- **Verification**: Undo restores both references and files
- **Complexity**: High

## Dependency Graph

```
Task 1.1 -> Task 1.2 -> Task 2.1 -> Task 2.2
                       \
                        -> Task 3.1 (after workspace decisions)
```

## Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| Asset undo complexity | High | High | stage it; require atomic workspace ops |
| Memory growth | Med | High | bounded stacks; avoid large snapshots |

## Rollback Strategy

1. Ship undo for model changes first.
2. Keep asset operations non-undoable behind a warning until ready.

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
