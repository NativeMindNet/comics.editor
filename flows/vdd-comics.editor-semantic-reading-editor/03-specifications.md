# Specifications: semantic reading editor

> Version: 1.0
> Status: DRAFT
> Last Updated: 2026-05-10
> Requirements: [01-requirements.md](./01-requirements.md)
> Visual: [02-visual.md](./02-visual.md)

## Overview

Define a semantic authoring layer above the existing layer, animation, sound, culture, and preview systems. The editor should represent reading intent directly: panels, balloon references, reading nodes, sound cues, and scroll animation should be visible in one stage and one timeline.

Balloon internals are owned by `flows/vdd-comics.editor-balloons-editor/`. This VDD references balloon IDs and uses balloon metadata for reading flow, preview, and validation.

## Affected Systems

| System | Impact | Notes |
|--------|--------|-------|
| Document model | Extend | Add semantic objects without breaking existing bundles |
| Scene/stage editor | Extend | Draw and edit panel regions, balloon references, reading badges, speaker links |
| Timeline | Extend | Add track groups for panels, balloon references, camera, sound, and layer anims |
| Inspector | Extend | Add semantic tabs for reading node, panel, speaker, preview, and links; balloon internals open Balloon Editor v2 |
| Preview | Extend | Add reader device profiles and reduced-motion preview |
| Localization | Extend | Consume balloon locale/fit metadata from Balloon Editor v2 for scene-level validation |
| Audio | Extend | Link sound/voice cues to reading nodes and balloon IDs |
| Validation | Create | Semantic checks before publish |

## Architecture

```text
[DocumentModel]
      |
      +--> [LayerModel + Anim + SoundModel]              existing
      |
      +--> [SemanticScene]
               |
               +--> PanelRegion[]
               +--> BalloonRef[]
               +--> ReadingNode[]
               +--> LocaleVariant[]
               +--> SemanticCue[]
                         |
                         v
                [Semantic Evaluator @ scroll]
                         |
                         v
              [Stage Renderer + Reader Preview + Timeline]
```

## Conceptual Data Models

```csharp
public sealed class SemanticScene
{
    public List<PanelRegion> Panels { get; set; }
    public List<SemanticBalloonRef> Balloons { get; set; }
    public List<ReadingNode> ReadingPath { get; set; }
    public List<SemanticCue> Cues { get; set; }
}

public sealed class PanelRegion
{
    public string Id { get; set; }
    public string Name { get; set; }          // "П02 Встреча"
    public Rect Bounds { get; set; }
    public int StartScroll { get; set; }
    public int EndScroll { get; set; }
}

public sealed class SemanticBalloonRef
{
    public string Id { get; set; }
    public string Name { get; set; }          // "B3 Villain line"
    public string PanelId { get; set; }
    public string BalloonEntityId { get; set; } // Entity defined by vdd-comics.editor-balloons-editor
    public string SpeakerLayerId { get; set; }
    public Rect ReadingBounds { get; set; }
}

public sealed class ReadingNode
{
    public string Id { get; set; }
    public int Order { get; set; }
    public ReadingTargetType TargetType { get; set; } // Panel, Balloon, Sound, Camera
    public string TargetId { get; set; }
    public int EnterScroll { get; set; }
    public int ExitScroll { get; set; }
    public string EnterMotionPreset { get; set; }     // semantic preset id
    public string ExitMotionPreset { get; set; }
}

// Balloon text, shape, tail, animation, locale variants, and FX are defined by:
// flows/vdd-comics.editor-balloons-editor/
```

## Behavior Specifications

### Stage Editing

- Stage draws existing artwork layers and semantic overlays.
- Semantic overlays have authoring handles and can be hidden in clean preview.
- Panel regions use rectangular handles first. Polygon/custom panel shapes may be later.
- Balloon references can be selected from reading mode; direct balloon handles and editing behavior are defined in `flows/vdd-comics.editor-balloons-editor/`.
- Reading order badges appear only in Reading Path Mode or when enabled.

### Reading Path

- A reading node targets a panel, balloon, camera focus, sound cue, or grouped event.
- Nodes have stable order numbers.
- Reordering updates badges and timeline row positions.
- Enter/Exit scroll values can be edited directly or derived from the timeline.
- Reader preview can step through nodes independent from raw scroll.

### Balloon Integration

- Reading mode stores references to balloon entities owned by Balloon Editor v2.
- Opening or creating a balloon delegates to `flows/vdd-comics.editor-balloons-editor/`.
- The reading editor can use balloon bounds, speaker references, timeline ranges, localization status, and validation metadata.
- Balloon text, shape, tail, typography, animation, FX, presets, and runtime export are not specified here.

### Localization

- Balloon localization variants are defined by Balloon Editor v2.
- The active culture controls stage text.
- Localization mode can show source and target side by side.
- Scene-level validation consumes text density, overflow, missing glyph/fallback, and contrast status from Balloon Editor v2.
- Suggested fixes: auto-fit, resize balloon, split text, create extension balloon, change style.

### Timeline

- Timeline uses grouped tracks:
  - Semantic: panels, balloon references, reading nodes, camera.
  - Media: ambience, music, voice, SFX.
  - Technical: layer translate, rotate, scale, alpha.
- Semantic timeline rows do not replace current animation segments. They reference or generate them.
- Sound cues can attach to reading nodes, balloon IDs, or free scroll ranges.
- Waveform display is recommended for voice and SFX tracks.

### Reader Preview

- Device profiles: phone portrait, tablet, desktop, low vision.
- Motion profiles: full, reduced.
- Reading modes: scroll, tap-to-next-node, hybrid.
- Preview checks reading order, unresolved links, missing sound, overlap with important art, and validation status received from Balloon Editor v2.

## Validation Rules

| Rule | Severity | Example |
|------|----------|---------|
| Balloon validation issue | Warning/Error from Balloon Editor v2 | Б3 требует правки в Balloon Editor |
| Balloon overlaps scene-critical region | Warning | Б3 закрывает важную область панели |
| Unreachable reading node | Error | Узел чтения не связан с панелью или балуном |
| Sound cue missing asset | Error | Не найден файл голосовой реплики |
| Reduced-motion issue | Info | Связанный балун требует reduced-motion вариант |

## Integration With Existing VDDs

- Balloon Editor VDD owns balloon text, shape, tail, typography, animation, FX, presets, and runtime export.
- Rendering VDD owns shared hit testing, handles, transforms, and stage drawing.
- Animation timeline VDD owns segment evaluation and scroll interpolation.
- Audio VDD owns waveform, scrubbing, sound playback, and sound file management.
- Format VDD owns bundle storage and schema migration.
- This VDD owns the semantic relationships between those systems.

## Edge Cases

| Case | Behavior |
|------|----------|
| Legacy document without semantic scene | Open normally and show "Добавить семантическую разметку" |
| Balloon reference without panel | Allow but warn: "Балун не привязан к панели" |
| Panel deleted | Offer: "Удалить дочерние балуны" or "Переназначить" |
| Culture removed | Preserve semantic references and defer balloon text cleanup to Balloon Editor v2 |
| Reading node overlaps another | Allow if intentional, warn: "Узлы чтения пересекаются" |
| Sound disabled | Still show cues and waveform, preview is muted |

## Open Design Questions

- [ ] Store semantic data as `semanticScene` in `data.json` or as `semantic.json` in bundle?
- [ ] Use Unity IMGUI for first version or move semantic stage tools to UI Toolkit?
- [ ] Which Balloon Editor v2 metadata is required for scene-level reading validation?
- [ ] How to preserve compatibility with current runtime engine?

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
