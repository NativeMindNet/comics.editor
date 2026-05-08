# Traversal State

> Persistent recursion stack for tree traversal. AI reads this to know where it is and what to do next.

## Existing Flows Index

| Flow Path | Type | Topics | Key Decisions | Updated |
|-----------|------|--------|---------------|---------|
| flows/vdd-legacy-format/ | VDD | document schema, .comics/.puzzle format, zip, json, culture mapping | Versioned schema, backward compat | 2026-05-08 |
| flows/vdd-legacy-image-pipeline/ | VDD | tiling, ImageMagick, preview rendering, multi-scale pyramids | Cross-platform tiling, perf budgets | - |
| flows/vdd-legacy-rendering/ | VDD | WPF Canvas, transforms, scene graph, hit-testing, tile streaming | Flutter scene graph, caching | 2026-05-08 |
| flows/vdd-legacy-undo-redo/ | VDD | undo/redo, command pattern, asset operations | Command vs snapshot architecture | - |
| flows/vdd-legacy-audio/ | VDD | audio preview, SoundAnim, MediaPlayer, mixing | Audio backend, latency targets | - |
| flows/vdd-legacy-animation-timeline/ | VDD | animation segments, timeline UI, interpolation | Segment vs keyframe model | 2026-05-08 |
| flows/vdd-legacy-workspace-packaging/ | VDD | workspace, temp directory, 7za.exe, atomic save | Cross-platform packaging, no CLI deps | 2026-05-08 |
| flows/sdd-unity-parity-gaps-overview/ | SDD | Unity port, WPF parity, gap analysis | Parent flow for child SDDs | 2026-05-08 |
| flows/sdd-unity-asset-pipeline-fidelity/ | SDD | Unity ZipFile, tile generation, PNG vs JPEG | Acceptance tests for parity | - |
| flows/sdd-unity-canvas-preview-transforms/ | SDD | Unity canvas, transforms, pivot, selection | Transform composition order | - |
| flows/sdd-unity-animation-timeline-ui/ | SDD | Unity timeline, anim CRUD, IMGUI vs UIToolkit | UI framework choice | - |
| flows/sdd-unity-audio-preview/ | SDD | Unity audio, MP3, SoundAnim | Audio API choice | - |
| flows/sdd-unity-undo-redo/ | SDD | Unity undo, command taxonomy, asset history | Command stack vs snapshots | - |

## Mode

- **BFS** (no comment): Breadth-first, analyze all domains systematically

## Source Path

[project root]

## Focus (DFS only)

[none]

## Current Stack

> Read top-to-bottom = root-to-current. Last item = where AI is now.

```
/ (root)                           DONE
├── document-model                 DONE (SYNTHESIZING)
├── animation-system               DONE (SYNTHESIZING)
├── file-format                    DONE (SYNTHESIZING)
├── canvas-rendering               DONE (SYNTHESIZING)
└── unity-port                     DONE (SYNTHESIZING)
```

## Stack Operations Log

| # | Operation | Node | Phase | Result |
|---|-----------|------|-------|--------|
| 1 | PUSH | / (root) | ENTERING | Started BFS traversal |
| 2 | PHASE | / (root) | EXPLORING | Scanned project structure, identified 9 domains |
| 3 | PHASE | / (root) | SPAWNING | Created 5 child domains for deep analysis |
| 4 | RECURSE | document-model | SYNTHESIZING | Model hierarchy, serialization, culture handling |
| 5 | RECURSE | animation-system | SYNTHESIZING | Interpolation, easing, segment lifecycle |
| 6 | RECURSE | file-format | SYNTHESIZING | ZIP bundles, workspace, external tools |
| 7 | RECURSE | canvas-rendering | SYNTHESIZING | Transform composition, tile assembly, hit-testing |
| 8 | RECURSE | unity-port | SYNTHESIZING | Parity gaps, child SDD status |
| 9 | PHASE | / (root) | EXITING | All children complete, synthesis done |

## Current Position

- **Node**: / (root)
- **Phase**: EXITING (complete)
- **Depth**: 1
- **Path**: /

## Pending Children

> Children identified but not yet explored (LIFO - last added explored first)

```
[all complete]
```

## Visited Nodes

> Completed nodes with their summaries

| Node Path | Summary | Flow Updated |
|-----------|---------|--------------|
| /document-model | Model hierarchy, serialization, culture mismatch (2 vs 3) | vdd-legacy-format |
| /animation-system | Scroll-driven, segment-based, cubic ease-out | vdd-legacy-animation-timeline |
| /file-format | ZIP + data.json, no atomic save, external tools on WPF | vdd-legacy-workspace-packaging |
| /canvas-rendering | Transforms at multiple DOM levels, broken hit-testing | vdd-legacy-rendering |
| /unity-port | ~40% complete, canvas/anim-UI/audio are critical gaps | sdd-unity-parity-gaps-overview |

## Next Action

```
BFS TRAVERSAL COMPLETE

All domains analyzed. 5 flows updated with Legacy Additions.
9 ADR candidates identified.
Unity port prioritization recommended: asset-fidelity + canvas-preview first.

To continue: Run /legacy again to resume or start new analysis.
```

---

## Phase Definitions

### ENTERING
- Just arrived at this node
- Create _node.md file
- Read relevant source files
- Form initial hypothesis

### EXPLORING
- Deep analysis of this node's scope
- Validate/refine hypothesis
- Identify what belongs here vs. children

### SPAWNING
- Identify child concepts that need deeper exploration
- Add children to Pending stack
- Children are LOGICAL concepts, not filesystem paths

### SYNTHESIZING
- All children completed (or no children)
- Combine insights from children
- Update this node's _node.md with full understanding

### EXITING
- Pop from stack
- Bubble up summary to parent
- Mark as visited

---

## Resume Protocol

When `/legacy` starts:
1. Read _traverse.md
2. Find current position (top of stack)
3. Check phase
4. Continue from that phase

If interrupted mid-phase:
- Re-enter same phase (idempotent operations)

---

*Updated by /legacy recursive traversal on 2026-05-08*
