# Specifications: Unity animation & layer inspector UI

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Requirements: [01-requirements.md](./01-requirements.md)

## Overview

Design an **inspector + timeline** surface that manipulates `LayerModel.Animations` and `SoundModel.Animations` using the same domain types as WPF, emitting **explicit edit commands** for future undo.

## Affected Systems

| System | Impact |
|--------|--------|
| `ComicsEditorWindow` | Split panels: hierarchy, inspector, timeline |
| New view models (optional) | Thin wrappers over models for UI state |
| `ComicsEditorSession` | APIs for apply mutation with validation |

## Architecture

```
UI Event → Command (immutable description) → Apply to model → Save dirty flag
                │
                └──► (future) Push to UndoStack
```

## Command set (initial)

- `AddAnim`, `RemoveAnim`, `UpdateAnimRange`, `UpdateAnimParams`
- `SetLayerImage`, `SetLayerPopup`, `ReorderLayer`, `DeleteLayer`
- `AddSound`, `RemoveSound`, `UpdateSoundAnim`

## Data / schema

- No schema change required if Unity already serializes `List<Anim>` with `$type`; confirm on save.

## Edge cases

| Case | Behavior |
|------|----------|
| Overlapping segments of same type | Match WPF policy (prevent or last-wins); document choice |
| Delete last TranslateAnim | Block or auto-insert default per product decision |

## Testing Strategy

- [ ] Edit ops applied then `Save` → reopen in WPF  
- [ ] Property corruption fuzz on null lists (invariants)

## Open Design Questions

- [ ] UIToolkit for complex lists vs IMGUI speed of implementation?

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
