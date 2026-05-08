# Specifications: audio subsystem (SoundAnim, preview, mixing)

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Requirements: [link to 01-requirements.md]

## Overview

Specify the runtime audio engine and editing behaviors:
- segment-based playback (`SoundAnim`: start/end)
- preview during play and scrubbing
- mixing policy for overlaps
- platform backend selection and latency goals

## Affected Systems

| System | Impact | Notes |
|--------|--------|-------|
| Audio engine | Create | play/stop/seek + mixing |
| Timeline evaluator | Create/Modify | determines active sounds at time T |
| UI | Create/Modify | sound list + segments + controls |
| Document model | Modify | store sound metadata + anim segments |

## Architecture

### Component Diagram

```
[Timeline] -> [SoundAnim Evaluator] -> [Playback Scheduler] -> [Audio Backend]
                        |
                        v
                [UI state + meters]
```

### Data Flow

```
time T -> active segments -> for each sound: desired state (playing/stopped/loop) -> backend calls
```

## Interfaces

### New Interfaces (conceptual)

```cpp
interface AudioBackend {
  void play(soundId, startOffset);
  void stop(soundId);
  void setVolume(soundId, volume);
  void seek(soundId, position);
}

interface SoundEvaluator {
  list<SoundPlaybackCommand> evaluateAt(time);
}
```

## Behavior Specifications

### Playback semantics

- A `SoundAnim` segment is active for \(t \in [start, end]\).
- On entering a segment:
  - if loop enabled: start sound (or continue) and keep looping until segment ends
  - else: play once; if sound ends before segment end, remain silent
- On leaving a segment: stop sound (or fade out if later added)

### Scrubbing semantics

- When user drags playhead:
  - fast, coarse preview is acceptable (policy in plan)
  - engine should avoid thrashing: debounce updates or limit start/stop frequency

### Mixing policy (initial)

- Allow multiple sounds concurrently.
- Provide master volume; per-sound volume optional.
- Add “solo/mute” later if needed.

### Error Handling

| Error | Cause | Response |
|-------|-------|----------|
| Backend unavailable | no device/permission | disable playback; show error |
| Decode fail | corrupt audio | mark asset invalid; warn |

## Dependencies

- Document format and asset layout (see `vdd-legacy-format`).

## Testing Strategy

### Unit Tests

- [ ] Evaluator returns correct active segments at time T
- [ ] Overlap cases produce expected play/stop commands

### Integration Tests

- [ ] Scrub across segments and confirm audible behavior matches expectations

## Open Design Questions

- [ ] Backend choice and supported formats across platforms.
- [ ] Latency target and acceptable preview degradation during fast scrub.

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
