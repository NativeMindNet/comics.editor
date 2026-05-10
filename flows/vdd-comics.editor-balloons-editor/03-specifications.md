# Specifications: Balloon Editor v2

> Version: 2.0
> Status: DRAFT
> Last Updated: 2026-05-10
> Requirements: [01-requirements.md](./01-requirements.md)
> Visual: [02-visual.md](./02-visual.md)

## Overview

Define the dedicated Balloon Editor module for comics.editor. This module owns balloon creation, direct manipulation, rich text, shape generation, tails, presets, animation tracks, FX, audio sync, validation, and runtime export.

The semantic reading editor may reference balloons as reading targets, but balloon internals live here.

## Affected Systems

| System | Impact | Notes |
|--------|--------|-------|
| Document model | Extend | Add timeline-aware balloon entities as optional semantic data |
| Stage/canvas | Extend | Direct manipulation handles, inline text editing, tail editing |
| Inspector | Create/extend | Balloon tabs: Shape, Border, Fill, Tail, Typography, Animation, FX, Audio Sync, Runtime |
| Timeline | Extend | Balloon track groups and per-letter text timing |
| Rendering | Extend | Vector/procedural shapes, rich text, FX previews |
| Presets | Create | Reusable style, typography, animation, emotion, and FX presets |
| Runtime export | Create/extend | Deterministic comics.engine payload |
| Localization | Extend | Per-locale text layout, overflow checks, vertical/RTL support |
| Accessibility | Extend | Metadata, high contrast, dyslexia fonts, subtitle export |

## Module Architecture

```text
balloon_editor/
 ├─ canvas/
 ├─ timeline/
 ├─ typography/
 ├─ procedural_shapes/
 ├─ fx/
 ├─ animation/
 ├─ presets/
 ├─ runtime_export/
 └─ ai_assist/
```

### Integration Diagram

```text
[Semantic Reading Editor]
          |
          v
[Balloon Editor] ----> [Rendering VDD]
      |     |          [Timeline VDD]
      |     |          [Audio VDD]
      |     v
      |  [Preset Store]
      v
[Format VDD] ----> [comics.engine Runtime]
```

## Entity Model

```yaml
Balloon:
  id: UUID
  type: speech | thought | scream | whisper | narration | radio_phone | ai_system | emotional | custom
  shape:
    geometry
    border
    fill
    effects
  tail:
    enabled
    style
    points
    target
  text:
    content
    typography
    layout
    animation
  animation:
    enter
    idle
    emphasis
    exit
  timeline:
    start
    duration
    tracks
  metadata:
    character
    emotion
    localization
    accessibility
```

### Conceptual Types

```csharp
public sealed class BalloonEntity
{
    public string Id { get; set; }
    public BalloonType Type { get; set; }
    public BalloonShape Shape { get; set; }
    public BalloonTail Tail { get; set; }
    public BalloonText Text { get; set; }
    public BalloonAnimation Animation { get; set; }
    public BalloonTimeline Timeline { get; set; }
    public BalloonMetadata Metadata { get; set; }
}

public sealed class BalloonShape
{
    public ShapeBase Base { get; set; }           // ellipse, rounded_rect, polygon, procedural
    public List<ShapeModifier> Modifiers { get; set; }
    public ShapeBorder Border { get; set; }
    public ShapeFill Fill { get; set; }
    public List<BalloonEffect> Effects { get; set; }
}

public sealed class BalloonTail
{
    public bool Enabled { get; set; }
    public TailStyle Style { get; set; }          // point, bezier, polyline, procedural
    public List<Vector2> Points { get; set; }
    public TailTarget Target { get; set; }        // speaker layer, mouth point, free point, off-panel
    public bool MagneticAttach { get; set; }
    public bool AutoAvoidance { get; set; }
}

public sealed class BalloonText
{
    public RichTextDocument Content { get; set; }
    public TypographyStyle Typography { get; set; }
    public TextLayoutSettings Layout { get; set; }
    public TextAnimationSettings Animation { get; set; }
    public List<LocalizedBalloonText> Localizations { get; set; }
}

public sealed class BalloonTimeline
{
    public double Start { get; set; }
    public double Duration { get; set; }
    public List<BalloonTrack> Tracks { get; set; }
}
```

