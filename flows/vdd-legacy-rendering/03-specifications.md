# Specifications: rendering & interaction engine (legacy parity)

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Requirements: [link to 01-requirements.md]

## Overview

Define the Flutter rendering and interaction architecture to support:
- layer compositing (z-order)
- transforms (translate/rotate/scale/opacity, optional pivot)
- deterministic hit testing
- tile streaming for large assets

## Affected Systems

| System | Impact | Notes |
|--------|--------|-------|
| Scene graph / layer model | Create/Modify | runtime representation for rendering + editing |
| Renderer | Create | draws layers + tiles + selection overlay |
| Hit testing | Create | deterministic selection rules |
| Interaction tools | Create | select/move/rotate/scale modes |
| Diagnostics | Create | tile loading status, perf counters (optional) |

## Architecture

### Component Diagram

```
[DocumentModel] -> [SceneGraph] -> [Renderer]
        |              |             |
        |              v             v
        |         [Hit Tester]   [Tile Resolver/Cache]
        v
   [Timeline/Anims] -> (evaluates) -> [EffectiveTransform/Opacity]
```

### Data Flow

```
input events -> tool state machine -> mutate model -> render invalidation -> draw
render -> compute visible tiles -> resolve/decode -> draw -> cache
```

## Interfaces

### New Interfaces (conceptual)

```cpp
interface SceneRenderer {
  void draw(SceneGraph scene, Viewport viewport);
}

interface HitTester {
  HitResult hitTest(SceneGraph scene, Viewport viewport, Point p);
}

interface ToolController {
  void onPointerEvent(PointerEvent e);
}
```

## Data Models

### New Types (conceptual)

```cpp
struct SceneNode {
  string id;
  Transform2D transform;
  double opacity;
  Drawable drawable; // image tiles, solid, text, etc.
}

struct HitResult {
  string? nodeId;
  // include local coords if needed for handles
}
```

## Behavior Specifications

### Hit Testing Rules (determinism)

1. Evaluate from topmost z-order to bottom.
2. Use transformed bounds in screen space.
3. If multiple overlap, pick first match (stable sort).
4. Optional: alpha-aware mode later (not required now).

### Interaction

- Select tool: pointer down selects; pointer down on empty clears.
- Move: drag updates translate; commit on pointer up as single action (undo-friendly).
- Rotate/scale: uses handles; pivot rules must be defined and consistent.

### Error Handling

| Error | Cause | Response |
|-------|-------|----------|
| Missing tile | asset not found | render transparent for that tile + warning |
| Decode failure | corrupt tile | mark tile bad; allow continue |

## Dependencies

- Tile resolver/cache (see `vdd-legacy-image-pipeline`) or equivalent subsystem.
- Document model loader (see `vdd-legacy-format`).

## Testing Strategy

### Unit Tests

- [ ] Transform composition correctness
- [ ] Hit testing determinism on overlaps

### Integration Tests

- [ ] Pan/zoom with tiled layers stays responsive
- [ ] Selection handles align with rendered bounds

## Migration / Rollout

- Start with parity: basic transforms + selection + tiled images.
- Add advanced tools later (snapping, multi-select, alpha hit testing).

## Open Design Questions

- [ ] Choose exact rendering strategy (retained vs immediate).
- [ ] Pivot representation (stored vs derived).

---

## WPF Implementation Details - Legacy Additions
> Added by /legacy on 2026-05-08

### Transform Composition Order (WPF)

WPF applies transforms at **multiple DOM levels**, which differs from a single matrix multiplication:

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

**Effective order:** Translate → Rotate (around pivot) → Scale (around separate pivot) → Alpha

**WARNING:** Rotation and scale have **separate pivots** at different DOM levels. New implementation should decide: unified pivot vs. per-transform pivots.

### Tile Assembly Strategy

**WPF (ImagePathConverter):**
```csharp
private ImageSource TileImage(string folder, Image image)
{
    var fileName = string.Format(image.File, scale, "*", "*");
    var visual = new DrawingVisual();

    using (var context = visual.RenderOpen())
    {
        foreach (var file in Directory.GetFiles(folder, fileName))
        {
            // Parse col, row from filename
            var x = col * FileManager.TileSize;  // 512
            var y = row * FileManager.TileSize;
            context.DrawImage(LoadBitmap(file), new Rect(x, y, w, h));
        }
    }

    // Render at HALF resolution for performance
    var bmp = new RenderTargetBitmap(
        image.Width / 2, image.Height / 2,
        48, 48, PixelFormats.Pbgra32);
    bmp.Render(visual);
    return BitmapToPng(bmp);
}
```

**Key details:**
- Tiles stitched to single composite texture
- Rendered at **50% resolution** (TileScale = 2 divisor)
- Cached as PNG in memory
- No lazy tile loading

### Hit Testing Gaps (WPF)

**Current implementation is BROKEN:**
```csharp
private void Button_Click(object sender, RoutedEventArgs e)
{
    var btn = sender as ToggleButton;
    Model.SelectedItem = btn?.IsChecked == true ?
        btn?.DataContext as LayerViewModel : null;
}
```

**Problems:**
1. No inverse transform applied
2. Tests against axis-aligned bounds, not transformed shape
3. Rotated rectangle's click area remains rectangular (incorrect)

**Correct approach:**
1. Convert screen point to document coordinates
2. For each layer (front-to-back):
   - Apply inverse of layer's transform matrix
   - Test point against layer's local bounds
   - Return first hit

### Z-Order Semantics

- `Layers[0]` = drawn first (back)
- `Layers[N]` = drawn last (front)
- Order from document model; no runtime z-index changes
- WPF ItemsControl renders in list order automatically

### Scroll vs. Viewport Pan

**IMPORTANT:** The `Scroll` property is NOT viewport pan.

```csharp
public double Scroll
{
    set
    {
        _scroll = value;
        foreach (var layer in Layers)
            layer.Scroll();  // Recalculate animation state
        foreach (var sound in Sounds)
            sound.Scroll();  // Update audio playback
    }
}
```

**Scroll** = timeline position (0-12000 range)
**Viewport pan** = handled by ScrollViewer/Viewbox separately

Consider renaming to `TimelinePosition` to avoid confusion.

### Layer Visibility

```csharp
public Visibility Visibility => IsVisible ? Visibility.Visible : Visibility.Collapsed;
```

- `Visibility.Collapsed` = completely hidden (no render)
- `Alpha.Alpha` = opacity (0.0-1.0) for visible layers only
- Visibility and alpha are independent; layer can be hidden while alpha animation plays

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
