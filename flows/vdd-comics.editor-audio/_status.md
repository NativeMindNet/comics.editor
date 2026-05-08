# Status: vdd-legacy-audio

## Current Phase

REQUIREMENTS

## Phase Status

DRAFTING

## Last Updated

2026-05-08 by GPT-5.2

## Blockers

- None

## Progress

- [ ] Requirements drafted
- [ ] Requirements approved
- [ ] Visual mockups drafted
- [ ] Visual approved
- [ ] Specifications drafted
- [ ] Specifications approved
- [ ] Plan drafted
- [ ] Plan approved
- [ ] Implementation started
- [ ] Implementation complete
- [ ] Documentation drafted
- [ ] Documentation approved

## Context Notes

Key decisions and context for resuming:

- Legacy uses WPF `MediaPlayer` to preview sounds and uses `SoundAnim` segments (start/end) on timeline.
- Flutter rewrite needs predictable audio preview during scrubbing/panning and policy for mixing multiple sounds.

## Fork History

- N/A

## Next Actions

1. Draft requirements for SoundAnim semantics, preview UX, and mixing policy.
2. Pick audio backend plugin(s) and latency targets.