## Balloon Types

| Type | Purpose | Default visual |
|------|---------|----------------|
| Speech | Standard dialogue | Rounded balloon |
| Thought | Internal thought | Cloud-like shape and bubble tail |
| Scream | Loud/emphatic speech | Spiky procedural outline |
| Whisper | Quiet speech | Dashed border and soft opacity |
| Narration | Caption or narrator voice | Rectangular cinematic box |
| Radio/Phone | Mediated speech | Electronic waveform border |
| AI/System | UI/system message | HUD panel style |
| Emotional | Emotion-driven distortion | Procedural deformation by emotion |
| Custom | Project-specific | User-defined preset |

## Canvas Interaction Model

### Selection

| Interaction | Behavior |
|-------------|----------|
| Single click/tap | Select balloon |
| Double click/tap | Edit text inline |
| Triple click/tap | Select paragraph |
| Drag body | Move balloon |
| Drag resize handle | Resize bounds |
| Drag shape node | Reshape outline |
| Drag tail anchor | Edit tail |
| Long press | Context/radial menu |

### Handles

- Resize handles, shape handles, text handles, rotation handle, and tail anchor are visible for selected balloons.
- Minimum hit target is 44 x 44 logical pixels.
- Visual handles may be smaller than hit targets, but hit testing must use the larger area.
- On touch devices, selected handles may expand or show a local radial menu.

### Tail Editing

Required modes:

- single-point tail
- bezier tail
- multi-segment tail
- procedural tail

Required features:

- snap to character mouth or speaker target
- magnetic attach
- auto avoidance
- auto smoothing
- off-panel pointer

## Shape System

```yaml
Shape:
  base:
    ellipse
    rounded_rect
    polygon
    procedural

  modifiers:
    noise
    spikes
    wobble
    inflate
    taper

  effects:
    shadow
    glow
    blur
    outline
```

### Procedural Controls

Authors can adjust:

- chaos
- sharpness
- softness
- frequency
- asymmetry
- pressure

Shape geometry should be cacheable and invalidated only when source controls change.

## Text Engine

### Inline Rich Text

Required support:

- bold
- italic
- ruby/furigana
- gradients
- animated spans
- inline icons
- multilingual text
- RTL

### Smart Auto Layout

The layout engine must:

- auto resize balloon when configured
- auto wrap text
- prevent overflow or report it
- balance lines visually
- support manga vertical text
- account for padding, tail intrusions, and shape interior

### Emotional Typography

| Emotion | Behavior |
|---------|----------|
| Fear | jitter |
| Rage | scale pulse |
| Sadness | slow fade |
| AI | digital glitch |
| Whisper | low opacity |

## Animation System

Every balloon owns timeline tracks:

```text
Balloon Track
 ├─ Transform
 ├─ Shape
 ├─ Tail
 ├─ Text
 ├─ FX
 └─ Audio Sync
```

### Enter Animations

Supported presets:

- fade
- scale pop
- elastic
- ink draw
- manga impact
- smoke reveal
- glitch

### Text Animations

Per-letter and per-span modes:

- typewriter
- bounce
- wave
- shake
- stagger
- karaoke sync

### State Machine

```text
Hidden -> Entering -> Visible -> Idle -> Exiting -> Destroyed
```

Static balloons use the same state machine with zero or default animation segments.

## FX System

```text
Balloon
 ├─ Border FX
 ├─ Shadow FX
 ├─ Distortion FX
 ├─ Particle FX
 ├─ Lighting FX
 └─ Post FX
```

Example presets:

- Manga Speed Burst
- Electric
- Divine Aura
- Digital Glitch
- Smoke Reveal

