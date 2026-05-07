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

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
