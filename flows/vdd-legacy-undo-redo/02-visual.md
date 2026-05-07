# Visual Mockups: undo/redo + history

> Version: 1.0
> Status: DRAFT
> Last Updated: 2026-05-08

## Overview

ASCII mockups for the undo/redo UX: buttons, history list, and how actions group into transactions.

---

## Screen: Editor with History Panel

```
+--------------------------------------------------------------------------------+
| = Top Bar: [Open] [Save] [Undo] [Redo]                                         |
+--------------------------------------------------------------------------------+
| +--------------------------+  +----------------------------------------------+ |
| | = History                |  | = Canvas                                    | |
| |  12: Move HERO           |  |                                              | |
| |  11: Rotate HERO         |  |   (selected layer shows handles)            | |
| |  10: Add Layer: FX       |  |                                              | |
| |  09: Import Image (ru)   |  |                                              | |
| |--------------------------|  |                                              | |
| | [Undo] [Redo]            |  |                                              | |
| +--------------------------+  +----------------------------------------------+ |
| ~ Status:  undo=12 redo=0                                                  |
+--------------------------------------------------------------------------------+
```

---

## Flow: Gesture Transaction (drag)

```
[Pointer down] -> [Begin transaction: "Move HERO"] -> [Many move updates]
       -> [Pointer up] -> [Commit transaction] -> [One history entry]
```

---

## Flow: Branching on new action after undo

```
[Do actions A, B, C] -> Undo -> Undo (now at A)
   |
   +-> [Do action D] -> (redo stack cleared) -> history: A, D
```

---

## Notes

- History should show human-readable names.
- Long-running asset operations should still be undoable, but may show “restoring…” states briefly.

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
