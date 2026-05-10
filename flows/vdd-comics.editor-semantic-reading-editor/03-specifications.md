# Specifications: semantic reading editor

> Version: 1.0
> Status: DRAFT
> Last Updated: 2026-05-10
> Requirements: [01-requirements.md](./01-requirements.md)
> Visual: [02-visual.md](./02-visual.md)

## Overview

Define a semantic authoring layer above the existing layer, animation, sound, culture, and preview systems. The editor should represent reading intent directly: panels, balloons, reading nodes, localized text variants, sound cues, and scroll animation should be visible in one stage and one timeline.

## Affected Systems

| System | Impact | Notes |
|--------|--------|-------|
| Document model | Extend | Add semantic objects without breaking existing bundles |
| Scene/stage editor | Extend | Draw and edit panel regions, balloon regions, reading badges, speaker links |
| Timeline | Extend | Add track groups for panels, balloons, camera, sound, and layer anims |
| Inspector | Extend | Add semantic tabs: Text, Shape, Tail, Motion, Locale, Sound |
| Preview | Extend | Add reader device profiles and reduced-motion preview |
| Localization | Extend | Add per-balloon text variants and readability validation |
| Audio | Extend | Link sound/voice cues to reading nodes and balloons |
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
               +--> BalloonObject[]
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
    public List<BalloonObject> Balloons { get; set; }
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

public sealed class BalloonObject
{
    public string Id { get; set; }
    public string Name { get; set; }          // "Б3 Реплика злодея"
    public string PanelId { get; set; }
    public BalloonKind Kind { get; set; }     // Speech, Thought, Shout, Whisper, Narration, OffPanel
    public BalloonShape Shape { get; set; }
    public TailPath Tail { get; set; }
    public string SpeakerLayerId { get; set; }
    public List<LocaleTextVariant> Texts { get; set; }
    public BalloonStyle Style { get; set; }
}

public sealed class ReadingNode
{
    public string Id { get; set; }
    public int Order { get; set; }
    public ReadingTargetType TargetType { get; set; } // Panel, Balloon, Sound, Camera
    public string TargetId { get; set; }
    public int EnterScroll { get; set; }
    public int ExitScroll { get; set; }
    public string EnterMotionPreset { get; set; }     // "проявить", "хлопок", "печатать"
    public string ExitMotionPreset { get; set; }
}

public sealed class LocaleTextVariant
{
    public Cultures Culture { get; set; }
    public string Text { get; set; }
    public string FontFamily { get; set; }
    public float AutoFitScale { get; set; }
    public TextFitStatus FitStatus { get; set; }
}
```

## Behavior Specifications

### Stage Editing

- Stage draws existing artwork layers and semantic overlays.
- Semantic overlays have authoring handles and can be hidden in clean preview.
- Panel regions use rectangular handles first. Polygon/custom panel shapes may be later.
- Balloons use bounding handles, shape control points, text box handles, and tail handles.
- Reading order badges appear only in Reading Path Mode or when enabled.

### Reading Path

- A reading node targets a panel, balloon, camera focus, sound cue, or grouped event.
- Nodes have stable order numbers.
- Reordering updates badges and timeline row positions.
- Enter/Exit scroll values can be edited directly or derived from the timeline.
- Reader preview can step through nodes independent from raw scroll.

### Balloon Editing

- Balloon text and shape are edited as one semantic object.
- Supported initial kinds: `Речь`, `Мысль`, `Крик`, `Шепот`, `Закадр`, `Наррация`, `SFX`.
- Tail modes: straight, polyline, spline, off-panel.
- Tail can optionally target a speaker layer.
- Style is reusable and may define fill, stroke, font, padding, mood, and default motion.

### Localization

- Each balloon stores localized text variants.
- The active culture controls stage text.
- Localization mode can show source and target side by side.
- Validation computes text density, overflow, missing glyph/fallback, and contrast.
- Suggested fixes: auto-fit, resize balloon, split text, create extension balloon, change style.

### Timeline

- Timeline uses grouped tracks:
  - Semantic: panels, balloons, reading nodes, camera.
  - Media: ambience, music, voice, SFX.
  - Technical: layer translate, rotate, scale, alpha.
- Semantic timeline rows do not replace current animation segments. They reference or generate them.
- Sound cues can attach to reading nodes, balloons, or free scroll ranges.
- Waveform display is recommended for voice and SFX tracks.

### Reader Preview

- Device profiles: phone portrait, tablet, desktop, low vision.
- Motion profiles: full, reduced.
- Reading modes: scroll, tap-to-next-node, hybrid.
- Preview checks contrast, text fit, overlap with important art, tail target visibility, missing sound, and missing localization.

## Validation Rules

| Rule | Severity | Example |
|------|----------|---------|
| Missing localized text | Error | В балуне Б3 нет русского текста |
| Text overflow | Warning | "У нас почти не осталось времени!" обрезается |
| Dense text | Warning | Слишком много знаков для телефонного превью |
| Tail target missing | Warning | Хвост указывает на удаленный слой |
| Tail overlaps face | Warning | Хвост пересекает лицо персонажа |
| Unreachable reading node | Error | Узел чтения не связан с панелью или балуном |
| Sound cue missing asset | Error | Не найден файл голосовой реплики |
| Reduced-motion issue | Info | Дрожь балуна стоит упростить в режиме сниженного движения |

## Integration With Existing VDDs

- Rendering VDD owns hit testing, handles, transforms, and stage drawing.
- Animation timeline VDD owns segment evaluation and scroll interpolation.
- Audio VDD owns waveform, scrubbing, sound playback, and sound file management.
- Format VDD owns bundle storage and schema migration.
- This VDD owns the semantic relationships between those systems.

## Edge Cases

| Case | Behavior |
|------|----------|
| Legacy document without semantic scene | Open normally and show "Добавить семантическую разметку" |
| Balloon without panel | Allow but warn: "Балун не привязан к панели" |
| Panel deleted | Offer: "Удалить дочерние балуны" or "Переназначить" |
| Culture removed | Preserve text data until explicit cleanup |
| Reading node overlaps another | Allow if intentional, warn: "Узлы чтения пересекаются" |
| Sound disabled | Still show cues and waveform, preview is muted |

## Open Design Questions

- [ ] Store semantic data as `semanticScene` in `data.json` or as `semantic.json` in bundle?
- [ ] Use Unity IMGUI for first version or move semantic stage tools to UI Toolkit?
- [ ] Should balloon rendering be vector at runtime or rasterized during export?
- [ ] How to preserve compatibility with current runtime engine?

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
