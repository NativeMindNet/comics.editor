# Implementation Plan: animation timeline (segments, evaluation, editing)

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Specifications: [link to 03-specifications.md]

## Summary

Implement an animation segment model and evaluator, then build a basic timeline UI to create/move/resize segments with immediate preview and undo/redo integration.

## Task Breakdown

### Phase 1: Foundation

#### Task 1.1: Define segment model + easing set
- **Description**: Lock core fields (id, target, type, start/end, params, easing).
- **Files**:
  - `lib/...` - Create/Modify
  - `test/...` - Create/Modify
- **Dependencies**: None
- **Verification**: Serialization round-trip tests pass
- **Complexity**: Medium

#### Task 1.2: Implement evaluator at time T
- **Description**: Compute evaluated layer state from segments.
- **Files**:
  - `lib/...` - Create/Modify
  - `test/...` - Create/Modify
- **Dependencies**: Task 1.1
- **Verification**: Unit tests cover boundaries and interpolation
- **Complexity**: High

### Phase 2: UI + Editing

#### Task 2.1: Build timeline UI skeleton (tracks + playhead)
- **Description**: Render tracks for selected targets and show segments + playhead.
- **Files**:
  - `lib/...` - Create/Modify
- **Dependencies**: Task 1.1
- **Verification**: Manual: segments display correctly
- **Complexity**: High

#### Task 2.2: Segment editing (create/move/resize) + inspector
- **Description**: Interaction + inspector editing for params and easing.
- **Files**:
  - `lib/...` - Create/Modify
- **Dependencies**: Task 2.1, Task 1.2
- **Verification**: Edits immediately affect preview at playhead
- **Complexity**: High

### Phase 3: Integration

#### Task 3.1: Wire evaluation into renderer + audio
- **Description**: Renderer consumes evaluated transforms; audio consumes SoundAnim segments.
- **Files**:
  - `lib/...` - Modify
- **Dependencies**: `vdd-legacy-rendering`, `vdd-legacy-audio`
- **Verification**: Play timeline and see/hear expected results
- **Complexity**: High

## Dependency Graph

```
Task 1.1 -> Task 1.2 -> Task 2.2 -> Task 3.1
Task 1.1 -> Task 2.1 ----^
```

## Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| Timeline UI complexity | Med | High | iterate with minimal viable interactions first |
| Overlap semantics ambiguity | Med | Med | lock a policy early; enforce in UI |

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
