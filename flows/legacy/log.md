# Legacy Analysis Log

## Session History

### 2026-05-08 - BFS Traversal Complete

**Mode**: BFS
**Target**: project root

**Domains Analyzed**:

1. **document-model** (depth 2)
   - Three-layer architecture (Core/Editor/Unity)
   - ObservableCollection vs List divergence
   - Culture enum mismatch (2 vs 3)
   - TypeNameHandling.Auto serialization risk

2. **animation-system** (depth 2)
   - Scroll-driven, segment-based (not keyframe)
   - Cubic ease-out hardcoded: `(--t) * t * t + 1`
   - FindNearest algorithm for interpolation
   - WPF: 5 anim types + UI; Unity: TranslateAnim only

3. **file-format** (depth 2)
   - ZIP bundles with data.json + layers/ + sounds/
   - WPF: 7za.exe + ImageMagick (external)
   - Unity: ZipFile + Texture2D (built-in)
   - No atomic save; no schema versioning

4. **canvas-rendering** (depth 2)
   - Transforms at multiple DOM levels
   - Separate pivots for rotate/scale
   - Half-resolution preview (TileScale = 2)
   - Hit-testing broken (no inverse transform)

5. **unity-port** (depth 2)
   - ~40% complete
   - Data models done; UI incomplete
   - Critical gaps: canvas, anim CRUD, audio

**Flows Updated**:
- vdd-legacy-format/03-specifications.md
- vdd-legacy-animation-timeline/03-specifications.md
- vdd-legacy-workspace-packaging/03-specifications.md
- vdd-legacy-rendering/03-specifications.md
- sdd-unity-parity-gaps-overview/01-requirements.md

**ADR Candidates Identified**: 9
- TypeNameHandling.Auto vs explicit discriminator (High)
- Culture enum expansion (Medium)
- ObservableCollection vs List standardization (Medium)
- Atomic save strategy (High)
- Schema versioning (High)
- Transform composition order (High)
- Hit-testing implementation (Medium)
- Easing function configurability (Low)
- IMGUI vs UIToolkit (Medium)

**Recommended Priority**:
- Phase 1: sdd-unity-asset-pipeline-fidelity + sdd-unity-canvas-preview-transforms
- Phase 2: sdd-unity-animation-timeline-ui
- Phase 3: sdd-unity-audio-preview + sdd-unity-undo-redo

---

### 2026-05-08 - Initialization

**Mode**: BFS
**Target**: project root

**Actions**:
- Scanned existing flows directory
- Found 7 VDD flows (legacy-* prefix)
- Found 6 SDD flows (unity-* prefix)
- Built existing flows index in _traverse.md
- Initialized legacy workspace from templates
- Created root node in understanding/_root.md

**Existing Flows Discovered**:
- VDD: format, image-pipeline, rendering, undo-redo, audio, animation-timeline, workspace-packaging
- SDD: unity-parity-gaps-overview, unity-asset-pipeline-fidelity, unity-canvas-preview-transforms, unity-animation-timeline-ui, unity-audio-preview, unity-undo-redo

**Next**:
- Complete ENTERING phase at root
- Scan project structure to identify logical domains

---

*Append new entries at the top.*
