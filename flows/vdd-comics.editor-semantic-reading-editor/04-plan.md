# Implementation Plan: semantic reading editor

> Version: 1.0
> Status: DRAFT
> Last Updated: 2026-05-10
> Specifications: [03-specifications.md](./03-specifications.md)

## Milestone 1: Semantic Model Foundation

- [ ] Add `SemanticScene` container to document model.
- [ ] Add `PanelRegion`, `SemanticBalloonRef`, `ReadingNode`, `SemanticCue`.
- [ ] Add load/save compatibility for documents without semantic data.
- [ ] Add migration/default creation path: "Добавить семантическую разметку".
- [ ] Add undo/redo command wrappers for semantic object creation, deletion, and edits.

## Milestone 2: Stage Overlays

- [ ] Draw panel outlines and reading badges over composed preview.
- [ ] Draw balloon reference outlines and reading badges; delegate direct balloon handles to Balloon Editor v2.
- [ ] Add selection and hit testing for semantic objects.
- [ ] Add basic create/edit/delete tools for panels and balloons.
- [ ] Add visibility toggles: clean preview, semantic overlays, reading badges.

## Milestone 3: Balloon Editor Integration

- [ ] Add create/open handoff to `vdd-comics.editor-balloons-editor`.
- [ ] Store semantic references to `BalloonEntity` IDs.
- [ ] Surface balloon bounds, reading label, speaker reference, and validation status in reading mode.
- [ ] Link reading nodes to balloon timeline ranges supplied by Balloon Editor v2.
- [ ] Keep text, shape, tail, typography, animation, FX, and presets out of this flow.

## Milestone 4: Reading Path Mode

- [ ] Add reading node list.
- [ ] Add create/reorder/delete node actions.
- [ ] Show node badges on stage.
- [ ] Add enter/exit scroll values and motion presets.
- [ ] Add "Проверить порядок" validation.

## Milestone 5: Unified Timeline

- [ ] Add grouped tracks: semantic, media, technical.
- [ ] Show panels, balloon references, reading nodes, camera, layers, and sound in one scroll timeline.
- [ ] Add sound/voice cues linked to reading nodes.
- [ ] Add waveform display for voice/SFX where available.
- [ ] Add snap points for panel starts, linked balloon ranges, and sound cues.

## Milestone 6: Localization Mode

- [ ] Display Balloon Editor v2 localization status for linked balloons.
- [ ] Add scene-level filters for unresolved balloon localization warnings.
- [ ] Open Balloon Editor v2 from scene-level localization warnings.
- [ ] Keep text fit metrics, overflow rules, glyph checks, and fix actions in the Balloon Editor VDD.
- [ ] Verify Russian examples and labels in UI.

## Milestone 7: Reader Preview Lab

- [ ] Add device profiles: phone, tablet, desktop, low vision.
- [ ] Add motion profiles: full and reduced.
- [ ] Add reading modes: scroll, tap-to-next-node, hybrid.
- [ ] Add validation panel for scene overlap, unresolved links, missing sound, and linked Balloon Editor status.
- [ ] Add preview stepper for reading node sequence.

## Testing Strategy

- [ ] Unit: semantic model serialization round trip.
- [ ] Unit: reading node ordering and target resolution.
- [ ] Unit: linked balloon validation status aggregation.
- [ ] Unit: validation rules.
- [ ] Integration: legacy document opens without semantic data.
- [ ] Integration: create panel, balloon, reading path, save, reopen.
- [ ] Integration: scrub timeline and verify semantic overlays evaluate with layer/sound animation.
- [ ] Manual: linked Balloon Editor warnings appear in reading context.

## Rollout

1. Ship semantic model and overlays behind an editor toggle.
2. Enable Balloon Editor v2 integration for internal documents.
3. Enable reading path and reader preview for production review.
4. Add localization and validation as publish blockers after adoption.

## Risks

| Risk | Mitigation |
|------|------------|
| Too much UI complexity | Keep modes separate: Scene, Reading, Locale, Preview |
| Runtime format breakage | Store semantic data as optional section and preserve legacy fields |
| Balloon integration contract drift | Treat `vdd-comics.editor-balloons-editor` as the owner and test metadata handoff |
| Linked validation noise | Make scene-level warnings actionable and dismissible |

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
