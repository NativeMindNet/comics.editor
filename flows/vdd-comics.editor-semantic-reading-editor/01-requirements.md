# Requirements: semantic reading editor

> Version: 1.0
> Status: DRAFT
> Last Updated: 2026-05-10

## Problem Statement

The current editor is layer and segment oriented: authors add image layers, sound clips, and scroll-driven animation ranges. This is technically useful, but it does not model the reader's experience directly.

Authors need a semantic reading editor where panels, balloons, reading order, localization, sound, and scroll animation are edited as one scene. The product should help creators answer: where does the reader look, what do they read, who speaks, when does it appear, how does it move, how does it sound, and whether it still works after translation.

## User Stories

### Primary

**As a** comics author
**I want** to define panels, balloons, and reading order on the scene
**So that** the editor matches how the reader experiences the comic.

**As a** motion comics author
**I want** scroll animation, camera focus, balloon reveals, and sound cues on one timeline
**So that** I can stage the reading flow without switching mental models.

**As a** localization editor
**I want** to compare Russian, English, Hindi, and future localized text in the same balloon
**So that** translated text remains readable and does not break layout.

### Secondary

- **As a** reviewer
  **I want** a reader preview for phone, tablet, desktop, and reduced motion
  **So that** I can validate the actual reading experience.

- **As a** creator
  **I want** balloon styles, tail targets, and speaker links
  **So that** speech, thought, whisper, shout, narration, and off-panel voice are visually distinct.

- **As a** production lead
  **I want** semantic validation warnings
  **So that** issues like unreadable text, wrong reading order, missing sound, or a tail pointing to nowhere are caught before publish.

## Acceptance Criteria

### Must Have

1. **Given** a document with image layers
   **When** the author enters semantic edit mode
   **Then** the editor can create and display panel regions, balloon objects, and reading nodes over the scene.

2. **Given** a balloon object
   **When** the author edits it
   **Then** the editor exposes text, shape, tail, style, motion, locale, and optional sound link controls.

3. **Given** a reading path
   **When** the author reorders nodes
   **Then** the stage and preview show the updated numbered reading sequence.

4. **Given** localized text variants
   **When** text overflows or becomes too dense
   **Then** the localization mode shows a warning and offers fit, resize, split, or extension options.

5. **Given** scroll-driven animation and sound cues
   **When** the author scrubs the timeline
   **Then** panels, balloons, layers, camera focus, and audio cues evaluate together at the current scroll position.

6. **Given** reader preview mode
   **When** the author switches device profiles
   **Then** the preview shows readability, motion, sound, and reading node checks for that profile.

### Should Have

- Auto-detect imported panel regions and likely balloon regions.
- Suggest reading order from panel/balloon positions.
- Link a balloon to a speaker layer so the tail can track the target.
- Offer reusable voice styles per character.
- Support reduced-motion preview and validation.
- Show waveform tracks for voice and sound effects.

### Won't Have (This Iteration)

- Full vector drawing suite for artwork.
- AI-generated final comic art.
- Video editing beyond scroll-driven scene timing.
- Distribution/store publishing workflow.

## Constraints

- Must remain compatible with existing `.comics` / `.puzzle` document concepts.
- Must work inside Unity Editor first.
- Must preserve existing layer, sound, animation, culture, preview, and undo/redo flows.
- Semantic objects should be additive so legacy documents can still open.
- Russian examples and labels should be supported in visual specs and product UI copy.

## Open Questions

- [ ] Should semantic objects be stored in current `data.json` or a new schema-versioned section?
- [ ] Should panel regions be authoring-only, runtime-visible, or both?
- [ ] Should balloons render as runtime vector objects, pre-rendered textures, or hybrid overlays?
- [ ] Should reading order be scroll-driven, tap-driven, or support both modes?
- [ ] How much AI assistance is allowed in production authoring?

## References

- `flows/vdd-comics.editor-rendering/` - canvas, hit testing, scene interaction.
- `flows/vdd-comics.editor-animation-timeline/` - scroll segments and timeline semantics.
- `flows/vdd-comics.editor-audio/` - sound tracks, scrubbing, waveform concepts.
- `flows/vdd-comics.editor-format/` - document bundle model.
- `flows/sdd-comics.editor-animation-timeline-ui/` - current Unity timeline implementation.

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
