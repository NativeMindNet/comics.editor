# Specifications: Unity animation & layer inspector UI

> Version: 2.0
> Status: DRAFT
> Last Updated: 2026-05-08
> Requirements: [01-requirements.md](./01-requirements.md)

## Overview

Design an **inspector + dual-rail timeline** surface that manipulates `LayerModel.Animations` and `SoundModel.Animations` using the same domain types as WPF, emitting **explicit edit commands** for future undo.

## Design Decisions

1. **Two synced rails** - Layer anims and sound anims in separate but scroll-synced timeline rails
2. **Auto-seek ON with toggle** - Selecting anim seeks scroll to `anim.End` (or layer Y for default translate)
3. **IMGUI implementation** - Consistent with existing editor, faster to implement

## Affected Systems

| System | Impact |
|--------|--------|
| `ComicsEditorWindow` | Add inspector panel, timeline panel |
| `ComicsEditorSession` | APIs for animation CRUD, image/popup change |
| New: `AnimationInspector` | IMGUI component for selected anim fields |
| New: `AnimationTimeline` | IMGUI component for dual-rail timeline |

## Architecture

```
UI Event → Command (immutable description) → Apply to model → Save dirty flag
                │
                └──► (future) Push to UndoStack
```

## UI Layout

```
┌─────────────────────────────────────────────────────────────────┐
│ Toolbar: [New Comics] [New Puzzle] [Open] [Save]                │
├─────────────┬─────────────────────────────────────────────────────┤
│ Left Panel  │  Center: Preview (Composed/Stacked)                │
│             │                                                     │
│ - Canvas    ├─────────────────────────────────────────────────────┤
│ - Scroll    │  Timeline (dual-rail)                               │
│ - Culture   │  ┌─────────────────────────────────────────────────┐│
│             │  │ Layer: [===T===][==R==][====S====][=A=]         ││
│ Layers:     │  │ Sound: [==S1==]     [====S2====]                ││
│ [Layer 0] ◄─│  └─────────────────────────────────────────────────┘│
│ [Layer 1]   │  ^ scroll ruler: 0────1000────2000────3000          │
│ [Layer 2]   ├─────────────────────────────────────────────────────┤
│             │  Inspector (selected anim/layer/sound)              │
│ Sounds:     │  ┌─────────────────────────────────────────────────┐│
│ [Sound 0]   │  │ TranslateAnim                                   ││
│             │  │ Start: [____] End: [____]                       ││
│ [+ Layer]   │  │ X: [____]  Y: [____]                            ││
│ [+ Sound]   │  └─────────────────────────────────────────────────┘│
└─────────────┴─────────────────────────────────────────────────────┘
```

## Inspector Fields by Type

### Base (all anims)
| Field | Type | Notes |
|-------|------|-------|
| Start | int | Scroll position where anim begins |
| End | int | Scroll position where anim ends |

### TranslateAnim
| Field | Type | Default |
|-------|------|---------|
| X | double | 0 |
| Y | double | layer.y (initial position) |

### RotateAnim
| Field | Type | Default |
|-------|------|---------|
| PivotX | double | 0 (relative to layer) |
| PivotY | double | 0 |
| Angle | double | 0 (degrees) |

### ScaleAnim
| Field | Type | Default |
|-------|------|---------|
| PivotX | double | 0 |
| PivotY | double | 0 |
| ScaleX | double | 1.0 |
| ScaleY | double | 1.0 |

### AlphaAnim
| Field | Type | Default |
|-------|------|---------|
| Alpha | double | 1.0 (0-1 range) |

### SoundAnim
| Field | Type | Notes |
|-------|------|-------|
| Start | int | When sound triggers |
| End | int | If Start==End: play once; else: loop while in range |

## Layer Inspector (non-anim fields)

| Field | Action |
|-------|--------|
| Change Image | `EditorUtility.OpenFilePanel` → update per-culture image |
| Change Popup | `EditorUtility.OpenFilePanel` → update per-culture popup |
| Delete Layer | Remove from document |
| Move Up/Down | Reorder in z-stack |

