# Specifications: legacy document format v2 (comics/puzzle)

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Requirements: [link to 01-requirements.md]

## Overview

Define a portable, versioned document format (v2) for comics/puzzle bundles, with:
- explicit schema versioning + migrations
- explicit culture mapping
- manifest-driven assets (including tiles + checksums)
- compatibility strategy for legacy `.comics/.puzzle`

## Affected Systems

| System | Impact | Notes |
|--------|--------|-------|
| Document container | Create | v2 bundle layout + manifest |
| JSON schema | Create | `data.json` + `manifest.json` (or unified) |
| Legacy import | Create | Read legacy zip + map to v2 model |
| Validation | Create | Missing assets, checksum verification |
| Export | Modify/Create | Optional “export legacy” for compatibility |

## Architecture

### Component Diagram

```
[Bundle Reader] -> [Schema Detect] -> [Migrator] -> [Validator] -> [Editor Model]
      |                               |
      v                               v
 [Asset Index]                    [Issue Report]
```

### Data Flow

```
zip -> list entries -> read manifest/data -> build asset index -> lazy decode tiles -> editor renders
```

## Interfaces

### New Interfaces

```cpp
// Pseudocode / shape only (language-agnostic)
interface DocumentBundleReader {
  DocumentLoadResult load(bytesOrPath input);
}

interface DocumentMigrator {
  DocumentModel migrateToLatest(DocumentModel anyVersion);
}

interface DocumentValidator {
  ValidationReport validate(DocumentBundle bundle);
}
```

### Modified Interfaces

- N/A (new system for Flutter rewrite; legacy stays read-only).

## Data Models

### New Types (conceptual)

```cpp
struct DocumentBundle {
  Manifest manifest;
  DocumentModel data;
  AssetIndex assets;
}

struct Manifest {
  int schemaVersion;
  string docType; // "comics" | "puzzle"
  map<string, AssetEntry> assets; // id -> entry (path + checksum + metadata)
}

struct DocumentModel {
  int width;
  int height;
  list<Layer> layers;
  list<Sound> sounds;
}
```

### Schema Changes

- Introduce `schemaVersion`.
- Replace culture-indexed `images[]` with `imagesByCulture: { "en": {...}, "ru": {...} }`.
- Make tile naming **manifest-driven** (avoid parsing `{0}` placeholders as schema).

## Behavior Specifications

### Happy Path

1. User opens `.comics/.puzzle`.
2. System detects legacy vs v2.
3. If legacy: import to latest in-memory model, build asset index.
4. Validate assets; show non-blocking warnings if allowed.
5. Editor opens with lazy asset loading.

### Edge Cases

| Case | Trigger | Expected Behavior |
|------|---------|-------------------|
| Missing tile | archive missing one tile | mark layer degraded; allow open; render missing as transparent + warning |
| Unknown culture | document contains culture not supported by app | keep data; show as “unsupported” until app adds culture |
| Unknown schemaVersion | newer than supported | refuse open OR open with warning in read-only mode (decision TBD) |
| Corrupt zip | cannot read entries | fail with actionable error |

### Error Handling

| Error | Cause | Response |
|-------|-------|----------|
| BundleReadError | bad archive | show error + recovery options |
| ValidationError | missing required fields | block open unless “open anyway” allowed |
| MigrationError | incompatible legacy data | block open; include diagnostics |

## Dependencies

### Requires

- A zip/container implementation usable on target platforms.
- A checksum/hash implementation (if using checksums).

### Blocks

- Rendering pipeline depends on agreed tile/asset layout.

## Integration Points

### External Systems

- Optional: legacy backend download/upload if documents are stored remotely (TBD).

### Internal Systems

- Editor scene graph consumes `DocumentModel`.
- Asset loader consumes `AssetIndex` and tile conventions.

## Testing Strategy

### Unit Tests

- [ ] Legacy importer: parse legacy `data.json` -> v2 model mapping.
- [ ] Validator: missing asset detection.
- [ ] Migrator: schema upgrades (v1 -> v2 -> ...).

### Integration Tests

- [ ] Open a real legacy bundle and verify layer count, sizes, and asset resolution.

### Manual Verification

- [ ] Open legacy `.puzzle` with multi-scale tiles; scroll/zoom; confirm no full-bitmap assembly required.

## Migration / Rollout

- Phase 1: Read legacy + write v2 (no legacy export).
- Phase 2 (optional): Export legacy for compatibility.

## Open Design Questions

- [ ] Zip vs alternative container (but still single file).
- [ ] Strictness of validation and "open anyway" policy.
- [ ] Canonical tile naming vs manifest-only addressing.

---

## Data Models - Legacy Additions
> Added by /legacy on 2026-05-08

### Animation Type Hierarchy

The existing models use a polymorphic animation system:

```csharp
Anim (abstract)
├── Start (int) - scroll position begin
├── End (int) - scroll position end
├── Type (AnimTypes enum: Translate, Rotate, Scale, Alpha, Sound)
├── Interpolate(Anim, double) - abstract interpolation method
│
├── TranslateAnim
│   ├── X (int) - horizontal offset
│   └── Y (int) - vertical offset
│
├── RotateAnim extends PivotAnim
│   └── Angle (double) - rotation in degrees
│
├── ScaleAnim extends PivotAnim
│   ├── ScaleX (double) - horizontal scale multiplier
│   └── ScaleY (double) - vertical scale multiplier
│
├── AlphaAnim
│   └── Alpha (double) - opacity 0-1
│
├── SoundAnim
│   └── [marks playback point in timeline]
│
└── PivotAnim (abstract)
    ├── PivotX (double, default 0.5) - normalized X pivot
    └── PivotY (double, default 0.5) - normalized Y pivot
```

### Interpolation Formula

Animation easing uses a cubic function:
```csharp
protected double Factor(double t)
{
    return (--t) * t * t + 1;  // Ease-out cubic
}
```
- `t` ranges from 0.0 to 1.0 (normalized progress)
- Formula produces smooth deceleration

### Serialization Details

Current serialization relies on `TypeNameHandling.Auto`:
```json
{
  "animations": [
    { "$type": "ComicsUnity.Models.TranslateAnim, ComicsUnity", "start": 0, "end": 100, "x": 10, "y": 20 },
    { "$type": "ComicsUnity.Models.AlphaAnim, ComicsUnity", "start": 100, "end": 200, "alpha": 0.5 }
  ]
}
```

**Risks:**
- Class rename/namespace change breaks document loading
- v2 should consider explicit type discriminator: `"animType": "translate"`

### Culture Handling Mismatch

| Layer | Cultures Supported |
|-------|-------------------|
| Comics.Core (DAL) | En, Ru (2 cultures) |
| Comics.Editor (WPF) | En, Ru, Hi (3 cultures) |
| UnityComicsEditor | En, Ru, Hi (3 cultures) |

**Implication:** Hindi content in editor cannot roundtrip to Core database. v2 should document culture expansion strategy.

### Collection Type Divergence

| Platform | Animations Collection Type |
|----------|---------------------------|
| WPF | `ObservableCollection<Anim>` (for UI binding) |
| Unity | `List<Anim>` |

v2 schema should standardize on JSON array; runtime can wrap as needed.

### Pivot Initialization Edge Case

`PivotAnim.Init()` sets defaults to (0.5, 0.5), but is only called explicitly. Deserialization may skip this, leaving pivots at 0. v2 should serialize pivot defaults explicitly or guarantee initialization.

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
