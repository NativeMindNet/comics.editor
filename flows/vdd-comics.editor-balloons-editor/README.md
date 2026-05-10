# balloons editor

> Client-Facing Documentation
> Last Updated: 2026-05-10
> Version: 2.0

## What This Feature Does

Balloon Editor v2 is the dedicated comics.editor workspace for speech balloons, narration boxes, thought clouds, kinetic dialogue, emotional typography, and runtime-ready animated text objects.

It is part of the broader semantic reading workflow, but this VDD is the source of truth for balloon-specific behavior, visuals, data model, animation, presets, FX, audio sync, and runtime export.

## How It Works

In simple terms:

1. The author places a balloon directly on the canvas.
2. The author edits text inline and reshapes the balloon with visible handles.
3. The author drags the tail to a speaker or target point.
4. The author chooses a preset such as Speech, Thought, Scream, Whisper, Narration, Radio, System, or Emotional.
5. The author animates the balloon, text, tail, and FX on a timeline.
6. The editor exports a deterministic runtime object for comics.engine.

## Key Benefits

- Balloons are semantic scene objects, not flat artwork baked into an image.
- Static and animated balloons share the same timeline-aware model.
- Text can animate independently from the balloon shape.
- Presets make common comic, manga, webtoon, VN, and sci-fi styles reusable.
- Tail targets, localization, audio sync, accessibility metadata, and reading-flow checks stay attached to the balloon object.

## Where It Fits

- `vdd-comics.editor-semantic-reading-editor` owns reading order, panels, reader preview, and semantic relationships.
- `vdd-comics.editor-balloons-editor` owns the balloon object itself and the editor used to create and animate it.
- `vdd-comics.editor-animation-timeline` owns shared timeline evaluation.
- `vdd-comics.editor-rendering` owns stage rendering, hit testing, handles, and transforms.
- `vdd-comics.editor-format` owns storage, schema migration, and bundle compatibility.

## Getting Started

1. Open a `.comics` document.
2. Switch to `Balloon` mode or create a balloon from the semantic reading editor.
3. Pick a preset.
4. Type directly inside the balloon.
5. Adjust shape and tail handles on the canvas.
6. Add enter, idle, text, FX, or exit animation on the balloon timeline.
7. Preview against target devices and export.

---

**Note for Stakeholders**: Technical details and progress are tracked in `05-implementation-log.md`.
