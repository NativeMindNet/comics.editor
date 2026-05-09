# Requirements: Unity audio preview (SoundAnim + scroll)

> Version: 2.0
> Status: DRAFT
> Last Updated: 2026-05-08

## Problem Statement

Authors cannot **hear** sound timing in the Unity editor. WPF previews mp3 and reacts to `Scroll` via `SoundAnim` segments and optional global mute.

## User Stories

**As a** author
**I want** to hear sounds activate as I change scroll position
**So that** I can sync audio with vertical scroll narratives

**As a** author
**I want** a "disable sound" toggle
**So that** I can work without audio when needed

## Acceptance Criteria

### Must Have

1. **Given** a document with `SoundModel` entries and `SoundAnim` segments
   **When** scroll position enters a segment range (Start <= scroll <= End)
   **Then** the associated sound plays (once if Start==End, loop if range)

2. **Given** scroll position exits a looping segment range
   **When** the segment was looping
   **Then** playback stops

3. **Given** `DisableSound` toggle is ON
   **When** scrolling or any preview activity
   **Then** no audible output, no resource leaks

4. **Given** mp3 files under `sounds/` in temp workspace
   **When** preview runs in Editor
   **Then** playback works on Windows and macOS

### Should Have

- Volume slider per sound (if WPF has it - appears not to)

### Won't Have (This Iteration)

- Mixer export / WAV bake
- Runtime player audio (separate system)

## Design Decisions

### Audio API: Unity AudioSource + AudioClip

**Rationale:**
- `AudioClip` supports mp3 import in Editor via `UnityWebRequestMultimedia`
- No native plugins required
- Works on Windows/macOS editors
- `AudioSource.PlayOneShot()` for single plays, `AudioSource.Play()` + `loop=true` for ranges

### SoundAnim Behavior (from WPF)

```
FindCurrent(anims, prevScroll, scroll):
  - If Start <= scroll <= End: trigger (looping if Start != End)
  - If Start == End AND prevScroll < scroll AND prevScroll <= Start <= scroll: trigger once
  - When scroll leaves range: stop looping sounds
```

### Debounce Strategy

- Rapid scroll changes should not thrash audio
- Use ~50ms debounce before triggering sounds
- Stop immediately when leaving range (no debounce on stop)

## Constraints

- Editor-only preview (runtime uses `SoundManager` from comics.engine)
- Avoid blocking UI thread on audio decode
- Must dispose audio resources on window close

## References

- `Comics.Editor/ViewModel/SoundViewModel.cs` - WPF MediaPlayer implementation
- `ComicsUnity/Editor/Models/SoundAnim.cs` - FindCurrent logic

---

## Approval

- [x] Reviewed by: Anton
- [x] Approved on: 2026-05-08