FX evaluation must be lazy and timeline-aware. Expensive FX should degrade in editor preview if performance budgets are exceeded, while preserving deterministic export settings.

## Audio Synchronization

Optional advanced sync modes:

```yaml
sync:
  mode: amplitude | phoneme | beat | subtitle | manual
  source: voice_track_01
```

Balloon sync can drive:

- text reveal timing
- per-letter emphasis
- scale pulse
- border glow
- shape wobble
- mouth/tail target highlighting

## Smart Features

### Auto Tail Attach

The editor may detect:

- nearest face
- speaking character
- explicit speaker layer
- camera framing

The tail can auto-target the inferred speaker while preserving author override.

### Collision Avoidance

Validation and optional auto-layout should prevent:

- overlapping balloons
- unreadable stacking
- tail intersections
- text covering important faces

### Reading Flow Assistant

The assistant analyzes:

- left-to-right flow
- right-to-left manga flow
- vertical webtoon flow

Suggestions are non-destructive and must be reviewable.

## Preset System

```yaml
Preset:
  appearance
  typography
  animations
  emotion
  fx
```

Categories:

- Manga
- Western comics
- Webtoon
- Cyberpunk
- Fantasy
- Horror
- Retro
- VN/UI
- Sci-fi HUD

Applying a preset should keep object identity, timeline links, speaker targets, localization variants, and runtime IDs.

## Runtime Integration With comics.engine

Runtime requirements:

- deterministic playback
- timeline scrubbing
- serialization
- runtime variable injection
- localization replacement
- dynamic resizing

### Runtime JSON Example

```json
{
  "id": "balloon_01",
  "type": "speech",
  "text": "Hello",
  "animation": {
    "enter": "pop",
    "idle": "float"
  },
  "timeline": {
    "start": 1.2,
    "duration": 4.0
  }
}
```

## Validation Rules

| Rule | Severity | Response |
|------|----------|----------|
| Text overflow | Warning/Error by export profile | Offer auto-fit, resize, split, or extension balloon |
| Missing localized text | Error | Show missing locale and block localized export |
| Missing glyph | Warning | Offer fallback font |
| Tail target missing | Warning | Reassign, convert to free point, or disable tail |
| Tail crosses face | Warning | Suggest reroute |
| Balloon overlaps critical art | Warning | Suggest alternate placement |
| Animation outside duration | Error | Clamp or extend duration |
| Unsupported runtime FX | Error | Fallback, bake, or block export |
| Contrast too low | Warning/Error by accessibility profile | Suggest high-contrast variant |

## Accessibility

Required support:

- dyslexia-friendly fonts
- subtitle export
- high contrast mode
- keyboard navigation
- screen reader metadata
- reduced-motion compatible animation substitutions

## Performance Requirements

Desktop target:

- 120 FPS editor viewport target

Mobile target:

- 60 FPS editing target

Web target:

- WebGL accelerated rendering

Optimization requirements:

- geometry caching
- GPU text rendering
- partial redraw
- timeline virtualization
- lazy FX evaluation

## Ownership Boundaries

This VDD owns:

- balloon entity data model
- balloon editor workspace
- balloon canvas handles and interaction semantics
- shape, tail, typography, FX, and preset behavior
- balloon timeline track structure
- balloon runtime export contract

Related VDDs own:

- semantic reading order, panels, and reader preview: `flows/vdd-comics.editor-semantic-reading-editor/`
- generic stage rendering and hit testing: `flows/vdd-comics.editor-rendering/`
- generic timeline evaluation: `flows/vdd-comics.editor-animation-timeline/`
- sound file management and playback: `flows/vdd-comics.editor-audio/`
- document bundle schema and migrations: `flows/vdd-comics.editor-format/`

## Future Extensions

### AI-Assisted Balloons

Possible future:

- auto balloon placement
- emotion detection
- dialogue pacing
- auto emphasis
- auto manga layout

### Procedural Cinematics

Balloon state may affect:

- camera shake
- panel zoom
- lighting
- music intensity

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
