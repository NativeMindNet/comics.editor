# Implementation Plan: Balloon Editor v2

> Version: 2.0
> Status: DRAFT
> Last Updated: 2026-05-10
> Specifications: [03-specifications.md](./03-specifications.md)

## Phase 1: Core Balloon Authoring

- [ ] Add `BalloonEntity` model as optional document data.
- [ ] Add basic balloon types: speech, thought, scream, whisper, narration.
- [ ] Add canvas create/select/move/resize/delete flow.
- [ ] Add inline text editing and basic auto-wrap.
- [ ] Add tail editing for straight, bezier, and polyline tails.
- [ ] Add preset browser and reusable base styles.
- [ ] Add serialization round trip.
- [ ] Add undo/redo commands for balloon creation, deletion, text, shape, tail, and style edits.

## Phase 2: Timeline and Motion Typography

- [ ] Add balloon timeline group with Transform, Shape, Tail, Text, FX, and Audio Sync tracks.
- [ ] Add enter, idle, emphasis, and exit animation presets.
- [ ] Add per-letter and per-span text animation model.
- [ ] Add typewriter, bounce, wave, shake, stagger, and karaoke sync modes.
- [ ] Add curve editor integration for balloon properties.
- [ ] Add deterministic timeline scrub preview.

## Phase 3: Procedural Shapes and FX

- [ ] Add shape bases: ellipse, rounded rectangle, polygon, procedural.
- [ ] Add modifiers: noise, spikes, wobble, inflate, taper.
- [ ] Add procedural controls: chaos, sharpness, softness, frequency, asymmetry, pressure.
- [ ] Add FX stack: border, shadow, distortion, particles, lighting, post FX.
- [ ] Add geometry caching and lazy FX evaluation.
- [ ] Add runtime compatibility checks for FX.

## Phase 4: Smart Placement, Localization, and Audio Sync

- [ ] Add speaker target model and magnetic tail attach.
- [ ] Add collision and tail-intersection validation.
- [ ] Add reading-flow assistant hooks for LTR, RTL, and vertical webtoon flow.
- [ ] Add per-locale text variants and fit metrics.
- [ ] Add manga vertical text and RTL layout support.
- [ ] Add audio sync modes: amplitude, phoneme, beat, subtitle, manual.
- [ ] Add accessibility checks and reduced-motion substitutions.

## Phase 5: Runtime Export and Engine Integration

- [ ] Define final runtime JSON schema for balloon objects.
- [ ] Add comics.engine export pipeline.
- [ ] Add deterministic playback tests for enter, idle, text, FX, and exit tracks.
- [ ] Add runtime variable injection for dialogue text.
- [ ] Add localization replacement at runtime.
- [ ] Add dynamic resizing behavior for variable text.

## Testing Strategy

- [ ] Unit: balloon serialization round trip.
- [ ] Unit: preset apply preserves object identity and speaker/localization references.
- [ ] Unit: text layout reports overflow, dense text, and missing glyphs.
- [ ] Unit: tail target resolution and missing target validation.
- [ ] Unit: animation state machine transitions.
- [ ] Unit: procedural shape cache invalidation.
- [ ] Integration: create balloon, edit text, edit tail, save, reopen.
- [ ] Integration: scrub timeline and verify shape, text, tail, and FX evaluation.
- [ ] Integration: export to runtime JSON and replay deterministically.
- [ ] Manual: desktop mouse, tablet touch, mobile radial menu.
- [ ] Manual: Russian, English, Hindi, vertical text, and RTL samples.

## Rollout

1. Ship core static balloons behind an editor feature toggle.
2. Enable timeline-aware balloon authoring for internal documents.
3. Enable presets and localization validation for production review.
4. Enable runtime export once comics.engine compatibility tests pass.
5. Add smart placement and audio sync after the core authoring loop is stable.

## Risks

| Risk | Mitigation |
|------|------------|
| UI becomes too complex | Keep beginner workflow one-click preset + inline text; move advanced controls into inspector tabs |
| Runtime/editor rendering diverges | Add shared fixtures and deterministic export/replay tests early |
| Rich text scope explodes | Define a runtime-compatible rich-text subset before implementation |
| Procedural FX hurt performance | Cache geometry, virtualize timeline, and evaluate expensive FX lazily |
| Smart placement overrides artist intent | Make suggestions non-destructive and preserve manual overrides |

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
