# Requirements: Unity editor parity — gaps & simplifications (overview)

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08

## Problem Statement

The Unity **Comics Editor** window is a first port of the legacy **WPF Comics.Editor**. Stakeholders need a single place that lists **what is not implemented yet** and **what was intentionally or accidentally simplified**, so work can be scoped without re-deriving the diff from memory.

## User Stories

**As a** product owner / tech lead  
**I want** a traceable backlog of parity items  
**So that** we can schedule child specs and measure “done” against WPF behavior

**As a** developer  
**I want** each major gap in its own SDD flow  
**So that** implementation and review stay focused

## Acceptance Criteria

1. **Given** this overview  
   **When** someone plans the next sprint  
   **Then** they can map each row to exactly one child `flows/sdd-unity-*` flow (or mark “out of scope”)

2. **Given** a parity topic (e.g. audio)  
   **When** they open the linked SDD flow  
   **Then** they find requirements → specs → plan suitable for `/sdd` execution

3. **Given** the WPF app’s feature set  
   **When** comparing to Unity v1  
   **Then** no major subsystem is missing from the matrix below (may be explicitly deferred)

## Parity matrix (WPF → Unity v1)

| Area | WPF behavior | Unity v1 | Gap type | Child SDD |
|------|----------------|----------|----------|------------|
| Shell | MainWindow, Comics vs Puzzle routes | Toolbar: New Comics/Puzzle, Open, Save | Partial | — |
| Zip I/O | `7za.exe` pack/unpack | `System.IO.Compression.ZipFile` | Simplified (behavior/compat) | `sdd-unity-asset-pipeline-fidelity` |
| Temp workspace | `%LocalAppData%\Comics Editor\Temp` | `Application.temporaryCachePath/ComicsUnityEditor` | Different path | `sdd-unity-asset-pipeline-fidelity` |
| Tiling | ImageMagick resize/crop naming | Texture2D + RenderTexture | Simplified (fidelity) | `sdd-unity-asset-pipeline-fidelity` |
| JSON | Newtonsoft, `TypeNameHandling.Auto` | Same serializer settings (intent) | Must validate on real files | `sdd-unity-asset-pipeline-fidelity` |
| Layer list | ItemsControl + reorder | List + ↑↓ + delete | Partial (no full MVVM binding) | `sdd-unity-animation-timeline-ui` |
| Culture | `CulturesHelper`, per-culture images | Culture dropdown | Partial (no per-field file pick UX) | `sdd-unity-animation-timeline-ui` |
| Preview | `LayersControl` canvas: transforms + selection | Stacked static previews + labels | **Major gap** | `sdd-unity-canvas-preview-transforms` |
| Scroll-driven state | Layer/Sound VMs update from `Scroll` | Scroll slider + interpolation readout | Partial (no sound feedback) | `sdd-unity-canvas-preview-transforms`, `sdd-unity-audio-preview` |
| Animations | Add/delete types, select segment, edit controls | One button: add translate segment | **Major gap** | `sdd-unity-animation-timeline-ui` |
| Sound | `MediaPlayer`, `SoundAnim`, `DisableSound` | Copy mp3 to bundle only | **Missing** | `sdd-unity-audio-preview` |
| Undo/redo | None in legacy — but expected in new stack | None | **Missing** (desired) | `sdd-unity-undo-redo` |
| Convert | `Convert()` retile flat images | Not ported | **Missing** | `sdd-unity-asset-pipeline-fidelity` |
| Popup images | Per-culture popup via `Image.Popup` | Not in UI | **Missing** | `sdd-unity-animation-timeline-ui` |
| Selection / handles | WPF hit-test, adorners | None | **Missing** | `sdd-unity-canvas-preview-transforms` |

## Constraints

- Parity targets **behavior and file compatibility** with existing `.comics`/`.puzzle` where reasonable, not pixel-perfect reproduction of WPF rendering.
- Child flows may depend on each other (canvas ↔ animation evaluation ↔ audio time).

## Open Questions

- [ ] Is **strict** byte-identical zip required, or only extractable by legacy runtime?
- [ ] Target Unity versions LTS only (2022.3) or newer?
- [ ] Is undo/redo mandatory before beta?

## References

- WPF: `app/unity_comics.editor/Comics.Editor/`
- Unity: `app/unity_comics.editor/UnityComicsEditor/Assets/ComicsUnity/Editor/`
- Related VDD: `flows/vdd-legacy-*`

---

## Detailed Feature Matrix - Legacy Additions
> Added by /legacy on 2026-05-08

### Completion Assessment: ~40%

