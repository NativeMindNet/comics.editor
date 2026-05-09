# Implementation Plan: Unity undo/redo system

> Version: 2.0
> Status: DRAFT
> Last Updated: 2026-05-08
> Specifications: [02-specifications.md](./02-specifications.md)

## Overview

Implement command-based undo/redo with UndoStack, command implementations, and UI integration.

## Phases

### Phase 1: Core Infrastructure

**Tasks:**
1. Create `IEditCommand` interface
2. Create `UndoStack` class with Execute/Undo/Redo
3. Implement coalescing logic (500ms window, CanMergeWith)
4. Implement depth limiting with IDisposable cleanup

**Files:**
- NEW: `Assets/Editor/Commands/IEditCommand.cs`
- NEW: `Assets/Editor/Commands/UndoStack.cs`

### Phase 2: Model Commands

**Tasks:**
1. Create `UpdateAnimRangeCommand` with coalescing
2. Create `AddAnimCommand`
3. Create `RemoveAnimCommand`
4. Create `UpdateAnimParamsCommand` (for type-specific fields)

**Files:**
- NEW: `Assets/Editor/Commands/UpdateAnimRangeCommand.cs`
- NEW: `Assets/Editor/Commands/AddAnimCommand.cs`
- NEW: `Assets/Editor/Commands/RemoveAnimCommand.cs`
- NEW: `Assets/Editor/Commands/UpdateAnimParamsCommand.cs`

### Phase 3: Asset Commands

**Tasks:**
1. Create `SetLayerImageCommand` with backup/restore
2. Implement `.undo/` directory management
3. Handle backup cleanup on Dispose (history eviction)

**Files:**
- NEW: `Assets/Editor/Commands/SetLayerImageCommand.cs`

### Phase 4: Session Integration

**Tasks:**
1. Add `UndoStack` property to `ComicsEditorSession`
2. Add `Execute(IEditCommand)` method
3. Add `Undo()` and `Redo()` methods
4. Clear history on New/Open/Save

**Files:**
- MODIFY: `Assets/ComicsEditorSession.cs`

### Phase 5: Window Integration

**Tasks:**
1. Add keyboard handling for Ctrl+Z, Ctrl+Y, Ctrl+Shift+Z
2. Add Undo/Redo toolbar buttons with enabled state
3. Wire existing edit operations through commands
4. Call `InvalidatePreviews()` after undo/redo

**Files:**
- MODIFY: `Assets/Editor/ComicsEditorWindow.cs`
- MODIFY: `Assets/Editor/Inspector/AnimationInspector.cs` (return commands)
- MODIFY: `Assets/Editor/Timeline/AnimationTimeline.cs` (return commands)

## Task Summary

| # | Task | Phase |
|---|------|-------|
| 1 | Create IEditCommand interface | 1 |
| 2 | Create UndoStack class | 1 |
| 3 | Implement coalescing | 1 |
| 4 | Create UpdateAnimRangeCommand | 2 |
| 5 | Create AddAnimCommand | 2 |
| 6 | Create RemoveAnimCommand | 2 |
| 7 | Create UpdateAnimParamsCommand | 2 |
| 8 | Create SetLayerImageCommand | 3 |
| 9 | Add UndoStack to session | 4 |
| 10 | Wire New/Open/Save to clear | 4 |
| 11 | Add keyboard shortcuts | 5 |
| 12 | Add toolbar buttons | 5 |
| 13 | Wire inspector to commands | 5 |
| 14 | Wire timeline to commands | 5 |

## Dependencies

- Animation Timeline UI implementation (completed)
- Inspector components (completed)

## Risks

| Risk | Mitigation |
|------|------------|
| Commands capturing stale references | Capture values at construction time |
| Backup files accumulating | Cleanup on eviction via IDisposable |
| Merge logic incorrect | Keep original old values, take new new values |

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
