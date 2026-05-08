# ADR-006: Transform Composition Order

## Meta

- **Number**: ADR-006
- **Type**: constraining
- **Status**: DRAFT
- **Created**: 2026-05-08
- **Decided**: -
- **Author**: /legacy analysis
- **Reviewers**: -

## Context

The WPF implementation applies transforms at **multiple DOM levels**:

```xml
<ContentPresenter Canvas.Left="{Binding Translate.X}"
                  Canvas.Top="{Binding Translate.Y}"
                  RenderTransformOrigin="{Binding Rotate.Pivot}">
  <RotateTransform Angle="{Binding Rotate.Angle}"/>

  <Grid RenderTransformOrigin="{Binding Scale.Pivot}">
    <Image Opacity="{Binding Alpha.Alpha}"/>
    <Grid.RenderTransform>
      <ScaleTransform ScaleX="{Binding Scale.ScaleX}"
                      ScaleY="{Binding Scale.ScaleY}"/>
    </Grid.RenderTransform>
  </Grid>
</ContentPresenter>
```

**Problem**:
1. Rotation and scale have **separate pivot points** at different DOM levels
2. The composition order is: Translate → Rotate (around pivot) → Scale (around separate pivot)
3. This is **not equivalent** to a single matrix with unified pivot
4. Unity port needs to replicate this exact behavior, but it's not mathematically obvious

**Example mismatch**: If rotate pivot = (0.5, 0.5) and scale pivot = (0, 0), the visual result differs from a single matrix that applies scale then rotate around (0.5, 0.5).

## Decision Drivers

- **Visual parity**: Unity must match WPF pixel-for-pixel (within tolerance)
- **Mathematical clarity**: Developers need to understand the transform math
- **Future portability**: Flutter/other renderers need clear specification
- **Simplicity**: Prefer single unified approach if possible

## Considered Options

### Option 1: Replicate WPF Multi-Level Transforms

**Description**: Unity applies transforms in same order with separate pivots.

**Pros**:
- Exact WPF parity
- No migration needed

**Cons**:
- Complex to implement correctly
- Separate pivots are confusing
- No single matrix representation

**Estimated Effort**: Medium

### Option 2: Unified Pivot for All Transforms

**Description**: Single pivot shared by rotate and scale:

```csharp
class LayerTransform {
    Point2D Pivot;      // Shared
    double Angle;
    double ScaleX, ScaleY;
    Point2D Translate;
    double Alpha;
}
```

Composition: Translate → Scale around Pivot → Rotate around Pivot

**Pros**:
- Simpler mental model
- Single matrix multiplication
- Standard graphics approach

**Cons**:
- **Breaking change**: Existing documents with different pivots render differently
- Migration complexity

**Estimated Effort**: Medium + Migration

### Option 3: Explicit Matrix Storage

**Description**: Store pre-computed 3x3 affine matrix per animation segment.

**Pros**:
- No ambiguity
- Portable to any renderer

**Cons**:
- Large storage
- Interpolation of matrices is complex (need decompose → interpolate → recompose)
- Loses semantic meaning

**Estimated Effort**: High

### Option 4: Document and Lock Current Behavior

**Description**: Keep WPF behavior as-is; document the exact math for porters:

```
Final position =
  Translate(X, Y) *
  Rotate(angle, around RotatePivot) *
  Scale(sx, sy, around ScalePivot) *
  OriginalPoint
```

**Pros**:
- No document changes
- Clear specification
- Implementers can replicate

**Cons**:
- Separate pivots remain confusing
- Each port must understand this carefully

**Estimated Effort**: Low (documentation)

## Decision

**[PENDING DECISION]**

Recommended: **Option 4 (Document and Lock)** for immediate parity, with consideration of **Option 2 (Unified Pivot)** for v2 format.

Key requirement: Write automated tests comparing WPF and Unity renders for sample documents.

## Consequences

### Positive

- Clear specification for all platforms
- Test suite validates parity

### Negative

- Separate pivot complexity remains
- Each port must handle carefully

### Neutral

- Document format unchanged (for Option 4)

## Implementation Notes

- Create `TransformCompositor` utility with documented math
- Add visual diff tests: render in WPF → render in Unity → compare
- Tolerance: ±2px for complex transforms

**Transform Math (for documentation)**:

```
Given:
  T = Translate(X, Y)
  R = Rotate(angle)
  S = Scale(sx, sy)
  Rp = RotatePivot (normalized 0-1)
  Sp = ScalePivot (normalized 0-1)
  W, H = layer dimensions

RotateAroundPivot:
  1. Translate by (-Rp.x * W, -Rp.y * H)
  2. Rotate by angle
  3. Translate by (+Rp.x * W, +Rp.y * H)

ScaleAroundPivot:
  1. Translate by (-Sp.x * W, -Sp.y * H)
  2. Scale by (sx, sy)
  3. Translate by (+Sp.x * W, +Sp.y * H)

Final = T * RotateAroundPivot * ScaleAroundPivot
```

## Related Decisions

- ADR-007: Hit-Testing (needs inverse of this transform)

## Related Specs

- `flows/vdd-legacy-rendering/`: Rendering specification
- `flows/sdd-unity-canvas-preview-transforms/`: Unity implementation

## References

- CSS Transform Order: https://developer.mozilla.org/en-US/docs/Web/CSS/transform
- WPF RenderTransformOrigin: https://learn.microsoft.com/en-us/dotnet/api/system.windows.uielement.rendertransformorigin

## Tags

rendering transforms architecture

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
