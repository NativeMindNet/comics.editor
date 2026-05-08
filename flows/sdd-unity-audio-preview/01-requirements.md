# Requirements: Unity audio preview (SoundAnim + scroll)

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08

## Problem Statement

Authors cannot **hear** sound timing in the Unity editor. WPF previews mp3 and reacts to `Scroll` via `SoundAnim` segments and optional global mute.

## User Stories

**As a** author  
**I want** to play/stop and scrub while hearing sounds activate per `SoundAnim`  
**So that** I can sync audio with vertical scroll narratives  

**As a** author  
**I want** a “disable sound” toggle like WPF  
**So that** I can work without audio when needed  

## Acceptance Criteria

### Must Have

1. **Given** a document with one or more `SoundModel` entries  
   **When** I press Play (or scrub `Scroll`)  
   **Then** audio follows rules derived from `SoundAnim` segments (enter/exit windows) within documented latency budget  

2. **Given** `DisableSound` is on  
   **When** scrubbing or playing  
   **Then** no audible output and no resource leaks (stop/dispose)  

3. **Given** mp3 paths under `sounds/`  
   **When** preview runs in Editor  
   **Then** supported formats behave consistently on Windows/macOS editors (define matrix)  

### Should Have

- Per-sound volume UI parity if WPF exposes it (confirm legacy).  

### Won’t Have (This Iteration)

- Mixer export / WAV bake.

## Constraints

- Editor-only preview is acceptable; runtime player separate.
- Avoid blocking UI thread on decode.

## Open Questions

- [ ] Import mp3 as UnityAudioClip vs stream from disk via native plugin?
- [ ] Exact parity for `SoundAnim.FindCurrent` edge case with `Start==End`?

## References

- `Comics.Editor/ViewModel/SoundViewModel.cs`, `Models/SoundAnim.cs`
- VDD: `flows/vdd-legacy-audio/`

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
