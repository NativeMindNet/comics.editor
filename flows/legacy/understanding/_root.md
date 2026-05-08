# Understanding: Project Root

> Entry point for recursive understanding. Children are top-level logical domains.

## Phase: EXPLORING

## Project Overview

**Comics.Editor** is a hybrid multi-platform editor application for creating interactive comics and puzzles. The project involves:

1. **Legacy WPF Editor** (`app/unity_comics.editor/Comics.Editor/`) - Original .NET 4.5.2 desktop application with ~56 C# files and ~40 XAML controls
2. **Unity Editor Port** (`app/unity_comics.editor/UnityComicsEditor/`) - Active development target using Unity 2022.3 LTS as an Editor extension
3. **Shared Core** (`app/unity_comics.editor/Comics.Core/`) - C# data models shared between implementations

The project uses a documentation-driven development approach with explicit flow types (VDD, SDD, PDD, DDD, TDD, ADR) to track feature implementation and architectural decisions.

## Identified Domains

> Logical domains discovered. Each becomes a child directory for deeper exploration.

| Domain | Hypothesis | Priority | Status |
|--------|------------|----------|--------|
| document-model | Core data structures: ComicsDocument, LayerModel, ImageModel, SoundModel, animation types | HIGH | PENDING |
| animation-system | 6 animation types with segment-based timeline (Translate, Rotate, Scale, Alpha, Pivot, Sound) | HIGH | PENDING |
| file-format | .comics/.puzzle ZIP bundles with data.json + layers/ + sounds/ | HIGH | PENDING |
| image-pipeline | Tiling, preview generation, multi-scale pyramids; legacy uses ImageMagick | MEDIUM | PENDING |
| workspace-management | Temp directories, atomic save, unpacking/packing bundles | MEDIUM | PENDING |
| audio-system | Sound import, SoundAnim segments, preview playback | MEDIUM | PENDING |
| canvas-rendering | Layer composition, transforms, z-order, hit-testing | HIGH | PENDING |
| undo-redo | Missing in legacy; new feature for Unity port | MEDIUM | PENDING |
| unity-port | Unity Editor extension implementation, parity gaps with legacy | HIGH | PENDING |

## Source Mapping

> Which source paths map to which logical domains

| Source Path | -> Domain |
|-------------|----------|
| `app/unity_comics.editor/Comics.Core/` | document-model, animation-system |
| `app/unity_comics.editor/Comics.Editor/Models/` | document-model |
| `app/unity_comics.editor/Comics.Editor/ViewModel/` | canvas-rendering, animation-system |
| `app/unity_comics.editor/Comics.Editor/Controls/` | canvas-rendering, animation-system (UI) |
| `app/unity_comics.editor/Comics.Editor/IWS/` | workspace-management, file-format |
| `app/unity_comics.editor/UnityComicsEditor/Assets/ComicsUnity/Editor/` | unity-port |
| `flows/vdd-legacy-*/` | Existing reverse-engineering analysis for each domain |
| `flows/sdd-unity-*/` | Unity port specifications for each domain |

## Cross-Cutting Concerns

> Things that span multiple domains (may become ADRs)

- **Serialization Strategy**: Newtonsoft JSON with TypeNameHandling.Auto for polymorphic types
- **External Tool Dependencies**: Legacy uses 7za.exe (ZIP) and magick.exe (ImageMagick); Unity port eliminates these
- **Culture/Localization**: Index-based mapping for cultures, potentially brittle
- **Property Change Notifications**: INotifyPropertyChanged pattern throughout models
- **Coordinate Systems**: Transforms use pivot normalization; composition order matters

## Existing Flow Coverage

> Domains already covered by existing flows

| Domain | Existing Flow(s) | Coverage |
|--------|-----------------|----------|
| document-model | vdd-legacy-format | Partial (schema focus) |
| animation-system | vdd-legacy-animation-timeline, sdd-unity-animation-timeline-ui | Good |
| file-format | vdd-legacy-format, vdd-legacy-workspace-packaging | Good |
| image-pipeline | vdd-legacy-image-pipeline, sdd-unity-asset-pipeline-fidelity | Good |
| workspace-management | vdd-legacy-workspace-packaging | Partial |
| audio-system | vdd-legacy-audio, sdd-unity-audio-preview | Good |
| canvas-rendering | vdd-legacy-rendering, sdd-unity-canvas-preview-transforms | Good |
| undo-redo | vdd-legacy-undo-redo, sdd-unity-undo-redo | Good |
| unity-port | sdd-unity-parity-gaps-overview + child SDDs | Comprehensive |

## Children Spawned

```
document-model/     DONE - Model hierarchy, serialization, culture handling
animation-system/   DONE - Interpolation, easing, segment lifecycle
file-format/        DONE - ZIP bundles, workspace, external tools
canvas-rendering/   DONE - Transform composition, tile assembly, hit-testing
unity-port/         DONE - Parity gaps, child SDD status
```

## Synthesis

> Updated 2026-05-08 after all children complete

### Key Findings

1. **Document Model**: Three-layer architecture (Core/Editor/Unity) with subtle differences (ObservableCollection vs List). Culture enum mismatch (2 vs 3). TypeNameHandling.Auto serialization is fragile.

2. **Animation System**: Scroll-driven, segment-based (not keyframe). Cubic ease-out hardcoded. WPF fully implements all 5 types + UI; Unity only has TranslateAnim button.

3. **File Format**: ZIP bundles with data.json + tiles. WPF uses external tools (7za, ImageMagick); Unity uses built-in APIs. No atomic save or schema versioning.

4. **Canvas Rendering**: WPF applies transforms at multiple DOM levels with separate pivots for rotate/scale. Hit-testing is broken (no inverse transform). Unity has no composed canvas (vertical list only).

5. **Unity Port**: ~40% complete. Data models done; UI incomplete. Critical gaps: composed canvas, animation CRUD, audio playback.

### ADR Candidates Identified

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

### Flow Updates Applied

| Flow | Type | Additions |
|------|------|-----------|
| vdd-legacy-format/03-specifications.md | APPEND | Animation hierarchy, serialization details, culture mismatch |
| vdd-legacy-animation-timeline/03-specifications.md | APPEND | FindNearest algorithm, Factor formula, lifecycle details |
| vdd-legacy-workspace-packaging/03-specifications.md | APPEND | Temp paths, cleanup retry, atomic save gap |
| vdd-legacy-rendering/03-specifications.md | APPEND | Transform order, tile assembly, hit-testing gaps |
| sdd-unity-parity-gaps-overview/01-requirements.md | APPEND | Detailed feature matrix, prioritized execution order |

### Recommendations

**Phase 1 (Foundation):**
- Complete sdd-unity-asset-pipeline-fidelity (validate round-trip)
- Complete sdd-unity-canvas-preview-transforms (visual feedback)

**Phase 2 (Authoring Core):**
- Complete sdd-unity-animation-timeline-ui (all anim CRUD)

**Phase 3 (Media + Safety):**
- Complete sdd-unity-audio-preview (sound playback)
- Complete sdd-unity-undo-redo (crash protection)

---

*Created by /legacy ENTERING phase on 2026-05-08*
