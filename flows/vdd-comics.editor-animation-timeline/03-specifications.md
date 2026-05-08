# Specifications: animation timeline (segments, evaluation, editing)

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Requirements: [link to 01-requirements.md]

## Overview

Define how animations are represented and evaluated in the new editor, and how timeline UI edits map to model updates. Maintain legacy parity for segment semantics while enabling future evolution.

## Affected Systems

| System | Impact | Notes |
|--------|--------|-------|
| Animation model | Create/Modify | segment representation, easing |
| Timeline evaluator | Create | computes effective state at time T |
| UI | Create/Modify | tracks, segments, inspector |
| Undo/Redo | Modify | timeline edits are transactional |

## Architecture

### Component Diagram

```
[Timeline UI] -> [Anim Editing API] -> [DocumentModel]
                       |
                       v
                [Evaluator @ time T]
                       |
                       v
                  [SceneGraph]
```

### Data Flow

```
time T -> for each layer: gather relevant segments -> resolve overlaps -> compute effective values
```

## Interfaces

### New Interfaces (conceptual)

```cpp
interface AnimEvaluator {
  EvaluatedLayerState evaluate(layerId, time);
}

interface AnimEditor {
  void addSegment(...);
  void moveSegment(id, delta);
  void resizeSegment(id, newStart, newEnd);
  void updateParams(id, params);
}
```

## Data Models

### New Types (conceptual)

```cpp
struct AnimSegment {
  string id;
  string targetId;   // layerId or soundId
  AnimType type;     // translate/rotate/scale/opacity/sound
  double start;
  double end;
  Easing easing;
  Params params;     // from/to, etc.
}
```

## Behavior Specifications

### Segment evaluation

- Active if \(t \in [start, end]\).
- Value computed by interpolating params using easing factor.

### Overlap rules (initial)

- Same-type overlaps on same target are not allowed by default (UI prevents) OR last-defined wins (decision to lock).
- Different types compose (translate * rotate * scale, opacity separate).

### Error Handling

| Error | Cause | Response |
|-------|-------|----------|
| Invalid segment | end < start | prevent in UI; auto-correct or block |
| Unknown easing | schema mismatch | fallback to linear + warning |

## Dependencies

- Rendering engine consumes evaluated transforms (`vdd-legacy-rendering`).
- Undo/redo system for transactional edits (`vdd-legacy-undo-redo`).

## Testing Strategy

### Unit Tests

- [ ] Interpolation correctness per type
- [ ] Boundary conditions at start/end

### Integration Tests

- [ ] Scrub playhead and verify scene matches expected evaluated state

## Open Design Questions

- [ ] Overlap policy and UI constraints.
- [ ] Seconds vs frames timeline units.
- [ ] Migration path to keyframes/curves (future).

---

## Behavior Specifications - Legacy Additions
> Added by /legacy on 2026-05-08

### FindNearest Algorithm (Current WPF Implementation)

The legacy system uses `FindNearest<T>()` to locate the active segment:

```csharp
private static (T prev, T curr) FindNearest<T>(IList<Anim> anims, double scroll)
{
    T prev = null, curr = null;

    foreach (var anim in anims.OfType<T>().OrderBy(x => x.Start))
    {
        if (anim.End <= scroll)
            prev = anim;           // completed segment
        else
        {
            if (anim.Start < scroll)
                curr = anim;       // currently active
            break;
        }
    }

    if (prev == null) { prev = new T(); prev.Init(); }
    return (prev, curr);
}
```

**Logic:**
1. Iterate segments ordered by Start time
2. `prev` = last segment that completed (End <= scroll)
3. `curr` = segment currently active (Start < scroll < End)
4. If no `prev`, create default instance with Init() defaults
5. If `curr` exists, interpolate between prev and curr

### Easing Factor Formula

The legacy system uses a hardcoded cubic ease-out:

```csharp
protected double Factor(double scroll)
{
    var t = (scroll - Start) / (End - Start);  // normalize to [0, 1]
    return (--t) * t * t + 1;                  // cubic ease-out
}
```

**Characteristics:**
- Pre-decrement `--t` transforms range from [0,1] to [-1,0]
- Result: slow start, fast middle, smooth deceleration
- Formula equivalent to standard ease-out-cubic

### Segment Lifecycle (WPF Implementation)

**Create (Add):**
```csharp
public static T Add<T>(IList<Anim> anims, double scroll)
{
    var prev = FindNearest<T>(anims, double.MaxValue).prev;
    var anim = (T)prev.MemberwiseClone();
    anim.Start = scroll > prev.End ? (int)scroll : prev.End + 1;
    anim.End = anim.Start + 200;  // Default duration: 200 scroll units
    anims.Add(anim);
    return anim;
}
```

**Default Values (via Init()):**
| Type | Defaults |
|------|----------|
| TranslateAnim | X=0, Y=0 |
| RotateAnim | Angle=0, PivotX=0.5, PivotY=0.5 |
| ScaleAnim | ScaleX=1.0, ScaleY=1.0, PivotX=0.5, PivotY=0.5 |
| AlphaAnim | Alpha=1.0 |
| SoundAnim | Point trigger (Start=scroll, End=scroll) |

**Resize/Move (Drag Handles):**
```csharp
// Top thumb: resize start
anim.Start = Math.Min(anim.Start + delta, anim.End);

// Bottom thumb: resize end
anim.End = Math.Max(anim.End + delta, anim.Start);

// Center thumb: move both
anim.Start += delta;
anim.End += delta;
```

### Scroll Propagation Architecture

Single `Scroll` property broadcasts to all layers/sounds:

```csharp
public double Scroll
{
    set {
        _scroll = value;
        OnPropertyChanged(nameof(Scroll));
        foreach (var layer in Layers) layer.Scroll();  // All layers update
        foreach (var sound in Sounds) sound.Scroll();  // All sounds update
    }
}
```

**LayerViewModel.Scroll():**
```csharp
public void Scroll()
{
    Translate = Anim.Interpolate<TranslateAnim>(Layer.Animations, SelectedAnim, scroll);
    Rotate = Anim.Interpolate<RotateAnim>(Layer.Animations, SelectedAnim, scroll);
    Scale = Anim.Interpolate<ScaleAnim>(Layer.Animations, SelectedAnim, scroll);
    Alpha = Anim.Interpolate<AlphaAnim>(Layer.Animations, SelectedAnim, scroll);
}
```

### SoundAnim Special Behavior

Unlike other animation types, SoundAnim uses event-based triggering:

```csharp
// SoundAnim.FindCurrent() criteria:
// Range: Start <= scroll && End >= scroll
// Point trigger: Start == End && prevScroll < scroll && scroll >= Start

// Playback behavior:
if (anim.Start == anim.End)
    Play(Playing);   // One-shot
else
    Play(Looping);   // Loop while in range
```

### Focus-Aware Property Editing

TextBox binding shows live scroll position when focused:

```csharp
// ScrollConverter
if (isFocused) return (int)scroll;  // Live position
else return value;                   // Stored value
```

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
