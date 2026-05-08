# Understanding: Animation System

## Phase: SYNTHESIZING

## Hypothesis

Scroll-driven animation system with 6 animation types using segment-based timeline model with cubic easing interpolation.

## Sources

- `app/unity_comics.editor/Comics.Editor/Models/Anim.cs` - Base animation class and factory methods
- `app/unity_comics.editor/Comics.Editor/Models/TranslateAnim.cs` - Position animation
- `app/unity_comics.editor/Comics.Editor/Models/RotateAnim.cs` - Rotation with pivot
- `app/unity_comics.editor/Comics.Editor/Models/ScaleAnim.cs` - Scale with pivot
- `app/unity_comics.editor/Comics.Editor/Models/AlphaAnim.cs` - Opacity animation
- `app/unity_comics.editor/Comics.Editor/Models/SoundAnim.cs` - Audio trigger segments
- `app/unity_comics.editor/Comics.Editor/ViewModel/ComicsViewModel.cs` - Scroll orchestration
- `app/unity_comics.editor/Comics.Editor/ViewModel/LayerViewModel.cs` - Layer animation evaluation
- `app/unity_comics.editor/Comics.Editor/ViewModel/SoundViewModel.cs` - Sound playback control
- `app/unity_comics.editor/Comics.Editor/Controls/*AnimControl.xaml` - WPF animation UI

## Validated Understanding

### Architecture Overview

The animation system is **scroll-driven** rather than time-driven. A single `Scroll` property (integer units) drives all animation evaluations across layers and sounds.

### Animation Type Hierarchy

```
Anim (abstract)
├── Start (int) - segment begin scroll position
├── End (int) - segment end scroll position
├── Type (AnimTypes enum)
├── Factor(scroll) - cubic easing function
├── Interpolate(Anim, scroll) - abstract method
│
├── TranslateAnim (X, Y ints)
├── RotateAnim extends PivotAnim (Angle)
├── ScaleAnim extends PivotAnim (ScaleX, ScaleY)
├── AlphaAnim (Alpha 0-1)
├── SoundAnim (no interpolation)
│
└── PivotAnim (abstract)
    ├── PivotX (default 0.5)
    └── PivotY (default 0.5)
```

### Interpolation Mechanics

**Easing Function:**
```csharp
double Factor(double scroll) {
    var t = (scroll - Start) / (End - Start);  // normalize to [0, 1]
    return (--t) * t * t + 1;                  // cubic ease-out
}
```

**Segment Finding Algorithm (FindNearest):**
1. Find `prev` = last completed segment (End <= scroll)
2. Find `curr` = currently active segment (Start < scroll < End)
3. If no `prev`, create default instance via `Init()`
4. If `curr` exists, interpolate between prev and curr

**Interpolation Formula:**
```csharp
interpolated_value = prev_value + (curr_value - prev_value) * Factor(scroll)
```

### Segment Lifecycle

| Operation | Trigger | Implementation |
|-----------|---------|----------------|
| Create | Add button | `Anim.Add<T>()` - clones prev, sets Start=scroll, End=scroll+200 |
| Move | Center drag | Start += delta, End += delta |
| Resize Start | Top drag | Start = min(Start + delta, End) |
| Resize End | Bottom drag | End = max(End + delta, Start) |
| Delete | Delete button | `Layer.Animations.Remove()` |

### Scroll-Driven Playback Flow

```
User scrolls/plays
    ↓
ComicsViewModel.Scroll property set
    ↓
OnPropertyChanged("Scroll")
    ↓
├── foreach LayerViewModel.Scroll()
│   └── Evaluate Translate, Rotate, Scale, Alpha
│       └── Anim.Interpolate<T>() per type
│           └── FindNearest() + prev.Interpolate(curr, scroll)
│
└── foreach SoundViewModel.Scroll()
    └── SoundAnim.FindCurrent(prevScroll, scroll)
        └── Play/Stop based on segment presence
```

### SoundAnim Special Behavior

- **No interpolation** - event-based triggers
- **Point trigger**: Start == End, fires once when scroll crosses point forward
- **Loop segment**: Start < End, loops while within range, stops on exit

```csharp
// FindCurrent criteria:
// Range: Start <= scroll && End >= scroll
// Point: Start == End && prevScroll < scroll && scroll >= Start
```

### WPF UI Implementation

- **Timeline Canvas**: Segments rendered with Canvas.Top and Height bindings
- **Drag Handles**: Thumb controls for resize/move
- **Inspector**: Type-specific property textboxes
- **Focus-Aware Binding**: ScrollConverter shows live scroll when focused

### Unity Implementation Status

- **Model layer**: Complete, parallel to WPF
- **UI layer**: Not implemented (SDD task pending)
- **Key differences**: `List<Anim>` instead of `ObservableCollection<Anim>`

## Children Identified

| Child | Hypothesis | Status |
|-------|------------|--------|
| (none) | Animation system is leaf node | - |

## Dependencies

- **Uses**: document-model (Layer/Sound containers)
- **Used by**: canvas-rendering (applies transforms), audio-system (playback triggers)

## Key Insights

1. **Segment-based, not keyframe-based**: Animation is between prev and curr segments, not individual keyframes
2. **MemberwiseClone pattern**: New segments clone previous values for continuity
3. **Single Scroll broadcasts**: One property change evaluates ALL layer/sound animations
4. **No configurable easing**: Cubic ease-out is hardcoded
5. **SoundAnim is event-based**: No interpolation, just trigger/loop/stop
6. **Focus-aware editing**: TextBox shows live scroll when focused, stored value otherwise

## ADR Candidates

1. **Segment vs. Keyframe model** - Current design is segment-based; keyframes would be architectural change
2. **Easing function configurability** - Currently hardcoded cubic; enum-based selection possible
3. **MemberwiseClone limitations** - Works for value types but not reference properties
4. **Timeline zoom/pan behavior** - Pixel scale multiplier, details undefined
5. **Segment overlap resolution** - No defined policy when segments overlap

## Flow Recommendation

- **Type**: VDD (existing flow: vdd-legacy-animation-timeline)
- **Confidence**: high
- **Rationale**: Existing flow covers requirements; gaps in implementation details

## Flow Updates Required

| Flow | Action | Additions |
|------|--------|-----------|
| vdd-legacy-animation-timeline/03-specifications.md | APPEND | FindNearest algorithm, Factor formula, lifecycle details |
| sdd-unity-animation-timeline-ui/02-specifications.md | APPEND | WPF implementation patterns for reference |

## Synthesis

### Combined Understanding

The animation system is a scroll-driven, segment-based interpolation engine with:
- 5 interpolating types (Translate, Rotate, Scale, Alpha) + 1 event type (Sound)
- Cubic ease-out easing with no configurability
- Factory pattern for segment creation (clone + position)
- Cascading update architecture via single Scroll property

The WPF implementation is complete; Unity has model parity but no UI.

## Bubble Up

- Scroll-driven architecture broadcasts changes to all layers/sounds simultaneously
- Segment-based (not keyframe) - architectural decision affecting future features
- WPF UI fully implemented; Unity UI pending (SDD flow)
- Cubic easing hardcoded; configurability would require ADR
- SoundAnim is event-based with point/loop modes

---

*Phase: SYNTHESIZING | Depth: 2 | Parent: / (root)*
