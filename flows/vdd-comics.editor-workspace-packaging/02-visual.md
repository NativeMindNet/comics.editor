# Visual Mockups: workspace & packaging

> Version: 1.0
> Status: DRAFT
> Last Updated: 2026-05-08

## Overview

ASCII mockups for the save/open flows and the conceptual workspace layout.

---

## Component: Workspace Layout (conceptual)

```
workspace/
|
|-- document/
|    |-- data.json
|    |-- manifest.json
|
|-- assets/
|    |-- layers/...
|    |-- sounds/...
|
|-- derived/
|    |-- tiles/...
|
|-- journal/
|    |-- operations.log        (optional; for recovery/undo)
|    |-- pending/              (atomic ops staging)
```

---

## Flow: Atomic Save

```
[Edit in workspace] -> [Save]
       |
       v
[Write new bundle to temp file] -> [fsync/close] -> [Rename temp -> final]
       |
       +-> if fail: keep old bundle unchanged + report error
```

---

## Screen: Save Progress (large documents)

```
+--------------------------------------------------------------+
| = Saving                                                     |
+--------------------------------------------------------------+
|  Packing assets...                                            |
|  [==========>          ]  52%                                 |
|                                                              |
|  Current: tiles/s2/x12_y7.png                                 |
|  [Cancel]                                                     |
+--------------------------------------------------------------+
```

---

## Notes

- Recovery should be explicit and safe: never silently produce a corrupted bundle.

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
