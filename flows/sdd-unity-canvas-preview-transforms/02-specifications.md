# Specifications: Unity canvas preview with composed transforms

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Requirements: [01-requirements.md](./01-requirements.md)

## Overview

Introduce a **stage renderer** used by `ComicsEditorWindow` (or a dedicated preview host) that evaluates each layer’s anim state at `Scroll` and draws layers back-to-front.

## Affected Systems

| System | Impact |
|--------|--------|
| `ComicsEditorWindow` | Replace stacked thumbnails with stage view (toggle acceptable) |
| New `ComicsStagePreview` (or similar) | Create |
| `PreviewTextureBuilder` | May supply per-layer texture; stage applies transforms |
| Shared evaluators | Reuse same logic as future timeline |

## Architecture

```
Scroll ─► Anim.Interpolate* per layer ─► LayerRenderState (matrix + opacity + texture)
                │
                └─► Stage draws layers[0..n] with GPU or IMGUI matrix stack
```

## Interfaces (conceptual)

```csharp
interface ILayerFrameEvaluator {
  LayerFrameState Evaluate(LayerModel layer, double scroll, Cultures culture, Anim selectedAnim);
}

interface IStageRenderer {
  void Draw(Rect viewport, IReadOnlyList<LayerFrameState> layers);
}
```

## Data models

- `LayerFrameState`: texture (or tile handle), 2D transform matrix, opacity, z-index.

## Edge cases

| Case | Behavior |
|------|----------|
| Missing texture | Skip layer; show warning badge in list |
| Alpha = 0 | Layer invisible but still in hit order (future) |
| Extreme scroll outside anim ranges | Evaluator uses nearest segment rules (same as WPF) |

## Testing Strategy

- [ ] Unit: matrix composition from known anim keyframes vs hand-calculated snapshot.
- [ ] Visual: screenshot compare with WPF for 3 canned scenes (tolerance in pixels).

## Open Design Questions

- [ ] IMGUI `GUI.matrix` vs `Handles`/UIToolkit for rotation clarity.

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
