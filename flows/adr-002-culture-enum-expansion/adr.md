# ADR-002: Culture Enum Expansion (2 vs 3 Cultures)

## Meta

- **Number**: ADR-002
- **Type**: constraining
- **Status**: DRAFT
- **Created**: 2026-05-08
- **Decided**: -
- **Author**: /legacy analysis
- **Reviewers**: -

## Context

The codebase has inconsistent culture definitions:

| Layer | Cultures Supported |
|-------|-------------------|
| Comics.Core (DAL) | En, Ru (2 cultures) |
| Comics.Editor (WPF) | En, Ru, Hi (3 cultures) |
| UnityComicsEditor | En, Ru, Hi (3 cultures) |

**Problem**: Hindi (Hi) content created in the editor cannot be persisted to the Core database. If a document with Hindi images is saved and then loaded by a system that only knows 2 cultures, the Hindi data may be lost or misinterpreted.

The culture system uses **index-based mapping**:
```csharp
List<Image> images = new List<Image>(3);  // [0]=En, [1]=Ru, [2]=Hi
```

This is fragile: if culture order changes or new cultures are added, all existing documents become misaligned.

## Decision Drivers

- **Data integrity**: No content loss when round-tripping through different systems
- **Extensibility**: Future cultures (Es, Zh, etc.) should be addable
- **Backward compatibility**: Existing 2-culture documents must remain readable
- **Simplicity**: Avoid overengineering for limited localization needs

## Considered Options

### Option 1: Expand Core to 3 Cultures

**Description**: Add Hindi to Comics.Core enum and update database schema.

**Pros**:
- Full round-trip compatibility
- Single source of truth

**Cons**:
- Database migration required
- May break existing Core consumers

**Estimated Effort**: Medium

### Option 2: Keep Hi as Editor-Only

**Description**: Document that Hindi is editor-only; Core roundtrip not supported.

**Pros**:
- No Core changes
- Simple to implement (already working)

**Cons**:
- Data loss risk when syncing to Core
- Confusing for users

**Estimated Effort**: Low (documentation only)

### Option 3: Explicit Culture Keys in JSON

**Description**: Replace index-based array with keyed dictionary:

```json
{
  "images": {
    "en": { "file": "image_en.jpg", ... },
    "ru": { "file": "image_ru.jpg", ... },
    "hi": { "file": "image_hi.jpg", ... }
  }
}
```

**Pros**:
- Order-independent
- Unlimited culture expansion
- Self-documenting JSON

**Cons**:
- Breaking change (v2 format)
- Migration required for existing files
- Slightly more complex deserialization

**Estimated Effort**: Medium

### Option 4: Hybrid - Index + Overflow

**Description**: Keep indexed array for known cultures; add overflow dictionary for new cultures.

```json
{
  "images": [...],  // legacy [En, Ru]
  "imagesByCulture": { "hi": {...} }  // new cultures
}
```

**Pros**:
- Backward compatible for read
- Extensible

**Cons**:
- Two storage locations
- Complex merge logic

**Estimated Effort**: Medium

## Decision

**[PENDING DECISION]**

Recommended: **Option 3 (Explicit Culture Keys)** as part of v2 format, combined with **Option 1 (Expand Core)** if database sync is required.

For immediate term, **Option 2** (document editor-only) is acceptable if Core sync is not a priority.

## Consequences

### Positive

- Future-proof for unlimited cultures
- No index ordering bugs
- Self-documenting format

### Negative

- Breaking change requires format migration
- Core database migration if sync needed

### Neutral

- Editor UX unchanged (same culture dropdown)

## Implementation Notes

- Add migration logic: read index-based → write key-based
- Update `CulturesHelper` to use dictionary lookup
- Consider ISO 639-1 codes ("en", "ru", "hi") for standardization

## Related Decisions

- ADR-005: Schema Versioning (v2 format container)
- ADR-001: Serialization Type Handling (related format changes)

## Related Specs

- `flows/vdd-legacy-format/`: Document format specification

## References

- ISO 639-1 Language Codes: https://en.wikipedia.org/wiki/ISO_639-1

## Tags

localization architecture data-model

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
