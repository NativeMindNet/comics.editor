# Status: sdd-unity-audio-preview

## Current Phase

REQUIREMENTS

## Phase Status

DRAFTING

## Last Updated

2026-05-08

## Blockers

- Unity audio API choice (Editor vs clip import) and mp3 support on all editor platforms.

## Progress

- [ ] Requirements drafted
- [ ] Requirements approved
- [ ] Specifications drafted
- [ ] Specifications approved
- [ ] Plan drafted
- [ ] Plan approved
- [ ] Implementation started
- [ ] Implementation complete

## Context Notes

- WPF: `SoundViewModel` + `MediaPlayer`, `DisableSound` toggles behavior on scroll.
- Unity: files copied; no playback loop tied to `Scroll`.

## Next Actions

1. Specify scrub/play UX and debouncing to avoid audio thrash.
2. Map `SoundAnim.FindCurrent` behavior to Unity time driver.
