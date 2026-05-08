# Specifications: undo/redo + transactional editing

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Requirements: [link to 01-requirements.md]

## Overview

Introduce an undo/redo subsystem suitable for an editor that changes:
- document model state (layers, transforms, animations, sounds)
- asset/workspace state (import, delete, retile, rename, save/export)

## Affected Systems

| System | Impact | Notes |
|--------|--------|-------|
| Undo/redo core | Create | stacks, transactions, action naming |
| Tool system | Modify | define transaction boundaries (drag, scrub, edit) |
| Asset pipeline | Modify | make operations reversible/atomic |
| UI | Create/Modify | undo/redo buttons + optional history list |

## Architecture

### Component Diagram

```
[Tools/UI] -> [HistoryController] -> [Action/Transaction Log]
                     |
                     v
             [Apply/Unapply]
                /       \
      [Model changes]  [Asset changes]
```

### Data Flow

```
user event -> tool emits actions -> controller groups into transaction -> push to undo stack
undo -> pop transaction -> unapply actions in reverse -> push to redo
redo -> pop -> apply actions forward -> push back to undo
```

## Interfaces

### New Interfaces (conceptual)

```cpp
interface UndoableAction {
  string label;
  void apply();
  void unapply();
}

interface Transaction {
  string label;
  list<UndoableAction> actions;
}

interface HistoryController {
  void begin(string label);
  void add(UndoableAction action);
  void commit();
  void rollback();
  void undo();
  void redo();
}
```

## Behavior Specifications

### Transaction boundaries

- Drag/move: begin on pointer down, commit on pointer up.
- Continuous sliders/scrub: coalesce changes while interaction is active; commit on release.
- Multi-step commands (import+tile+attach): either single transaction or staged with clear UX.

### Asset-aware actions

Asset changes must be atomic and reversible:
- write new assets to temp path then commit/rename
- keep old versions until transaction commits
- for undo, restore old paths and metadata

### Error Handling

| Error | Cause | Response |
|-------|-------|----------|
| Unapply fails | missing asset | stop and report; prevent further edits until resolved |
| Apply fails | disk full | transaction aborted; show error and rollback partial writes |

## Dependencies

- Workspace/packaging design (see `vdd-legacy-workspace-packaging`) impacts asset reversibility.

## Testing Strategy

### Unit Tests

- [ ] Coalescing + transaction grouping rules
- [ ] Undo/redo stack branching behavior

### Integration Tests

- [ ] Import image -> place -> undo -> verify asset + references removed/restored correctly

## Migration / Rollout

- Start with model-only undo/redo.
- Add asset-aware undo in second pass once workspace model is stable.

## Open Design Questions

- [ ] Best representation for large state: commands only vs snapshots for some operations?
- [ ] How to handle background jobs that complete after undo?

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
