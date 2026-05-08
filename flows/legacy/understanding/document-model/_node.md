# Understanding: Document Model

## Phase: SYNTHESIZING

## Hypothesis

Core data structures defining the comics/puzzle document format, including ComicsDocument, layers, images, sounds, and animation types.

## Sources

- `app/unity_comics.editor/Comics.Core/DAL/Model/` - Entity Framework models for backend database
- `app/unity_comics.editor/Comics.Editor/Models/` - WPF editor runtime models
- `app/unity_comics.editor/UnityComicsEditor/Assets/ComicsUnity/Editor/Models/` - Unity port models

## Validated Understanding

### Model Hierarchy

**Three-Layer Architecture:**
1. **Comics.Core** (DAL): Entity Framework models for Episodes, Seasons, Puzzles, Pieces, Quotes, Tokens (localization)
2. **Comics.Editor** (WPF): In-memory working document (Comics, Layer, Image, Sound, Anim hierarchy)
3. **UnityComicsEditor**: Parallel structure to WPF (ComicsDocument, LayerModel, ImageModel, SoundModel, same Anim types)

### Document Structure

```
ComicsDocument (Root)
├── Width, Height (canvas dimensions, default 1080x2160)
├── Layers (List<LayerModel>)
│   └── LayerModel
│       ├── Preview (bool)
│       ├── Images (List<ImageModel>) - one per culture
│       └── Animations (List<Anim>)
└── Sounds (List<SoundModel>)
    └── SoundModel
        ├── File (string)
        └── Animations (List<Anim>)
```

### Animation Type Hierarchy

```
Anim (abstract)
├── Start, End (scroll positions)
├── Type (AnimTypes enum)
├── Interpolate() method
├── TranslateAnim (X, Y)
├── RotateAnim extends PivotAnim (Angle)
├── ScaleAnim extends PivotAnim (ScaleX, ScaleY)
├── AlphaAnim (Alpha 0-1)
├── SoundAnim
└── PivotAnim (abstract) (PivotX, PivotY default 0.5)
```

### Serialization

- Newtonsoft JSON with `TypeNameHandling.Auto` for polymorphic Anim types
- `CamelCasePropertyNamesContractResolver` + `DefaultValueHandling.Ignore`
- Extension methods: `ToJson()` / `FromJson<T>()`

### Culture Handling

- **Core DAL**: En, Ru (2 cultures)
- **Editor/Unity**: En, Ru, Hi (3 cultures) - MISMATCH
- Images stored as parallel lists indexed by `CulturesHelper.All`
- Animations shared across all cultures

## Children Identified

| Child | Hypothesis | Status |
|-------|------------|--------|
| animation-system | Deep dive into interpolation, easing, segment logic | Spawned separately |

## Dependencies

- **Uses**: file-format (JSON serialization), workspace-management (file I/O)
- **Used by**: canvas-rendering (display), animation-system (playback), unity-port (parity)

## Key Insights

1. **WPF uses ObservableCollection; Unity uses List** - affects UI update notification patterns
2. **Tile detection via `File.Contains("{0}")`** - fragile; proposed manifest approach in VDD
3. **Pivot defaults set via Init() method** - risk of uninitialized pivots on deserialization
4. **Easing is hardcoded cubic: `(--t) * t * t + 1`** - not configurable
5. **Layer.Preview boolean semantics unclear** - hidden? disabled? draft?
6. **Null handling is silent** - no logging for corrupt JSON, defensive `??=` re-initialization

## ADR Candidates

1. **TypeNameHandling.Auto vs. Explicit Type Discriminator** - class rename breaks documents
2. **Culture Enum Mismatch (2 vs 3)** - Hindi content cannot roundtrip to Core DAL
3. **ObservableCollection vs List** - WPF/Unity divergence affects shared code
4. **Tile Naming Convention** - runtime detection vs. manifest approach
5. **Animation Easing Configurability** - hardcoded cubic vs. parameterized
6. **Layer State Machine** - boolean Preview vs. enum (Visible/Hidden/Locked/Draft)
7. **Schema Versioning** - no explicit version field in document format

## Flow Recommendation

- **Type**: VDD (existing flow: vdd-legacy-format)
- **Confidence**: high
- **Rationale**: Matches existing flow; new insights should append to existing documentation

## Flow Updates Required

| Flow | Action | Additions |
|------|--------|-----------|
| vdd-legacy-format | APPEND | Animation type hierarchy details, serialization nuances, culture mismatch |
| adr-index | CREATE entries | 7 ADR candidates identified above |

## Synthesis

### From Children
[animation-system analyzed separately]

### Combined Understanding
The document model is well-structured but has several documentation gaps around serialization edge cases, culture handling mismatches, and implicit defaults. The existing vdd-legacy-format flow covers the high-level schema but lacks implementation details for:
- Complete animation type hierarchy with properties
- Interpolation formula documentation
- Culture index-based mapping brittleness
- Property change notification patterns

## Bubble Up

- Document model spans 3 codebases with subtle differences (ObservableCollection vs List)
- Culture enum mismatch between Core (2) and Editor/Unity (3) needs ADR
- Serialization relies on TypeNameHandling.Auto which is fragile for class renames
- 7 ADR candidates identified for architectural decisions

---

*Phase: SYNTHESIZING | Depth: 2 | Parent: / (root)*
