# Visual Mockups: rendering & interaction engine

> Version: 1.0
> Status: DRAFT
> Last Updated: 2026-05-08

## Overview

ASCII mockups for the canvas layout, selection UI, and the core interaction flows (select, move, rotate/scale).

---

## Screen: Editor Canvas (desktop-ish layout)

```
+--------------------------------------------------------------------------------+
| = Top Bar: [Open] [Save] [Undo] [Redo]     Zoom: [ 100% v ]   Culture: [en v] |
+--------------------------------------------------------------------------------+
| +--------------------------+  +----------------------------------------------+ |
| | = Layers                 |  | = Canvas                                    | |
| |  [ ] BG                  |  |                                              | |
| |  [x] HERO   (selected)   |  |   +--------------------------------------+   | |
| |  [x] FX                  |  |   |  (viewport / scrollable)             |   | |
| |  [x] TEXT                |  |   |                                      |   | |
| |--------------------------|  |   |   [HERO] with handles                |   | |
| | = Properties             |  |   |     o--------o                       |   | |
| |  X: [  120 ]             |  |   |     |        |                       |   | |
| |  Y: [   80 ]             |  |   |     o--------o                       |   | |
| |  Rot: [  15° ]           |  |   |        (pivot) *                     |   | |
| |  Scale: [ 1.00 ]         |  |   +--------------------------------------+   | |
| +--------------------------+  +----------------------------------------------+ |
| ~ Status: tiles 24/36 loaded   FPS: 60   Cache: 120MB                         |
+--------------------------------------------------------------------------------+
```

---

## Flow: Select → Move

```
[Pointer down] -> [Hit test] -> [Select topmost layer] -> [Drag] -> [Commit transform]
       |                |
       | miss           v
       v          [Show handles]
 [Clear selection]
```

---

## Component: Selection Handles

```
  o----------------o
  |                |
  |     layer      |
  |                |
  o----------------o
        *
   rotate/pivot
```

Notes:
- corner `o` = scale handles
- `*` = rotate handle or pivot indicator (depending on mode)

---

## Notes

- Canvas must remain responsive while tiles stream in/out.
- Selection should be deterministic (same click = same selected layer).

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
