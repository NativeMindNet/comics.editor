# Visual Mockups: animation timeline

> Version: 1.0
> Status: DRAFT
> Last Updated: 2026-05-08

## Overview

ASCII mockups for a simple timeline with tracks per layer and per animation type.

---

## Screen: Timeline Editor

```
+--------------------------------------------------------------------------------+
| = Timeline   [Play] [Stop]   Time: 12.40s   Zoom: [ 1x v ]                     |
+--------------------------------------------------------------------------------+
| Tracks                                                                           |
| +----------------------+------------------------------------------------------+ |
| | HERO / Translate      | [==== segment ====]          [==]                    | |
| | HERO / Rotate         |           [=====]                                     | |
| | HERO / Scale          | [==]      [==]                                        | |
| | HERO / Opacity        | [==========]                                          | |
| | SFX_click / Sound     |                 [==]                                  | |
| +----------------------+------------------------------------------------------+ |
|            ^ playhead                                                          |
+--------------------------------------------------------------------------------+
| = Segment Inspector:                                                           |
| Type: [Translate v]   Start: [10.00]  End: [14.00]  Easing: [Linear v]         |
| From: (x=120,y=80)    To: (x=220,y=120)                                        |
+--------------------------------------------------------------------------------+
```

### States

#### Empty State

```
+------------------------------+
| = Timeline                   |
+------------------------------+
| No animations yet            |
| [Add Animation]              |
+------------------------------+
```

---

## Flow: Add Segment

```
[Select layer] -> [Add Animation Type] -> [Click-drag to create segment]
      -> [Edit parameters in inspector] -> [Preview]
```

---

## Notes

- Segments should snap to nearby boundaries optionally (later).
- Segment inspector edits should reflect immediately at playhead time.

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
