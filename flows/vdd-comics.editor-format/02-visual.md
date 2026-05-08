# Visual Mockups: legacy document format v2 (comics/puzzle)

> Version: 1.0
> Status: DRAFT
> Last Updated: 2026-05-08

## Overview

ASCII mockups for the **document bundle layout** and the **conceptual data model** to align on what “a document” is before writing the technical schema.

---

## Screen: Document Inspector (Editor UI)

This is a small UI panel (or page) that shows document metadata, schema version, and asset health.

```
+--------------------------------------------------------------+
|  = Document Inspector                                        |
+--------------------------------------------------------------+
|  File:  my_episode_12.comics                                 |
|  Format: ZIP bundle                                           |
|  Schema: v2 (schemaVersion=2)     [Export Legacy] [Validate] |
|--------------------------------------------------------------|
|  Canvas:  2048 x 1536                                        |
|  Cultures:  en, ru, es                                       |
|--------------------------------------------------------------|
|  Assets                                                     |
|  +--------------------------------------------------------+  |
|  | Layers: 12        Missing: 0     Tiles: 384            |  |
|  | Sounds: 3         Missing: 0                           |  |
|  | Checksums: OK     Manifest: OK                         |  |
|  +--------------------------------------------------------+  |
|--------------------------------------------------------------|
|  Warnings                                                    |
|  - none                                                      |
+--------------------------------------------------------------+
```

### States

#### Error State (missing assets)

```
+--------------------------------------------------------------+
|  = Document Inspector                                        |
+--------------------------------------------------------------+
|  ! Validation failed                                         |
|  Missing assets: 4                                           |
|  - layers/hero/en/tiles/hero_1_2_3.png                        |
|  - sounds/sfx_click.mp3                                      |
|                                                             |
|  [Repair...]  [Open Anyway]  [Cancel]                        |
+--------------------------------------------------------------+
```

---

## Flow: Document Load & Migrate

```
[Open File] -> [Detect schema/legacy] -> [Migrate?] -> [Validate] -> [Open Editor]
                      |                    |
                      | (legacy)           | (fails)
                      v                    v
              [Legacy Import]        [Show Issues + Options]
```

### Step-by-Step

1. **Open File**: user selects `.comics` or `.puzzle`
2. **Detect**: loader identifies legacy vs v2
3. **Migrate** (if legacy): creates in-memory v2 representation (and optionally writes new bundle)
4. **Validate**: checks manifest + referenced assets
5. **Open Editor**: editor renders with incremental tile loading

---

## Component: Bundle Layout (v2, conceptual)

```
my_doc.comics (zip)
|
|-- manifest.json        (schemaVersion, checksums, index)
|-- data.json            (document model: canvas, layers, anims, sounds)
|
|-- layers/
|    |-- <layerId>/
|         |-- images/
|              |-- <cultureCode>/
|                   |-- original.png              (optional)
|                   |-- tiles/
|                        |-- s1/ x0_y0.png ...     (scale 1.0)
|                        |-- s2/ x0_y0.png ...     (scale 0.5)
|                        |-- ...
|         |-- popups/
|              |-- <cultureCode>/ popup.png
|
|-- sounds/
     |-- <soundId>.mp3
```

Notes:
- Culture mapping is by `cultureCode` folder, not list index.
- Tiles are grouped by scale directory; names are manifest-driven (avoid fragile parsing).

---

## Notes

- Keep visuals simple; this phase is to align on “what exists” (bundle layout, inspector UX, and load flow).

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
