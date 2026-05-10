# Requirements: semantic reading editor

> Version: 1.0
> Status: DRAFT
> Last Updated: 2026-05-10

## Problem Statement

The current editor is layer and segment oriented: authors add image layers, sound clips, and scroll-driven animation ranges. This is technically useful, but it does not model the reader's experience directly.

Authors need a semantic reading editor where panels, balloon references, reading order, sound, and scroll animation are edited as one scene. The product should help creators answer: where does the reader look, what do they read, who speaks, when does it appear, how does it move, how does it sound, and whether it still works after translation.

Balloon authoring itself is specified separately in `flows/vdd-comics.editor-balloons-editor/`. This document only owns reading-order relationships, panel context, preview, and scene-level validation that consumes balloon data.

## User Stories

### Primary

**As a** comics author
**I want** to define panels, balloon references, and reading order on the scene
**So that** the editor matches how the reader experiences the comic.

**As a** motion comics author
**I want** scroll animation, camera focus, linked balloon timing, and sound cues on one timeline
**So that** I can stage the reading flow without switching mental models.

**As a** localization editor
**I want** to see Balloon Editor v2 localization status in reading context
**So that** translated dialogue issues are visible during scene review.

### Secondary

- **As a** reviewer
  **I want** a reader preview for phone, tablet, desktop, and reduced motion
  **So that** I can validate the actual reading experience.

- **As a** creator
  **I want** the reading editor to open and reference Balloon Editor v2 objects
  **So that** balloon creation stays centralized while reading flow can still use them as semantic targets.

- **As a** production lead
  **I want** semantic validation warnings
  **So that** issues like wrong reading order, missing sound, unresolved balloon references, or Balloon Editor validation errors are caught before publish.

## Acceptance Criteria

### Must Have

1. **Given** a document with image layers
   **When** the author enters semantic edit mode
   **Then** the editor can create and display panel regions, balloon references, and reading nodes over the scene.

2. **Given** a balloon reference in the reading scene
   **When** the author opens balloon editing
   **Then** the dedicated Balloon Editor v2 opens with the selected object as defined in `flows/vdd-comics.editor-balloons-editor/`.

3. **Given** a reading path
   **When** the author reorders nodes
   **Then** the stage and preview show the updated numbered reading sequence.

4. **Given** Balloon Editor v2 validation metadata
   **When** a linked balloon has localization or layout warnings
   **Then** the localization mode shows the linked status and opens Balloon Editor v2 for the fix.

5. **Given** scroll-driven animation and sound cues
   **When** the author scrubs the timeline
   **Then** panels, balloons, layers, camera focus, and audio cues evaluate together at the current scroll position.

6. **Given** reader preview mode
   **When** the author switches device profiles
   **Then** the preview shows readability, motion, sound, and reading node checks for that profile.

### Should Have

- Auto-detect imported panel regions and likely balloon reference positions.
- Suggest reading order from panel/balloon positions.
- Link reading nodes to balloon IDs and speaker references supplied by Balloon Editor v2.
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
- [ ] How should semantic reading consume Balloon Editor v2 runtime output without duplicating rendering rules?
- [ ] Should reading order be scroll-driven, tap-driven, or support both modes?
- [ ] How much AI assistance is allowed in production authoring?

## References

- `flows/vdd-comics.editor-rendering/` - canvas, hit testing, scene interaction.
- `flows/vdd-comics.editor-balloons-editor/` - balloon creation, editing, typography, tails, animation, FX, and runtime export.
- `flows/vdd-comics.editor-animation-timeline/` - scroll segments and timeline semantics.
- `flows/vdd-comics.editor-audio/` - sound tracks, scrubbing, waveform concepts.
- `flows/vdd-comics.editor-format/` - document bundle model.
- `flows/sdd-comics.editor-animation-timeline-ui/` - current Unity timeline implementation.

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
