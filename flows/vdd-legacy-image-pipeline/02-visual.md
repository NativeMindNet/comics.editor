# Visual Mockups: image pipeline (import, tiling, preview)

> Version: 1.0
> Status: DRAFT
> Last Updated: 2026-05-08

## Overview

ASCII mockups to align on the import/tiling UX and how tiles are fetched/rendered during pan/zoom.

---

## Screen: Import Image → Tiling Job

```
+--------------------------------------------------------------+
|  = Import Image                                               |
+--------------------------------------------------------------+
|  File:  hero_ru.png                               [Browse...] |
|  Target: [Layer: HERO]                                        |
|--------------------------------------------------------------|
|  Tiling                                                      |
|  Tile size:   [ 512 ] px                                      |
|  Pyramid:     (O) None   (O) Puzzle default   (O) Custom      |
|  Levels:      [1.0] [0.5] [0.25] [0.125]                      |
|  Store original: (O) Yes  (O) No                              |
|--------------------------------------------------------------|
|  Output estimate                                              |
|  Tiles: ~384     Size: ~120MB                                 |
|--------------------------------------------------------------|
|  [Start]  [Cancel]                                            |
+--------------------------------------------------------------+
```

### Loading State (job running)

```
+--------------------------------------------------------------+
|  = Generating tiles                                           |
+--------------------------------------------------------------+
|  Progress:  43%                                               |
|  [=========>         ]                                        |
|                                                              |
|  Current: level 0.5, row 12, col 7                            |
|  [Cancel Job]                                                 |
+--------------------------------------------------------------+
```

### Error State

```
+--------------------------------------------------------------+
|  ! Tile generation failed                                     |
+--------------------------------------------------------------+
|  Reason: Out of disk space                                    |
|                                                              |
|  [Change Output Location]  [Retry]  [Cancel]                  |
+--------------------------------------------------------------+
```

---

## Component: Tile streaming during pan/zoom (conceptual)

```
Viewport (screen)
+------------------------------+
|  visible tiles only          |
|  +----+----+----+            |
|  | t  | t  | t  |   ...      |
|  +----+----+----+            |
|  | t  | t  | t  |            |
|  +----+----+----+            |
+------------------------------+

Tile cache:
[hot] decoded tiles for current viewport + small margin
[warm] encoded tiles on disk / bundle
[cold] tiles not yet generated (optional)
```

---

## Flow: Import → Use on Canvas

```
[Pick file] -> [Configure tiling] -> [Run job] -> [Attach to layer] -> [Render via tiles]
```

---

## Notes

- Preview should not require stitching tiles into a single bitmap; render visible tiles instead.

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
