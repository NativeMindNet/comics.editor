# ADR-005: Schema Versioning for Document Format

## Meta

- **Number**: ADR-005
- **Type**: enabling
- **Status**: DRAFT
- **Created**: 2026-05-08
- **Decided**: -
- **Author**: /legacy analysis
- **Reviewers**: -

## Context

The current document format (data.json inside .comics/.puzzle ZIP) has no version field:

```json
{
  "width": 1080,
  "height": 2160,
  "layers": [...],
  "sounds": [...]
}
```

**Problem**:
- No way to detect format version
- Breaking changes (e.g., new serialization, culture keys) can't be distinguished
- No migration path when format evolves
- Older editors may silently corrupt newer documents

Multiple ADRs propose breaking changes that require version detection:
- ADR-001: Serialization type handling
- ADR-002: Culture key mapping

## Decision Drivers

- **Forward compatibility**: Newer editors should read older documents
- **Backward safety**: Older editors should refuse/warn on newer documents
- **Migration path**: Clear upgrade story for each version
- **Simplicity**: Version detection should be trivial

## Considered Options

### Option 1: No Versioning (Status Quo)

**Description**: Continue without version field; rely on feature detection.

**Pros**:
- No changes required
- Feature detection is flexible

**Cons**:
- Ambiguous format detection
- No clear migration path
- Silent corruption risk

**Estimated Effort**: None

### Option 2: Integer Version Field

**Description**: Add `schemaVersion` integer to document root:

```json
{
  "schemaVersion": 2,
  "width": 1080,
  ...
}
```

Rules:
- Missing `schemaVersion` = version 1 (legacy)
- Version N+1 editor reads version N and below
- Version N editor refuses version N+1 (or opens read-only)

**Pros**:
- Simple and unambiguous
- Easy to implement
- Clear semantics

**Cons**:
- Single dimension (can't express feature flags)
- Must increment for any breaking change

**Estimated Effort**: Low

### Option 3: Semantic Versioning

**Description**: Use semver string: `"schemaVersion": "2.1.0"`

**Pros**:
- Rich version semantics (major.minor.patch)
- Can express backward-compatible additions

**Cons**:
- Overkill for document format
- More complex parsing
- Semver semantics may not fit

**Estimated Effort**: Low

### Option 4: Feature Flags

**Description**: Use feature flags instead of version:

```json
{
  "features": ["explicitAnimType", "cultureDictionary"],
  ...
}
```

**Pros**:
- Fine-grained compatibility
- Can enable/disable independently

**Cons**:
- Complex compatibility matrix
- Feature dependencies hard to track
- No clear ordering

**Estimated Effort**: Medium

## Decision

**[PENDING DECISION]**

Recommended: **Option 2 (Integer Version Field)**

Simple, clear, and sufficient for document format evolution. Combined with a migration registry:

```csharp
var migrators = new Dictionary<int, Func<JObject, JObject>> {
    { 1, MigrateV1ToV2 },
    { 2, MigrateV2ToV3 },
};
```

## Consequences

### Positive

- Clear format versioning
- Enables safe format evolution
- Migration path documented

### Negative

- Must increment version for any breaking change
- Old editors can't read new documents (by design)

### Neutral

- Minimal JSON size increase (one field)

## Implementation Notes

- Default `schemaVersion` to 1 if missing (legacy detection)
- Add version check on load; refuse if > supported version
- Create `DocumentMigrator` class with per-version upgrade functions
- Log migration actions for debugging

## Related Decisions

- ADR-001: Serialization Type Handling (requires v2)
- ADR-002: Culture Enum Expansion (requires v2)

## Related Specs

- `flows/vdd-legacy-format/`: Document format specification

## References

- JSON Schema versioning: https://json-schema.org/understanding-json-schema/reference/schema.html

## Tags

architecture compatibility schema

---

## Approval

### Review History

| Date | Reviewer | Status | Comments |
|------|----------|--------|----------|
| - | - | pending | - |

### Final Decision

- [ ] Approved by: -
- [ ] Decided on: -
- [ ] Implementation assigned to: -
