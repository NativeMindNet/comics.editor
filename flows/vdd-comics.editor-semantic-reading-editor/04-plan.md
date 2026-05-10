# Implementation Plan: semantic reading editor

> Version: 1.0
> Status: DRAFT
> Last Updated: 2026-05-10
> Specifications: [03-specifications.md](./03-specifications.md)

## Milestone 1: Semantic Model Foundation

- [ ] Add `SemanticScene` container to document model.
- [ ] Add `PanelRegion`, `BalloonObject`, `ReadingNode`, `LocaleTextVariant`, `SemanticCue`.
- [ ] Add load/save compatibility for documents without semantic data.
- [ ] Add migration/default creation path: "Добавить семантическую разметку".
- [ ] Add undo/redo command wrappers for semantic object creation, deletion, and edits.

## Milestone 2: Stage Overlays

- [ ] Draw panel outlines and reading badges over composed preview.
- [ ] Draw balloon bounding boxes, text bounds, shape handles, and tail handles.
- [ ] Add selection and hit testing for semantic objects.
- [ ] Add basic create/edit/delete tools for panels and balloons.
- [ ] Add visibility toggles: clean preview, semantic overlays, reading badges.

## Milestone 3: Balloon Studio

- [ ] Implement balloon inspector tabs: Text, Shape, Tail, Motion, Locale, Sound.
- [ ] Support initial balloon kinds: speech, thought, shout, whisper, narration, off-panel.
- [ ] Implement tail modes: straight, polyline, spline, off-panel.
- [ ] Add reusable balloon styles.
- [ ] Link balloon tail to speaker layer.

## Milestone 4: Reading Path Mode

- [ ] Add reading node list.
- [ ] Add create/reorder/delete node actions.
- [ ] Show node badges on stage.
- [ ] Add enter/exit scroll values and motion presets.
- [ ] Add "Проверить порядок" validation.

## Milestone 5: Unified Timeline

- [ ] Add grouped tracks: semantic, media, technical.
- [ ] Show panels, balloons, reading nodes, camera, layers, and sound in one scroll timeline.
- [ ] Add sound/voice cues linked to reading nodes.
- [ ] Add waveform display for voice/SFX where available.
- [ ] Add snap points for panel starts, balloon enters, and sound cues.

## Milestone 6: Localization Mode

- [ ] Add source/target text comparison view.
- [ ] Add text fit metrics for each culture.
- [ ] Add warnings for overflow, dense text, missing glyphs, and missing localized text.
- [ ] Add fix actions: auto-fit, resize, split, extension balloon.
- [ ] Verify Russian examples and labels in UI.

## Milestone 7: Reader Preview Lab

- [ ] Add device profiles: phone, tablet, desktop, low vision.
- [ ] Add motion profiles: full and reduced.
- [ ] Add reading modes: scroll, tap-to-next-node, hybrid.
- [ ] Add validation panel for contrast, overlap, tail target, missing sound, missing localization.
- [ ] Add preview stepper for reading node sequence.

## Testing Strategy

- [ ] Unit: semantic model serialization round trip.
- [ ] Unit: reading node ordering and target resolution.
- [ ] Unit: localization text fit status.
- [ ] Unit: validation rules.
- [ ] Integration: legacy document opens without semantic data.
- [ ] Integration: create panel, balloon, reading path, save, reopen.
- [ ] Integration: scrub timeline and verify semantic overlays evaluate with layer/sound animation.
- [ ] Manual: Russian text examples fit and warnings appear when expected.

## Rollout

1. Ship semantic model and overlays behind an editor toggle.
2. Enable balloon studio for internal documents.
3. Enable reading path and reader preview for production review.
4. Add localization and validation as publish blockers after adoption.

## Risks

| Risk | Mitigation |
|------|------------|
| Too much UI complexity | Keep modes separate: Scene, Reading, Locale, Preview |
| Runtime format breakage | Store semantic data as optional section and preserve legacy fields |
| Balloon rendering divergence | Define authoring preview and runtime rendering tests early |
| Localization fit false positives | Make warnings actionable and dismissible |

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
