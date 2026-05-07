# Implementation Plan: audio subsystem (SoundAnim, preview, mixing)

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Specifications: [link to 03-specifications.md]

## Summary

Implement audio playback with segment evaluation and responsive preview. Start with a minimal, reliable backend integration and expand to better scrubbing/mixing UX.

## Task Breakdown

### Phase 1: Foundation

#### Task 1.1: Choose audio backend + formats
- **Description**: Decide Flutter plugin/backend and supported formats (mp3/aac/wav).
- **Files**:
  - `flows/vdd-legacy-audio/03-specifications.md` - Modify
- **Dependencies**: None
- **Verification**: Can play/stop/seek a test asset on all target platforms
- **Complexity**: High

#### Task 1.2: Implement evaluator for SoundAnim segments
- **Description**: Determine active segments at time T and produce commands.
- **Files**:
  - `lib/...` - Create/Modify
  - `test/...` - Create/Modify
- **Dependencies**: Task 1.1
- **Verification**: Unit tests for overlaps and boundaries pass
- **Complexity**: Medium

### Phase 2: Playback scheduler

#### Task 2.1: Implement scheduler (play/stop transitions)
- **Description**: Apply evaluator output to backend with debouncing.
- **Files**:
  - `lib/...` - Create/Modify
- **Dependencies**: Task 1.2
- **Verification**: No audible thrash while scrubbing; correct transitions
- **Complexity**: High

### Phase 3: UI integration

#### Task 3.1: Sounds panel + basic controls
- **Description**: Add/remove sound, set loop/volume, see segments.
- **Files**:
  - `lib/...` - Create/Modify
- **Dependencies**: Task 2.1
- **Verification**: Manual: add sound, set segment, preview works
- **Complexity**: Medium

## Dependency Graph

```
Task 1.1 -> Task 1.2 -> Task 2.1 -> Task 3.1
```

## Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| Backend inconsistencies across platforms | Med | High | keep feature set minimal; add platform shims |
| Scrub preview jitter | Med | Med | debouncing + rate limiting |

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
