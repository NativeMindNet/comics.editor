# Specifications: animation timeline (segments, evaluation, editing)

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Requirements: [link to 01-requirements.md]

## Overview

Define how animations are represented and evaluated in the new editor, and how timeline UI edits map to model updates. Maintain legacy parity for segment semantics while enabling future evolution.

## Affected Systems

| System | Impact | Notes |
|--------|--------|-------|
| Animation model | Create/Modify | segment representation, easing |
| Timeline evaluator | Create | computes effective state at time T |
| UI | Create/Modify | tracks, segments, inspector |
| Undo/Redo | Modify | timeline edits are transactional |

## Architecture

### Component Diagram

```
[Timeline UI] -> [Anim Editing API] -> [DocumentModel]
                       |
                       v
                [Evaluator @ time T]
                       |
                       v
                  [SceneGraph]
```

### Data Flow

```
time T -> for each layer: gather relevant segments -> resolve overlaps -> compute effective values
```

## Interfaces

### New Interfaces (conceptual)

```cpp
interface AnimEvaluator {
  EvaluatedLayerState evaluate(layerId, time);
}

interface AnimEditor {
  void addSegment(...);
  void moveSegment(id, delta);
  void resizeSegment(id, newStart, newEnd);
  void updateParams(id, params);
}
```

## Data Models

### New Types (conceptual)

```cpp
struct AnimSegment {
  string id;
  string targetId;   // layerId or soundId
  AnimType type;     // translate/rotate/scale/opacity/sound
  double start;
  double end;
  Easing easing;
  Params params;     // from/to, etc.
}
```

## Behavior Specifications

### Segment evaluation

- Active if \(t \in [start, end]\).
- Value computed by interpolating params using easing factor.

### Overlap rules (initial)

- Same-type overlaps on same target are not allowed by default (UI prevents) OR last-defined wins (decision to lock).
- Different types compose (translate * rotate * scale, opacity separate).

### Error Handling

| Error | Cause | Response |
|-------|-------|----------|
| Invalid segment | end < start | prevent in UI; auto-correct or block |
| Unknown easing | schema mismatch | fallback to linear + warning |

## Dependencies

- Rendering engine consumes evaluated transforms (`vdd-legacy-rendering`).
- Undo/redo system for transactional edits (`vdd-legacy-undo-redo`).

## Testing Strategy

### Unit Tests

- [ ] Interpolation correctness per type
- [ ] Boundary conditions at start/end

### Integration Tests

- [ ] Scrub playhead and verify scene matches expected evaluated state

## Open Design Questions

- [ ] Overlap policy and UI constraints.
- [ ] Seconds vs frames timeline units.
- [ ] Migration path to keyframes/curves (future).

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
