# Requirements: Balloon Editor v2

> Version: 2.0
> Status: DRAFT
> Last Updated: 2026-05-10

## Problem Statement

The current comics.editor model is layer, sound, and scroll-animation oriented. That is enough for moving images, but it does not provide a first-class system for dialogue objects.

Authors need a dedicated Balloon Editor that treats speech, thought, narration, emotional typography, kinetic dialogue, and speaker tails as semantic, timeline-aware, reusable scene objects. The editor must support static comics, motion comics, webtoon, manga, visual-novel style dialogue, and interactive or branching scenes without forcing artists to bake text into image layers.

## Product Vision

Balloon Editor v2 is a:

- semantic dialogue layout system
- timeline-aware animated typography engine
- visual emotional communication layer
- cinematic dialogue composition tool
- reusable scene-driven component system

It should feel closer to a focused mix of Figma, After Effects, Spine, motion comics tools, and modern visual novel editors while remaining approachable for non-technical artists.

## Core Principles

### Direct Manipulation

Everything important is editable directly on the canvas:

- drag balloon
- resize balloon
- reshape balloon
- drag tail
- edit text inline
- animate by handles
- scrub timeline visually

Hidden modal-heavy UX is out of scope for the primary workflow.

### Layered Complexity

Beginner workflow:

- add balloon
- type text
- choose preset

Advanced workflow:

- bezier shape editing
- animation curves
- procedural effects
- text timing
- emotion states
- runtime variables

### Animation First

Animation is not an optional plugin. Every balloon entity is timeline-aware. Even static balloons support appear, disappear, emphasis, idle motion, and transitions in the same model.

### Mobile and Tablet Friendly

Handles and gesture targets must be touch optimized.

- Minimum interactive area: 44 x 44 logical pixels.
- Required gestures: pinch zoom, rotate, two-finger pan, long press.
- Optional stylus pressure may drive procedural pressure or deformation.

## User Stories

### Primary

**As a** comics author
**I want** to add and edit balloons directly on the canvas
**So that** dialogue composition stays visual and fast.

**As a** motion comics author
**I want** balloon, text, tail, and FX animation on a timeline
**So that** dialogue can be staged cinematically.

**As a** localization editor
**I want** text layout, overflow checks, and alternate writing modes per balloon
**So that** translated dialogue remains readable.

**As a** style lead
**I want** reusable balloon presets and emotional styles
**So that** projects keep a consistent visual language.

### Secondary

- **As a** manga/webtoon creator
  **I want** vertical text, right-to-left flow, and long-scroll placement support
  **So that** the editor supports my publishing format.

- **As a** sound designer
  **I want** balloons to sync with voice, phonemes, and beats
  **So that** speech and visual timing feel intentional.

- **As a** runtime developer
  **I want** deterministic serialization and playback
  **So that** comics.engine can render the same result outside the editor.

## Acceptance Criteria

### Must Have

1. **Given** an open comics scene
   **When** the author chooses the Balloon tool
   **Then** the editor can create speech, thought, scream, whisper, narration, radio/phone, AI/system, and custom balloon objects.

2. **Given** a selected balloon
   **When** the author edits it on canvas
   **Then** resize handles, shape handles, text bounds, rotation handle, and tail anchor remain visible and hit-testable.

3. **Given** a balloon with text
   **When** the author double-clicks it
   **Then** text can be edited inline with rich text spans and immediate layout feedback.

4. **Given** a tail-enabled balloon
   **When** the author drags tail controls
   **Then** the editor supports single-point, bezier, multi-segment, and procedural tails with optional speaker snap.

5. **Given** a balloon object
   **When** the author opens the timeline
   **Then** the balloon exposes transform, shape, tail, text, FX, and audio-sync tracks.

6. **Given** a preset
   **When** it is applied
   **Then** appearance, typography, animation, emotion, and FX settings are applied as a reusable style without destroying editable object data.

7. **Given** text that overflows
   **When** the author previews target devices or locales
   **Then** the editor reports overflow, dense text, missing glyphs, and contrast issues with actionable fixes.

8. **Given** saved balloon data
   **When** the document is reopened or exported to comics.engine
   **Then** playback is deterministic and schema-compatible.

### Should Have

- Procedural shape modifiers: noise, spikes, wobble, inflate, taper.
- Emotional typography presets: fear, rage, sadness, AI, whisper.
- Per-letter animation: typewriter, bounce, wave, shake, stagger, karaoke sync.
- FX stack: border, shadow, distortion, particles, lighting, post FX.
- Audio sync with voice amplitude, phonemes, soundtrack beats, and subtitle timing.
- Auto tail attach to character mouth or speaker target.
- Collision avoidance and reading-flow assistance.
- Keyboard navigation, high contrast mode, dyslexia-friendly fonts, screen reader metadata, and subtitle export.

### Won't Have This Iteration

- Full vector illustration suite for drawing final character art.
- AI-generated final dialogue or story writing as a required feature.
- Collaborative multi-user editing.
- Full cinematic camera authoring beyond balloon integration points.

## Performance Targets

| Target | Requirement |
|--------|-------------|
| Desktop editor viewport | 120 FPS target |
| Mobile/tablet editing | 60 FPS target |
| Web target | WebGL accelerated rendering |

Required optimizations:

- geometry caching
- GPU text rendering
- partial redraw
- timeline virtualization
- lazy FX evaluation

## Constraints

- Must integrate with existing comics.editor document, rendering, timeline, audio, undo/redo, and localization flows.
- Must support desktop, tablet, mobile, and web as product targets.
- Must preserve legacy document loading; balloon data is additive.
- Must serialize in a form comics.engine can replay deterministically.
- Must keep touch handles at least 44 x 44 logical pixels.

## Open Questions

- [ ] Should runtime rendering use vector primitives, cached geometry, signed-distance fields, rasterized textures, or a hybrid?
- [ ] Which rich-text subset is required for the first runtime-compatible release?
- [ ] How should variable-driven dialogue and branching choices bind into balloon text?
- [ ] Which audio-sync format should be canonical: amplitude envelope, phoneme timeline, subtitle timing, or external analysis asset?
- [ ] Which parts of smart placement require AI assistance versus deterministic heuristics?

## References

- [Visual mockups](./02-visual.md)
- [Specifications](./03-specifications.md)
- [Implementation plan](./04-plan.md)
- `flows/vdd-comics.editor-semantic-reading-editor/` - semantic reading integration.
- `flows/vdd-comics.editor-rendering/` - canvas rendering, hit testing, transforms, handles.
- `flows/vdd-comics.editor-animation-timeline/` - shared timeline and segment evaluation.
- `flows/vdd-comics.editor-audio/` - sound preview, waveform, scrubbing.
- `flows/vdd-comics.editor-format/` - schema and bundle storage.

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
