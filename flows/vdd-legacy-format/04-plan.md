# Implementation Plan: legacy document format v2 (comics/puzzle)

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Specifications: [link to 03-specifications.md]

## Summary

Implement a versioned bundle reader + validator + legacy importer that produces a stable `DocumentModel` and an `AssetIndex` suitable for lazy rendering and editing.

## Task Breakdown

### Phase 1: Foundation

#### Task 1.1: Define v2 schema + manifest contract
- **Description**: Freeze the JSON shapes for `manifest` and `data` with explicit `schemaVersion`.
- **Files**:
  - `flows/vdd-legacy-format/03-specifications.md` - Modify (finalize schema section)
- **Dependencies**: None
- **Verification**: Example JSON validates against the agreed structure
- **Complexity**: Medium

#### Task 1.2: Define legacy-to-v2 mapping rules
- **Description**: Document exact mapping from legacy fields (layers/images/sounds/anims) to v2.
- **Files**:
  - `flows/vdd-legacy-format/03-specifications.md` - Modify
- **Dependencies**: Task 1.1
- **Verification**: Mapping table covers all legacy model fields and known quirks
- **Complexity**: Medium

### Phase 2: Core Implementation

#### Task 2.1: Implement bundle reader
- **Description**: Read container entries, load `manifest`/`data`, and build an asset index.
- **Files**:
  - `lib/...` - Create/Modify (Flutter/Dart code, paths TBD in implementation phase)
- **Dependencies**: Task 1.1
- **Verification**: Loads a v2 sample bundle end-to-end
- **Complexity**: High

#### Task 2.2: Implement validator + issue reporting
- **Description**: Detect missing/invalid assets, bad checksums, required fields.
- **Files**:
  - `lib/...` - Create/Modify
- **Dependencies**: Task 2.1
- **Verification**: Produces actionable report for known-bad bundles
- **Complexity**: Medium

#### Task 2.3: Implement legacy importer (read-only)
- **Description**: Read legacy zip + legacy `data.json`, map to v2 in-memory model and asset index.
- **Files**:
  - `lib/...` - Create/Modify
- **Dependencies**: Task 1.2, Task 2.1
- **Verification**: Opens a real legacy `.comics/.puzzle` without crashes; assets resolve
- **Complexity**: High

### Phase 3: Integration

#### Task 3.1: Integrate with editor model layer
- **Description**: Ensure the rest of the app consumes a stable `DocumentModel`.
- **Files**:
  - `lib/...` - Modify
- **Dependencies**: Task 2.1–2.3
- **Verification**: Editor can render basic scene from loaded document
- **Complexity**: Medium

### Phase 4: Testing & Polish

#### Task 4.1: Add fixtures + regression suite
- **Description**: Add a small set of sample bundles (or generated) and tests.
- **Files**:
  - `test/...` - Create/Modify
- **Dependencies**: Task 2.1–3.1
- **Verification**: CI/local test run passes; includes legacy fixture coverage
- **Complexity**: Medium

## Dependency Graph

```
Task 1.1 -> Task 2.1 -> Task 2.2 -> Task 3.1 -> Task 4.1
Task 1.2 -----------^-> Task 2.3 ----^
```

## File Change Summary

| File | Action | Reason |
|------|--------|--------|
| `flows/vdd-legacy-format/*` | Create | VDD artifacts for this initiative |
| `lib/...` | Create/Modify | Bundle reader/importer/validator |
| `test/...` | Create/Modify | Regression + fixture tests |

## Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| Tile conventions differ across legacy docs | Med | High | Manifest-driven addressing; tolerant importer |
| Memory/perf issues on huge images | Med | High | Strict lazy loading; no full-bitmap assembly |
| Culture mapping ambiguity in legacy | High | Med | Explicit mapping in importer; warnings |

## Rollback Strategy

1. Keep legacy import behind a feature flag.
2. Preserve read-only fallback that only inspects bundle and reports issues.

## Checkpoints

- [ ] After Phase 1: schema + mapping agreed
- [ ] After Phase 2: v2 + legacy load paths both work
- [ ] After Phase 3: editor opens documents end-to-end
- [ ] After Phase 4: tests + fixtures protect regressions

## Open Implementation Questions

- [ ] Where do sample fixtures live in this repo (size constraints)?

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
