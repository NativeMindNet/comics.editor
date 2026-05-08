# Understanding: Unity Port Parity Gaps

## Phase: SYNTHESIZING

## Hypothesis

Unity port is significantly behind WPF in feature completeness, with core data models ported but authoring UI, preview rendering, and audio integration substantially simplified.

## Sources

- `app/unity_comics.editor/UnityComicsEditor/Assets/ComicsUnity/Editor/` - Unity implementation
- `app/unity_comics.editor/Comics.Editor/` - WPF reference (full implementation)
- `flows/sdd-unity-parity-gaps-overview/` - Existing gap analysis
- `flows/sdd-unity-*/` - Child SDD flows

## Validated Understanding

### Overall Status

**Unity port is approximately 40% complete** relative to WPF parity.

| Aspect | WPF | Unity v1 | Parity |
|--------|-----|----------|--------|
| Data models | Full | Full | 100% |
| File I/O | 7za + ImageMagick | ZipFile + Texture2D | Unvalidated |
| Preview canvas | Composed transforms | Vertical image list | 20% |
| Animation UI | 4 type editors | 1 button (translate) | 10% |
| Audio playback | MediaPlayer + scrub | File copy only | 0% |
| Undo/Redo | None | None | N/A (new feature) |

### Critical Missing Functionality

1. **Composed preview canvas**: Unity shows stacked images, not transformed layers on single canvas
2. **Animation CRUD**: Only "Add translate key segment"; no rotate/scale/alpha/sound UI
3. **Audio playback**: No MediaPlayer equivalent; no scrub-driven sound triggers
4. **Image/culture workflow**: Culture dropdown exists but no per-culture image picker

### Feature Matrix

| Feature | WPF Has | Unity Has | Child SDD |
|---------|---------|-----------|-----------|
| Composed canvas | ✓ | ✗ | sdd-unity-canvas-preview-transforms |
| All anim editors | ✓ | ✗ | sdd-unity-animation-timeline-ui |
| Audio playback | ✓ | ✗ | sdd-unity-audio-preview |
| Zip round-trip | ✓ | Unvalidated | sdd-unity-asset-pipeline-fidelity |
| Undo/Redo | ✗ | ✗ | sdd-unity-undo-redo (new feature) |

### Architecture Differences

| Aspect | WPF | Unity |
|--------|-----|-------|
| UI framework | XAML + MVVM | IMGUI (OnGUI) |
| Zip library | 7za.exe | System.IO.Compression.ZipFile |
| Image processing | ImageMagick | Texture2D + RenderTexture |
| ViewModel layer | Full (LayerViewModel, SoundViewModel) | None (direct session) |
| Temp directory | %LOCALAPPDATA%\Comics Editor\Temp | Application.temporaryCachePath |

### Child SDD Status

All child SDDs are in **REQUIREMENTS DRAFTING** phase:

| SDD | Criticality | Status | Blocker |
|-----|-------------|--------|---------|
| sdd-unity-asset-pipeline-fidelity | Foundation | DRAFTING | None |
| sdd-unity-canvas-preview-transforms | Critical | DRAFTING | None |
| sdd-unity-animation-timeline-ui | Critical | DRAFTING | Canvas preview |
| sdd-unity-audio-preview | High | DRAFTING | Canvas preview |
| sdd-unity-undo-redo | Medium | DRAFTING | Animation UI |

## Children Identified

| Child | Hypothesis | Status |
|-------|------------|--------|
| (none) | Unity-port is analysis node | - |

## Dependencies

- **Uses**: document-model, animation-system, file-format, canvas-rendering (WPF as reference)
- **Used by**: (end deliverable)

## Key Insights

1. **Model layer complete**: All data models ported (ComicsDocument, LayerModel, Anim hierarchy)
2. **ViewModel layer missing**: No LayerViewModel, SoundViewModel equivalents in Unity
3. **UI framework gap**: IMGUI vs. XAML/MVVM creates structural differences
4. **Asset pipeline unvalidated**: Zip/JSON round-trip not tested against legacy
5. **Canvas is showstopper**: Current "vertical list" view blocks visual feedback
6. **Audio is showstopper**: Cannot author sound-synced content without playback

## ADR Candidates

1. **IMGUI vs UIToolkit**: Choose UI framework for timeline and inspector
2. **Audio API choice**: Unity Editor audio vs. AudioClip import vs. native plugin
3. **Undo/Redo architecture**: Command pattern vs. snapshot vs. hybrid
4. **Zip compatibility level**: Byte-for-byte vs. functional equivalence

## Flow Recommendation

- **Type**: SDD (existing flows: sdd-unity-parity-gaps-overview + children)
- **Confidence**: high
- **Rationale**: Existing parent SDD coordinates child flows

## Flow Updates Required

| Flow | Action | Additions |
|------|--------|-----------|
| sdd-unity-parity-gaps-overview/01-requirements.md | APPEND | Detailed feature matrix, prioritized dependencies |

## Synthesis

### Recommended Prioritization

**Phase 1 (Foundation):**
1. sdd-unity-asset-pipeline-fidelity - validate round-trip
2. sdd-unity-canvas-preview-transforms - visual feedback

**Phase 2 (Authoring Core):**
3. sdd-unity-animation-timeline-ui - all anim CRUD

**Phase 3 (Media + Safety):**
4. sdd-unity-audio-preview - sound playback
5. sdd-unity-undo-redo - crash protection

### Combined Understanding

Unity port has complete data models but lacks:
- Composed preview canvas (critical for visual feedback)
- Animation editing UI beyond translate (critical for authoring)
- Audio playback integration (high for sound authoring)
- Round-trip validation (high for data integrity)

All gaps are scoped into existing child SDD flows; execution order matters for dependency management.

## Bubble Up

- Unity port ~40% complete; data models done, UI incomplete
- Canvas preview is showstopper (no composed transforms view)
- Animation UI has only 1 of 4+ operations implemented
- Audio playback completely missing
- Recommend: asset-fidelity + canvas-preview first (parallel), then animation UI

---

*Phase: SYNTHESIZING | Depth: 2 | Parent: / (root)*