## Timeline Component

### Visual Design
- **Ruler**: Horizontal scroll scale (0 to maxScroll)
- **Layer rail**: Colored segments per anim type (T=blue, R=green, S=orange, A=purple)
- **Sound rail**: Segments for each SoundAnim
- **Playhead**: Vertical line at current scroll position
- **Zoom**: Mouse wheel to zoom timeline scale

### Interactions
| Action | Behavior |
|--------|----------|
| Click segment | Select anim, auto-seek scroll (if enabled) |
| Drag segment edge | Resize Start/End |
| Drag segment middle | Move segment (adjust Start/End together) |
| Double-click empty | Add new anim at position |
| Right-click segment | Context menu: Delete, Duplicate |
| Ctrl+click | Multi-select (future) |

### Segment Colors
```csharp
static readonly Dictionary<AnimTypes, Color> SegmentColors = new()
{
    { AnimTypes.Translate, new Color(0.3f, 0.5f, 0.9f) },  // Blue
    { AnimTypes.Rotate,    new Color(0.3f, 0.8f, 0.4f) },  // Green
    { AnimTypes.Scale,     new Color(0.9f, 0.6f, 0.2f) },  // Orange
    { AnimTypes.Alpha,     new Color(0.7f, 0.4f, 0.9f) },  // Purple
    { AnimTypes.Sound,     new Color(0.9f, 0.3f, 0.3f) },  // Red
};
```

## Command Interfaces (Undo-Ready)

```csharp
public interface IEditCommand
{
    string Description { get; }
    void Execute();
    void Undo();
}

// Layer animation commands
public record AddAnimCommand(LayerModel Layer, AnimTypes Type, double Scroll);
public record RemoveAnimCommand(LayerModel Layer, Anim Anim);
public record UpdateAnimRangeCommand(Anim Anim, int NewStart, int NewEnd);
public record UpdateAnimParamsCommand(Anim Anim, Dictionary<string, object> Changes);

// Layer commands
public record SetLayerImageCommand(LayerModel Layer, Cultures Culture, string FilePath, bool IsPopup);
public record ReorderLayerCommand(int OldIndex, int NewIndex);
public record DeleteLayerCommand(LayerModel Layer);

// Sound commands
public record AddSoundAnimCommand(SoundModel Sound, double Scroll);
public record RemoveSoundAnimCommand(SoundModel Sound, Anim Anim);
```

## Auto-Seek Behavior

When `SelectedAnim` changes (from `LayerViewModel` WPF reference):

```csharp
if (_selectedAnim != null)
{
    // For default translate (End=0), seek to layer Y - 1000
    // Otherwise seek to anim.End
    Parent.Scroll = _selectedAnim.End == 0 && _selectedAnim is TranslateAnim t
        ? Math.Max(t.Y - 1000, 0)
        : _selectedAnim.End;
}
```

Toggle checkbox "Sync scroll to selection" disables this behavior.

## Edge Cases

| Case | Behavior |
|------|----------|
| Overlapping segments of same type | Allow (WPF allows), last-wins during playback |
| Delete last anim of type | Allow (no forced defaults) |
| Drag segment past 0 | Clamp Start to 0 |
| Drag Start past End | Swap Start/End |
| Empty layer (no image) | Show placeholder in timeline, disable image-dependent ops |

## Testing Strategy

- [ ] Add each anim type → verify in preview
- [ ] Edit Start/End via inspector → verify segment moves in timeline
- [ ] Edit Start/End via drag → verify inspector updates
- [ ] Save → reopen in WPF → verify data integrity
- [ ] Auto-seek toggle on/off → verify scroll behavior
- [ ] Change image per culture → verify file copied to layers/

## Open Design Questions

(Resolved - see Design Decisions above)

---

## Approval

- [x] Reviewed by: Anton
- [x] Approved on: 2026-05-08
