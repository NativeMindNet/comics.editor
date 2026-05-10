# Visual Mockups: Balloon Editor v2

> Version: 2.0
> Status: DRAFT
> Last Updated: 2026-05-10
> Requirements: [01-requirements.md](./01-requirements.md)

## Overview

ASCII mockups for the dedicated Balloon Editor workspace inside comics.editor. This document is the visual source of truth for balloon authoring; other VDDs should link here instead of duplicating balloon editor screens.

## Screen: Full Workspace

```text
+--------------------------------------------------------------------------------+
| Toolbar                                                                        |
| [Select] [Balloon] [Text] [Tail] [Pen] [Animate] [FX] [Preview]               |
+----------------------+-----------------------------------+---------------------+
| Asset / Presets      |                                   | Inspector           |
|----------------------|                                   |---------------------|
| Speech               |                                   | Balloon             |
| Thought              |                                   | - Shape             |
| Scream               |           CANVAS                  | - Border            |
| Whisper              |                                   | - Tail              |
| Narration            |                                   | - Animation         |
| Radio / Phone        |                                   | - Typography        |
| AI / System          |                                   | - Audio Sync        |
| Emotional            |                                   | - Runtime           |
+----------------------+-----------------------------------+---------------------+
| Timeline                                                                       |
| [Track] Balloon_01   |----====----~~~~-----|                                  |
| [Track] Text         |--fade--pop--shake---|                                  |
| [Track] FX           |------glow-----------|                                  |
+--------------------------------------------------------------------------------+
```

## Screen: Selection Handles

```text
       ROTATE
          ^
          o
     o---------o
   o             o
  o    HELLO      o
  o               o
   o             o
     o---------o
          \
           \
            *
```

Legend:

- `o` = resize or shape node
- `*` = tail anchor or speaker target
- all handle hit areas are at least 44 x 44 logical pixels

## Screen: Tail Editing

```text
Balloon
   \
    o------o
           \
            o
             \
              * speaker target
```

Tail tools must support:

- straight tail
- bezier tail
- multi-segment tail
- procedural tail
- snap to character mouth
- magnetic attach
- auto avoidance
- auto smoothing

## Component: Balloon Types

```text
Speech:       ( hello )

Thought:      o o o  ( maybe... )

Whisper:      . . . . . . . . .
              . stay quiet...  .
              . . . . . . . . .

Scream:       /\/\/\/\/\/\/\/\/\
              | HELP!           |
              \/\/\/\/\/\/\/\/\/

Narration:    +----------------+
              | Long ago...    |
              +----------------+

System:       [:: SYSTEM READY ::]
```

## Screen: Preset Browser

```text
+---------------------------+
| PRESETS                   |
|---------------------------|
| [Speech]                  |
| ( hello )                 |
|                           |
| [Scream]                  |
| /\/ STOP /\/              |
|                           |
| [Narration]               |
| [ Long ago... ]           |
|                           |
| [AI / System]             |
| [:: STATUS ::]            |
+---------------------------+
```

Preset categories:

- Manga
- Western comics
- Webtoon
- Cyberpunk
- Fantasy
- Horror
- Retro
- VN / UI
- Sci-fi HUD

## Screen: Procedural Shape Controls

```text
+--------------------------------------------------------------------------------+
| Procedural Shape: Scream                                                        |
+----------------------+---------------------------------------------------------+
| Base                 | Preview                                                 |
| (*) Ellipse          |                                                         |
| ( ) Rounded Rect     |           /\/\/\/\/\/\/\/\/\                            |
| ( ) Polygon          |           |  TOO LATE!      |                            |
| (*) Procedural       |           \/\/\/\/\/\/\/\/\/                            |
|                      |                                                         |
| Modifiers            | Handles                                                 |
| Chaos:     [====  ]  | o sharpness node      o asymmetry node                   |
| Sharpness: [===== ]  |                                                         |
| Softness:  [==    ]  |                                                         |
| Frequency: [====  ]  |                                                         |
| Pressure:  [===   ]  |                                                         |
+----------------------+---------------------------------------------------------+
```

## Screen: Inline Rich Text

```text
+--------------------------------------------------------------------------------+
| Text Tool                                                                       |
+--------------------------------------------------------------------------------+
| +--------------------------------------+   Span Inspector                       |
| | I said <bold>NO</bold>.              |   [B] [I] [Ruby] [Icon] [Gradient]     |
| |                                      |   Animation: [typewriter v]            |
| | ruby: kanji / furigana               |   Direction: [LTR v]                   |
| +--------------------------------------+   Layout: [auto balance lines]          |
+--------------------------------------------------------------------------------+
```

## Screen: Animation Timeline

```text
+--------------------------------------------------------------------------------+
| Balloon Timeline: Balloon_01                                                    |
+--------------------------------------------------------------------------------+
| Time       0s        1s        2s        3s        4s        5s                 |
| Transform  [ scale pop ]---------------------------[ exit fade ]                |
| Shape          [ wobble idle ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ ]                 |
| Tail       ----[ draw tail ]------------------------[ retract ]                 |
| Text           [ typewriter H e l l o ] [ shake word: "NOW" ]                  |
| FX             [ glow ]             [ manga burst ]                            |
| Audio Sync          [ voice waveform :::::::::::::::::::: ]                    |
+--------------------------------------------------------------------------------+
```

## Component: Animation Curve Editor

```text
+---------------------------------------------------+
| Curve: Scale                                      |
|                                                   |
| 1.2 |                          *                  |
|     |                       ***                   |
| 1.0 |--------------------***--------------------  |
|     |                ****                         |
| 0.8 |___________*****___________________________  |
|                                                   |
+---------------------------------------------------+
```

## Component: State Machine

```text
          +-----------+
          | Hidden    |
          +-----------+
                 |
                 v
          +-----------+
          | Entering  |
          +-----------+
                 |
                 v
          +-----------+
          | Visible   |
          +-----------+
             |     |
             |     v
             |  +-------+
             |  | Idle  |
             |  +-------+
             |
             v
          +-----------+
          | Exiting   |
          +-----------+
                 |
                 v
          +-----------+
          | Destroyed |
          +-----------+
```

## Mobile Variant: Radial Menu

```text
          [FX]
             \
 [Text] -- (Balloon) -- [Tail]
             /
        [Animate]
```

Mobile and tablet behavior:

- selected balloon exposes a local radial menu
- primary edit actions stay within thumb reach where possible
- two-finger pan and pinch zoom remain available while balloon is selected
- long press opens context options

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
