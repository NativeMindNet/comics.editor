# Requirements: Unity Comics Editor undo/redo

> Version: 2.0
> Status: DRAFT
> Last Updated: 2026-05-08

## Problem Statement

Editing mutates `ComicsDocument` and files under the temp workspace **without** a safety net. Unlike WPF (which also lacked undo), we want Unity to provide **Undo/Redo** to reduce data loss during experimentation.

## User Stories

**As a** author
**I want** to undo mistaken layer deletes, bad anim edits, or wrong imports
**So that** I can work faster with less fear

**As a** developer
**I want** edit operations expressed as commands
**So that** history stays deterministic and testable

## Acceptance Criteria

### Must Have

1. **Given** a model-only edit (e.g. change anim Start/End, add animation)
   **When** Undo then Redo
   **Then** document state matches exactly before/after

2. **Given** an asset edit (import/replace image)
   **When** Undo
   **Then** filesystem and model references are restored together

3. **Given** Ctrl+Z pressed
   **When** history has entries
   **Then** last action is undone

4. **Given** Ctrl+Y or Ctrl+Shift+Z pressed
   **When** undo history has undone entries
   **Then** action is redone

5. **Given** a drag gesture (timeline segment drag)
   **When** Undo
   **Then** entire gesture reverts as **one** step

### Should Have

- History depth limit (default: 50 steps)
- Visual indicator showing undo is available

### Won't Have (This Iteration)

- Persistent undo across Unity restarts
- History panel with labels (future)
- Selective undo (out-of-order)

## Design Decisions

### Command Pattern

All edit operations wrapped in `IEditCommand`:
```csharp
public interface IEditCommand
{
    string Description { get; }
    void Execute();
    void Undo();
}
```

Commands from `sdd-comics.editor-animation-timeline-ui`:
- `AddAnimCommand`, `RemoveAnimCommand`
- `UpdateAnimRangeCommand`, `UpdateAnimParamsCommand`
- `SetLayerImageCommand`, `ReorderLayerCommand`, `DeleteLayerCommand`
- `AddSoundAnimCommand`, `RemoveSoundAnimCommand`

### Save as Non-Undoable Barrier

**Decision: Yes** - Save clears undo history
- Rationale: After save, filesystem state is committed
- Undo across save boundaries would require versioned backups

### Numeric Field Coalescing

**Decision: Yes** - Coalesce rapid edits
- If same field edited within 500ms, merge into single command
- Prevents history pollution from typing "1000" as 4 steps

### Asset Undo Strategy

For image/sound replacement:
- Before: Backup old file to `.undo/` temp directory
- On Undo: Restore from backup, update model
- On history eviction: Delete backup files

## Constraints

- Memory bounded: max 50 commands, evict oldest
- Session-scoped: cleared on New/Open/Save
- No disk persistence of history

## References

- `sdd-comics.editor-animation-timeline-ui/02-specifications.md` - Command interfaces
- Unity's `Undo.RecordObject` pattern (we won't use it directly - custom stack)

---

## Approval

- [x] Reviewed by: Anton
- [x] Approved on: 2026-05-08
