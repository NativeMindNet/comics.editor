# Specifications: Unity Comics Editor undo/redo

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Requirements: [01-requirements.md](./01-requirements.md)

## Overview

Implement `HistoryController` with **transactions** and **IUndoableCommand** implementations for model + asset operations against the temp workspace.

## Affected Systems

| System | Impact |
|--------|--------|
| All editor mutations | Route through commands |
| `ComicsEditorSession` | Holds history; optional snapshot of `ComicsDocument` |
| File temp workspace | Journal / COW copies for reversibility |

## Architecture

```
UI → CommandFactory → HistoryController.Push(transaction)
                           │
                           ├─ Apply forward
                           └─ Unapply reverse
```

### Asset reversibility strategies

- **Copy-on-write**: new files written to staging names, commit on transaction end; unapply restores previous names.
- **Optional**: lightweight snapshot of `data.json` only + tombstones for assets.

## Data models

- `ComicsDocument` deep clone for model undo (Newtonsoft clone or dedicated cloner).
- File ops: record `{ oldPath, newPath, backupPath? }`.

## Edge cases

| Case | Behavior |
|------|----------|
| Undo while background tiling runs | Block or queue; define policy |
| Disk full mid-transaction | Roll back transaction; surface error |

## Testing Strategy

- [ ] Unit: stack push/pop, branch after undo
- [ ] Integration: import layer → undo → layers folder restored

## Open Design Questions

- [ ] Hybrid: command log + occasional full doc snapshot for brevity?

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