| Aspect | WPF | Unity v1 | Parity % |
|--------|-----|----------|----------|
| Data models | Full | Full | 100% |
| File I/O | 7za + ImageMagick | ZipFile + Texture2D | Unvalidated |
| Preview canvas | Composed transforms | Vertical image list | 20% |
| Animation UI | 4 type editors | 1 button (translate only) | 10% |
| Audio playback | MediaPlayer + scrub | File copy only | 0% |

### Critical Missing Functionality

| Item | WPF Implementation | Unity Status | Risk Level |
|------|-------------------|--------------|------------|
| Composed preview canvas | LayersControl.xaml (Canvas + ItemsControl) | Stacked images, no transforms rendered | CRITICAL |
| Rotate anim CRUD | RotateAnimControl.xaml | Not in UI | CRITICAL |
| Scale anim CRUD | ScaleAnimControl.xaml | Not in UI | CRITICAL |
| Alpha anim CRUD | AlphaAnimControl.xaml | Not in UI | CRITICAL |
| Sound anim CRUD | SoundAnimsControl.xaml | Not in UI | CRITICAL |
| Audio playback | SoundViewModel + MediaPlayer | No playback | HIGH |
| Per-culture image picker | LayerViewModel.ChangeCommand | No UI | HIGH |
| Popup image workflow | Image.Popup + PopupCommand | Not in UI | MEDIUM |
| Zip round-trip validation | 7za tested | Not validated | HIGH |

### Recommended Execution Order (Dependencies)

```
Phase 0 (Foundation - ENGINE REFACTORING):
└── sdd-comics.engine-shared-core (IComicsSource abstraction)
    ├── Extracts: FolderSource from ZipArchiveProvider
    ├── Enables: Editor preview using runtime engine
    └── Shared: AnimationProcessor, TileRenderer

Phase 1 (Editor Preview) - Parallel:
├── sdd-comics.editor-asset-pipeline-fidelity (validates round-trip)
└── sdd-comics.editor-canvas-preview-transforms (visual feedback)
    ├── Uses: shared core AnimationProcessor
    └── sdd-comics.editor-engine-preview (full validation)
        └── Uses: ComicsViewer + FolderSource

Phase 2 (Authoring Core):
└── sdd-comics.editor-animation-timeline-ui (all anim CRUD)
    └── Depends on: canvas preview for feedback

Phase 3 (Media + Safety):
├── sdd-comics.editor-audio-preview (sound playback)
│   └── Depends on: engine preview (sound integration)
└── sdd-comics.editor-undo-redo (crash protection)
    └── Depends on: animation commands finalized
```

### Engine Integration Architecture

```
┌────────────────────────────────────────────────────────────┐
│  comics.editor                                             │
│  ┌─────────────────────┐  ┌────────────────────────────┐  │
│  │ Canvas Preview      │  │ Engine Preview             │  │
│  │ (inline editing)    │  │ ("Preview as Player")      │  │
│  │ - selection handles │  │ - full scroll + audio      │  │
│  │ - instant update    │  │ - runtime validation       │  │
│  └──────────┬──────────┘  └──────────────┬─────────────┘  │
│             │                            │                 │
│             └────────────┬───────────────┘                 │
│                          ▼                                 │
│  ┌───────────────────────────────────────────────────────┐│
│  │  comics.engine (shared core)                          ││
│  │  ┌─────────────────┐  ┌────────────────────────────┐  ││
│  │  │  FolderSource   │  │  AnimationProcessor        │  ││
│  │  │  (editor temp)  │  │  (Scale→Rotate→Translate)  │  ││
│  │  └─────────────────┘  └────────────────────────────┘  ││
│  │  ┌─────────────────┐  ┌────────────────────────────┐  ││
│  │  │  ZipArchiveSource│  │  TileRenderer             │  ││
│  │  │  (runtime)      │  │  (TILE_SIZE=512)           │  ││
│  │  └─────────────────┘  └────────────────────────────┘  ││
│  └───────────────────────────────────────────────────────┘│
└────────────────────────────────────────────────────────────┘
```

### Architecture Differences

| Aspect | WPF | Unity |
|--------|-----|-------|
| UI framework | XAML + MVVM | IMGUI (OnGUI) |
| ViewModel layer | Full (LayerViewModel, SoundViewModel) | None (direct session) |
| Transform rendering | RenderTransform at multiple DOM levels | Not rendered (labels only) |
| Audio integration | MediaPlayer with IDisposable lifecycle | N/A |

### Open Design Questions (from analysis)

1. **IMGUI vs UIToolkit**: For timeline UI, which framework?
2. **Audio API choice**: Unity Editor audio vs AudioClip import?
3. **Undo architecture**: Command pattern vs snapshots?
4. **Compatibility level**: Byte-for-byte zip vs functional equivalence?

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
