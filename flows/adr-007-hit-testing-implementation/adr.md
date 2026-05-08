# ADR-007: Hit-Testing Implementation

## Meta

- **Number**: ADR-007
- **Type**: enabling
- **Status**: DRAFT
- **Created**: 2026-05-08
- **Decided**: -
- **Author**: /legacy analysis
- **Reviewers**: -

## Context

Current WPF hit-testing is **broken**:

```csharp
private void Button_Click(object sender, RoutedEventArgs e)
{
    var btn = sender as ToggleButton;
    Model.SelectedItem = btn?.DataContext as LayerViewModel;
}
```

**Problems**:
1. Tests against axis-aligned bounds, not transformed shape
2. A rotated rectangle's click area remains rectangular (incorrect)
3. No inverse transform applied to click point
4. Overlapping layers selected by ToggleButton tap order (inconsistent)

**Expected behavior**:
- Click point should be transformed into layer's local coordinate space
- Hit test against layer's local bounds
- Top-most layer (by z-order) should be selected first

## Decision Drivers

- **Correctness**: Click on rotated layer should select correctly
- **Determinism**: Same click always selects same layer
- **Performance**: Must handle 50+ layers without lag
- **Extensibility**: Support future selection modes (lasso, marquee)

## Considered Options

### Option 1: Inverse Transform Hit-Testing

**Description**: For each click, apply inverse transform and test in local space:

```csharp
foreach (var layer in Layers.Reverse())  // Front to back
{
    var localPoint = layer.InverseTransform(screenPoint);
    if (layer.LocalBounds.Contains(localPoint))
        return layer;
}
return null;
```

**Pros**:
- Mathematically correct
- Works for any transform
- Standard graphics approach

**Cons**:
- Must compute inverse matrix per layer
- Complex for nested transforms

**Estimated Effort**: Medium

### Option 2: Pixel-Perfect Hit-Testing

**Description**: Sample the rendered pixel at click point; decode layer ID from color.

```csharp
// Render each layer with unique color ID
var hitBuffer = RenderToIDBuffer();
var layerId = hitBuffer.Sample(screenPoint);
```

**Pros**:
- Handles any shape (not just rectangles)
- Handles transparency

**Cons**:
- Requires additional render pass
- GPU memory for ID buffer
- Overkill for rectangles

**Estimated Effort**: High

### Option 3: Axis-Aligned Bounding Box (AABB) with Overlap Sort

**Description**: Test against transformed AABB, sort overlaps by z-order:

```csharp
var candidates = Layers
    .Where(l => l.TransformedBounds.Contains(screenPoint))
    .OrderByDescending(l => l.ZIndex);
return candidates.FirstOrDefault();
```

**Pros**:
- Simple to implement
- Fast (AABB test is cheap)

**Cons**:
- **Incorrect** for rotated layers (AABB is larger than visual)
- False positives in corner gaps

**Estimated Effort**: Low

### Option 4: Oriented Bounding Box (OBB) Test

**Description**: Test against rotated rectangle (OBB) in screen space:

```csharp
var obb = layer.GetOrientedBounds();  // 4 corners
if (obb.ContainsPoint(screenPoint))
    return layer;
```

**Pros**:
- Correct for rotated rectangles
- No inverse transform needed
- Fast (2D polygon test)

**Cons**:
- More complex than AABB
- Doesn't handle scale pivot correctly

**Estimated Effort**: Medium

## Decision

**[PENDING DECISION]**

Recommended: **Option 1 (Inverse Transform)** for correctness, optimized with spatial partitioning if performance becomes an issue.

For initial implementation:
1. Iterate layers front-to-back (z-order)
2. Apply inverse transform to click point
3. Test against local bounds (0, 0, width, height)
4. Return first hit

## Consequences

### Positive

- Correct selection for all transforms
- Foundation for selection handles
- Works with any transform composition

### Negative

- Must maintain inverse transform matrix
- Slightly more computation per click

### Neutral

- Same UX as expected (click selects visible layer)

## Implementation Notes

- Cache inverse matrix when transform changes (not per-click)
- Use `Matrix3x2.Invert()` or equivalent
- Consider spatial index (quadtree) if layer count > 100
- Add debug visualization for hit bounds

**Algorithm**:

```csharp
Matrix3x2 GetLayerTransform(Layer layer)
{
    var t = Matrix3x2.CreateTranslation(layer.Translate.X, layer.Translate.Y);
    var r = Matrix3x2.CreateRotation(layer.Rotate.Angle, layer.RotatePivot);
    var s = Matrix3x2.CreateScale(layer.Scale.X, layer.Scale.Y, layer.ScalePivot);
    return t * r * s;  // Composition order per ADR-006
}

Layer HitTest(Point2D screenPoint)
{
    foreach (var layer in Layers.OrderByDescending(z => z.Index))
    {
        var inverse = Matrix3x2.Invert(GetLayerTransform(layer));
        var localPoint = inverse.Transform(screenPoint);

        if (localPoint.X >= 0 && localPoint.X < layer.Width &&
            localPoint.Y >= 0 && localPoint.Y < layer.Height)
            return layer;
    }
    return null;
}
```

## Related Decisions

- ADR-006: Transform Composition Order (required for inverse calculation)

## Related Specs

- `flows/vdd-legacy-rendering/`: Rendering and interaction
- `flows/sdd-unity-canvas-preview-transforms/`: Unity canvas implementation

## References

- 2D Collision Detection: https://developer.mozilla.org/en-US/docs/Games/Techniques/2D_collision_detection
- Matrix inversion: https://en.wikipedia.org/wiki/Invertible_matrix

## Tags

interaction rendering selection

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
