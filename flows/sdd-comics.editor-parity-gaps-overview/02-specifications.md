# Specifications: Unity parity gaps overview

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Requirements: [01-requirements.md](./01-requirements.md)

## Overview

This document does not specify implementation details; it defines **how child SDD flows divide responsibility** and **ordering heuristics**.

## Affected Systems

| System | Impact |
|--------|--------|
| Documentation / planning | Create |
| Unity Comics Editor | Informed by child specs |

## Architecture

```
[Overview 01-requirements.md — matrix]
        │
        ├── sdd-unity-asset-pipeline-fidelity (foundation)
        ├── sdd-unity-canvas-preview-transforms (visual truth)
        ├── sdd-unity-animation-timeline-ui (authoring)
        ├── sdd-unity-audio-preview (time + sound)
        └── sdd-unity-undo-redo (cross-cutting)
```

## Dependency order (recommended)

1. **Asset pipeline fidelity** — wrong bytes break open/save round-trip.
2. **Canvas preview + transforms** — confirms scroll/evaluator visually.
3. **Animation UI** — depends on canvas feedback loop.
4. **Audio preview** — depends on shared timeline/scroll semantics.
5. **Undo/redo** — wraps commands once tools stabilize.

## Testing Strategy

- [ ] Each child flow defines its own tests; overview only tracks **cross-flow** smoke test: open legacy bundle → save → open in WPF.

## Open Design Questions

- [ ] Which flow owns “single timeline model” shared by canvas and audio?

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
