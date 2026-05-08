# Legacy Analysis Status

## Mode

- **Current**: COMPLETE
- **Type**: BFS (full project analysis)

## Source

- **Path**: [project root]
- **Focus**: [none]

## Traversal State

> See _traverse.md for full recursion stack

- **Current Node**: / (root)
- **Current Phase**: EXITING (complete)
- **Stack Depth**: 0
- **Pending Children**: 0

## Progress

- [x] Root node created
- [x] Initial domains identified
- [x] Recursive traversal in progress
- [x] All nodes synthesized
- [x] Flows updated (5 flows with Legacy Additions)
- [x] ADRs created (9 ADRs in DRAFT status)
- [x] Review list complete

## Statistics

- **Nodes created**: 6 (root + 5 domains)
- **Nodes completed**: 6
- **Max depth reached**: 2
- **Flows updated**: 5
- **ADRs created**: 9 (all DRAFT)
- **Pending review**: 0

## Flows Updated

| Flow | Action | Key Additions |
|------|--------|---------------|
| vdd-legacy-format/03-specifications.md | APPEND | Animation hierarchy, serialization details, culture mismatch |
| vdd-legacy-animation-timeline/03-specifications.md | APPEND | FindNearest algorithm, Factor formula, lifecycle details |
| vdd-legacy-workspace-packaging/03-specifications.md | APPEND | Temp paths, cleanup retry, atomic save gap |
| vdd-legacy-rendering/03-specifications.md | APPEND | Transform order, tile assembly, hit-testing gaps |
| sdd-unity-parity-gaps-overview/01-requirements.md | APPEND | Detailed feature matrix, prioritized execution order |

## ADR Candidates

| ADR | Domain | Priority |
|-----|--------|----------|
| TypeNameHandling.Auto vs explicit discriminator | document-model | High |
| Culture enum expansion (Hi) | document-model | Medium |
| ObservableCollection vs List standardization | document-model | Medium |
| Atomic save strategy | file-format | High |
| Schema versioning | file-format | High |
| Transform composition order | canvas-rendering | High |
| Hit-testing implementation | canvas-rendering | Medium |
| Easing function configurability | animation-system | Low |
| IMGUI vs UIToolkit | unity-port | Medium |

## Last Action

2026-05-08: Created refactoring SDDs for comics.engine shared core and editor integration.

## New Flows Created

| Flow | Purpose |
|------|---------|
| `sdd-comics.engine-shared-core` | IComicsSource abstraction for runtime/editor |
| `sdd-comics.editor-engine-preview` | "Preview as Player" validation mode |

## Updated Flows

| Flow | Change |
|------|--------|
| `sdd-comics.engine-csharp-unity` | Added shared core refactoring section |
| `sdd-comics.editor-canvas-preview-transforms` | Added engine integration architecture |

## Next Action

1. Create ADRs from candidates (optional)
2. Continue with SDD implementations per prioritization:

```
Phase 0 (Foundation - NEW):
└── sdd-comics.engine-shared-core (IComicsSource abstraction)
    └── Enables: canvas preview + engine preview

Phase 1 (Editor Preview):
├── sdd-comics.editor-asset-pipeline-fidelity (validates round-trip)
└── sdd-comics.editor-canvas-preview-transforms (uses shared core)
    └── sdd-comics.editor-engine-preview (full validation)

Phase 2 (Authoring Core):
└── sdd-comics.editor-animation-timeline-ui (all anim CRUD)
    └── Depends on: canvas preview for feedback

Phase 3 (Media + Safety):
├── sdd-comics.editor-audio-preview (sound playback)
│   └── Depends on: engine preview (sound integration)
└── sdd-comics.editor-undo-redo (crash protection)
```

---

*Updated by Claude on 2026-05-08*
