# Requirements: audio subsystem (SoundAnim, preview, mixing)

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08

## Problem Statement

The editor needs to attach sounds to a document and preview them accurately on the timeline/canvas. Legacy has basic playback controls and `SoundAnim` segments, but the Flutter rewrite must define clear semantics for preview, scrubbing, loop behavior, and multiple overlapping sounds.

## User Stories

### Primary

**As a** creator  
**I want** to add sounds and preview them in context  
**So that** timing matches the scene and animations

### Secondary

- **As a** creator  
  **I want** scrubbing to be responsive with low latency  
  **So that** I can place sounds precisely

- **As a** creator  
  **I want** predictable behavior when multiple sounds overlap  
  **So that** playback is not chaotic

## Acceptance Criteria

### Must Have

1. **Given** a sound file attached to a document  
   **When** I press Play  
   **Then** it plays from the current timeline position with correct start/stop based on `SoundAnim`

2. **Given** a `SoundAnim` segment (start/end)  
   **When** playback enters and exits the segment  
   **Then** the sound starts/stops according to the defined policy (including loop if enabled)

3. **Given** I scrub the timeline  
   **When** I pass through a sound segment  
   **Then** preview feedback is fast enough to place the sound precisely

### Should Have

- Volume controls per sound and master.
- Mute/solo per sound track.

### Won't Have (This Iteration)

- Advanced audio effects (reverb/EQ).
- Multi-channel export/mixdown.

## Constraints

- **Latency**: interactive preview should feel immediate (target defined in specs).
- **Platform**: must work on iOS/Android/desktop.
- **Consistency**: same document = same playback results.

## Open Questions

- [ ] Mixing policy: allow overlap freely, or limit concurrent sounds?
- [ ] Loop semantics for `SoundAnim`: loop whole file, or loop a segment?
- [ ] What happens when scrubbing jumps into the middle of a segment?
- [ ] Preferred audio backend and supported formats (mp3/aac/wav)?

## References

- Legacy sound VM: `legacy/legacy-comics-editor-csharp/Comics.Editor/ViewModel/SoundViewModel.cs`
- Legacy sound model: `legacy/legacy-comics-editor-csharp/Comics.Editor/Models/Sound.cs`
- Legacy sound anim: `legacy/legacy-comics-editor-csharp/Comics.Editor/Models/SoundAnim.cs`

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
