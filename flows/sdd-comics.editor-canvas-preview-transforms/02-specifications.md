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

## Engine Integration (Shared Core)

> Added 2026-05-08

### Using comics.engine for Transform Calculation

Instead of duplicating animation logic, the editor should use the same `AnimationProcessor` from `comics.engine`:

```csharp
// In ComicsEditorWindow:
private ComicsViewer _previewViewer;
private FolderSource _folderSource;

private void InitializePreview()
{
    _folderSource = new FolderSource(_tempFolderPath);
    _previewViewer = CreateHiddenViewer();
    _previewViewer.Initialize(_folderSource);
}

private void RenderPreviewCanvas(Rect rect)
{
    // Get transforms from engine (same logic as runtime)
    var transforms = _previewViewer.GetLayerTransforms(_scrollPosition);

    foreach (var (layer, matrix, alpha) in transforms)
    {
        // Apply matrix to GUI/UIToolkit
        DrawLayerWithTransform(rect, layer, matrix, alpha);
    }
}

private void OnDocumentModified()
{
    SaveDataJson();
    _previewViewer.RefreshFromSource();
}
```

### Architecture with Shared Core

```
┌─────────────────────────────────────────────────────────────┐
│  comics.editor (EditorWindow)                               │
│  ┌───────────────────────┐    ┌──────────────────────────┐  │
│  │  ComicsEditorWindow   │    │  IMGUI/UIToolkit Stage   │  │
│  │  - scroll control     │───►│  - DrawLayerWithTransform│  │
│  │  - layer list         │    │  - selection handles     │  │
│  └───────────────────────┘    └──────────────────────────┘  │
│              │                            ▲                  │
│              ▼                            │                  │
│  ┌───────────────────────────────────────┴─────────────────┐│
│  │  comics.engine (Runtime - shared core)                  ││
│  │  ┌─────────────────┐  ┌──────────────────────────────┐  ││
│  │  │  FolderSource   │  │  AnimationProcessor          │  ││
│  │  │  (IComicsSource)│  │  - Process(scroll)           │  ││
│  │  │  - LoadData()   │  │  - GetLayerTransforms()      │  ││
│  │  │  - LoadTile()   │  │  - Scale→Rotate→Translate    │  ││
│  │  │  - Invalidate() │  │  - Easing: (f-1)³+1          │  ││
│  │  └─────────────────┘  └──────────────────────────────┘  ││
│  └─────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────┘
```

### Benefits

| Aspect | Duplicate Logic | Shared Core |
|--------|-----------------|-------------|
| Transform correctness | Risk of drift | Guaranteed match |
| Bug fixes | Must apply twice | Single fix |
| Easing formula | Could differ | Invariant enforced |
| Maintenance | 2 codebases | 1 codebase |

### Dependencies

- `sdd-comics.engine-shared-core`: IComicsSource, FolderSource
- `adr-006-transform-composition-order`: Transform invariants

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
