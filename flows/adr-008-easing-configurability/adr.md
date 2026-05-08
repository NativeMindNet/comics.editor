# ADR-008: Easing Function Configurability

## Meta

- **Number**: ADR-008
- **Type**: enabling
- **Status**: DRAFT
- **Created**: 2026-05-08
- **Decided**: -
- **Author**: /legacy analysis
- **Reviewers**: -

## Context

The animation system uses a **hardcoded cubic ease-out** function:

```csharp
protected double Factor(double scroll)
{
    var t = (scroll - Start) / (End - Start);
    return (--t) * t * t + 1;  // Cubic ease-out
}
```

**Problem**:
- All animations use same easing (no variety)
- Can't create linear animations
- Can't create bounce, elastic, or other effects
- Not configurable per animation segment

**Current behavior**: Every interpolation uses `(--t) * t * t + 1`, which is a smooth deceleration curve.

## Decision Drivers

- **Creative flexibility**: Animators want different easing for different effects
- **Backward compatibility**: Existing documents must render identically
- **Simplicity**: Don't overengineer for limited use cases
- **Performance**: Easing calculation happens every frame

## Considered Options

### Option 1: Keep Hardcoded (Status Quo)

**Description**: Continue with single cubic ease-out for all animations.

**Pros**:
- No changes required
- Consistent behavior
- Simple codebase

**Cons**:
- No creative flexibility
- Can't do linear or other effects

**Estimated Effort**: None

### Option 2: Easing Enum per Animation

**Description**: Add `Easing` enum property to `Anim` base class:

```csharp
public enum EasingType { Linear, EaseIn, EaseOut, EaseInOut, Bounce, Elastic }

public abstract class Anim
{
    public EasingType Easing { get; set; } = EasingType.EaseOut;

    protected double Factor(double scroll)
    {
        var t = (scroll - Start) / (End - Start);
        return EasingFunctions.Apply(Easing, t);
    }
}
```

**Pros**:
- Configurable per segment
- Backward compatible (default = EaseOut)
- Finite set of options

**Cons**:
- Limited to predefined curves
- Schema change (new property)

**Estimated Effort**: Low

### Option 3: Cubic Bezier Easing

**Description**: Allow custom cubic bezier curves (like CSS):

```csharp
public class Anim
{
    public double[] EasingCurve { get; set; }  // [x1, y1, x2, y2]
}

// Usage: cubic-bezier(0.25, 0.1, 0.25, 1.0)
```

**Pros**:
- Maximum flexibility
- Standard (CSS uses this)
- Covers any smooth curve

**Cons**:
- Complex for users
- Need curve editor UI
- More validation needed

**Estimated Effort**: Medium

### Option 4: Named Presets + Custom

**Description**: Predefined presets with option to define custom:

```csharp
public enum EasingPreset { Linear, EaseOut, EaseIn, ... }

public class Anim
{
    public EasingPreset? Preset { get; set; }
    public double[]? CustomCurve { get; set; }  // Bezier if preset is null
}
```

**Pros**:
- Simple for common cases
- Flexible for advanced users
- Extensible

**Cons**:
- More complex schema
- UI needs both dropdown and curve editor

**Estimated Effort**: Medium

## Decision

**[PENDING DECISION]**

Recommended: **Option 2 (Easing Enum)** for near-term. Low effort, backward compatible, covers most use cases.

If demand exists, upgrade to **Option 3 (Cubic Bezier)** later.

Default to `EaseOut` for missing property (legacy compatibility).

## Consequences

### Positive

- Animators can choose easing style
- Linear animations possible
- Minimal schema change

### Negative

- Limited to predefined curves (for Option 2)
- Need UI for easing selection

### Neutral

- Performance unchanged (switch on enum is fast)

## Implementation Notes

- Add `Easing` property with `[DefaultValue(EaseOut)]` attribute
- Create `EasingFunctions` static class with curve implementations
- Default value ensures legacy documents render identically

**Easing implementations**:

```csharp
public static double Apply(EasingType type, double t)
{
    return type switch
    {
        Linear => t,
        EaseIn => t * t * t,
        EaseOut => (--t) * t * t + 1,
        EaseInOut => t < 0.5
            ? 4 * t * t * t
            : 1 - Math.Pow(-2 * t + 2, 3) / 2,
        Bounce => BounceOut(t),
        Elastic => ElasticOut(t),
        _ => t
    };
}
```

## Related Decisions

- ADR-001: Serialization (new property needs serialization)

## Related Specs

- `flows/vdd-legacy-animation-timeline/`: Animation specification
- `flows/sdd-unity-animation-timeline-ui/`: Animation UI

## References

- CSS Easing Functions: https://easings.net/
- Cubic Bezier: https://cubic-bezier.com/

## Tags

animation ux

---

## Approval

### Review History

| Date | Reviewer | Status | Comments |
|------|----------|--------|----------|
| - | - | pending | - |

### Final Decision

- [ ] Approved by: -
- [ ] Decided on: -
- [ ] Implementation assigned to: -
