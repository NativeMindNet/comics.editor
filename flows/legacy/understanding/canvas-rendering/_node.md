# Understanding: Canvas Rendering

## Phase: SYNTHESIZING

## Hypothesis

WPF Canvas with ItemsControl for layer composition, transforms applied via RenderTransform at multiple DOM levels, preview rendering via tile stitching to half-resolution texture.

## Sources

- `app/unity_comics.editor/Comics.Editor/Controls/LayersControl.xaml` - Layer rendering
- `app/unity_comics.editor/Comics.Editor/Controls/ComicsControl.xaml` - Viewport/scroll
- `app/unity_comics.editor/Comics.Editor/ViewModel/LayerViewModel.cs` - Transform evaluation
- `app/unity_comics.editor/Comics.Editor/ViewModel/ComicsViewModel.cs` - Scroll orchestration
- `app/unity_comics.editor/Comics.Editor/ViewModel/ImagePathConverter.cs` - Tile assembly
- `app/unity_comics.editor/Comics.Editor/ViewModel/PivotConverter.cs` - Pivot calculations
- `app/unity_comics.editor/UnityComicsEditor/Assets/ComicsUnity/Editor/PreviewTextureBuilder.cs`

## Validated Understanding

### Transform Composition Order

**WPF applies transforms at different DOM levels:**

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

**Order:** Canvas position (translate) → Rotation (with pivot) → Scale (with pivot) → Alpha

**Issue:** Rotation and scale have separate pivot points at different DOM levels. This may not match a single matrix multiplication order.

### Tile Assembly Strategy

**WPF (ImagePathConverter):**
1. Find tile files matching pattern `{name}_{scale}_{col}_{row}.ext`
2. Create DrawingVisual canvas
3. Draw each tile at position `(col * 512, row * 512)`
4. Render to RenderTargetBitmap at **half resolution** (divisor = 2)
5. Encode to PNG and cache

**Unity (PreviewTextureBuilder):**
1. Scan for tiles matching pattern
2. Create Texture2D at half resolution
3. Use GetPixels/SetPixels to composite
4. Apply() and return texture

### Z-Order Policy

- `Layers[0]` = back (drawn first)
- `Layers[N]` = front (drawn last)
- Order determined by document layer list order
- No runtime z-order changes in current UI

### Pivot Point Handling

Pivots are **normalized coordinates** (0.0 to 1.0):
- Default: (0.5, 0.5) = center
- Applied via `RenderTransformOrigin` in WPF
- PivotConverter maps normalized to screen-space for handle positioning

**Issue:** Pivots not validated; can be set outside [0, 1] range.

### Viewport & Scroll

**Comics:**
- Viewbox scales uniformly (zoom-to-fit)
- ScrollViewer for pan within scaled content
- Canvas fixed to document dimensions

**Puzzles:**
- Fixed viewport (~400px)
- Zoom controlled by slider (0.125x to 1.0x)
- LayoutTransform for zoom (affects layout, not render)

### Hit Testing

**Current implementation is broken:**
- Uses ToggleButton click on rendered bounds
- No inverse transform applied
- Rotated rectangle's click area remains axis-aligned (incorrect)

## Children Identified

| Child | Hypothesis | Status |
|-------|------------|--------|
| (none) | Canvas rendering is leaf node | - |

## Dependencies

- **Uses**: document-model (layers, images), animation-system (transforms), file-format (tiles)
- **Used by**: unity-port (rendering parity)

## Key Insights

1. **Separate pivots at different DOM levels**: Rotation pivot at ContentPresenter, scale pivot at Grid
2. **Half-resolution preview**: All tiles stitched to 50% size for performance
3. **No tile streaming**: Full composite texture created, not lazy tile loading
4. **Broken hit testing**: Click bounds don't inverse-transform; rotated layers have wrong hit areas
5. **Scroll = timeline, not pan**: "Scroll" property drives animation interpolation, not viewport pan
6. **Z-order is document order**: No runtime z-index manipulation

## ADR Candidates

1. **Transform composition order**: Define single matrix order (scale → rotate around pivot → translate)
2. **Unified pivot handling**: Single pivot for both rotate and scale vs. separate pivots
3. **Tile streaming vs. composite**: Performance tradeoff between lazy loading and pre-composite
4. **Hit testing strategy**: Inverse transform vs. bounding box vs. pixel-perfect
5. **Scroll semantics naming**: Rename to "timelinePosition" to avoid confusion with viewport pan

## Flow Recommendation

- **Type**: VDD (existing flow: vdd-legacy-rendering)
- **Confidence**: high
- **Rationale**: Existing flow covers architecture; gaps in implementation details

## Flow Updates Required

| Flow | Action | Additions |
|------|--------|-----------|
| vdd-legacy-rendering/03-specifications.md | APPEND | Transform composition order, tile assembly, hit testing gaps |
| sdd-unity-canvas-preview-transforms/02-specifications.md | APPEND | WPF reference implementation details |

## Synthesis

### Combined Understanding

Canvas rendering uses a layered ItemsControl with transforms at multiple DOM levels:
- Translate via Canvas.Left/Top
- Rotate via RenderTransform + RenderTransformOrigin
- Scale via nested Grid's RenderTransform + RenderTransformOrigin
- Alpha via Image.Opacity

Preview textures are composited at half resolution. Hit testing is broken for rotated/scaled layers.

## Bubble Up

- Transform composition at multiple DOM levels with separate pivots
- Tile preview at half resolution via DrawingVisual/Texture2D composite
- Z-order is document layer order (no runtime changes)
- Hit testing broken for transformed layers (no inverse transform)
- "Scroll" is timeline position, not viewport pan

---

*Phase: SYNTHESIZING | Depth: 2 | Parent: / (root)*
