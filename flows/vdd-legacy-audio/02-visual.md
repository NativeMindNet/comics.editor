# Visual Mockups: audio subsystem

> Version: 1.0
> Status: DRAFT
> Last Updated: 2026-05-08

## Overview

ASCII mockups for sound track list, per-sound controls, and timeline segments.

---

## Screen: Sounds Panel + Timeline

```
+--------------------------------------------------------------------------------+
| = Timeline:  0s --------------------- 10s --------------------- 20s            |
+--------------------------------------------------------------------------------+
| +--------------------------+  +----------------------------------------------+ |
| | = Sounds                 |  | = Canvas / Preview                           | |
| |  [x] sfx_click.mp3       |  |                                              | |
| |      Vol: [----|---]     |  |                                              | |
| |      Loop: ( )           |  |                                              | |
| |  [x] bg_music.mp3        |  |                                              | |
| |      Vol: [--|------]    |  |                                              | |
| |      Loop: (O)           |  |                                              | |
| |--------------------------|  |                                              | |
| | [Add Sound] [Remove]     |  |                                              | |
| | [Play] [Stop] [Scrub]    |  |                                              | |
| +--------------------------+  +----------------------------------------------+ |
|  Sound segments:                                                          |
|  bg_music:   [========== segment =========]                                |
|  sfx_click:              [==]    [==]                                       |
+--------------------------------------------------------------------------------+
```

### States

#### Error State

```
+--------------------------------------------------------------+
|  ! Audio device unavailable                                   |
+--------------------------------------------------------------+
|  Playback is disabled.                                        |
|  [Retry]  [Continue Editing]                                  |
+--------------------------------------------------------------+
```

---

## Flow: Scrub Preview

```
[Drag playhead] -> [Evaluate which SoundAnim segments intersect] -> [Start/stop audio]
```

---

## Notes

- Sound segments should be visually distinct and draggable/resizable if segment editing is supported.

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
