# ADR Index

Master index of all Architecture Decision Records.

## Active ADRs

| # | Name | Title | Type | Status | Created | Decided | File |
|---|------|-------|------|--------|---------|---------|------|
| 001 | serialization-type-handling | Serialization Type Handling for Polymorphic Animations | constraining | DRAFT | 2026-05-08 | - | `adr-001-serialization-type-handling/` |
| 002 | culture-enum-expansion | Culture Enum Expansion (2 vs 3 Cultures) | constraining | DRAFT | 2026-05-08 | - | `adr-002-culture-enum-expansion/` |
| 003 | collection-type-standardization | Collection Type Standardization (ObservableCollection vs List) | constraining | DRAFT | 2026-05-08 | - | `adr-003-collection-type-standardization/` |
| 004 | atomic-save-strategy | Atomic Save Strategy | enabling | DRAFT | 2026-05-08 | - | `adr-004-atomic-save-strategy/` |
| 005 | schema-versioning | Schema Versioning for Document Format | enabling | DRAFT | 2026-05-08 | - | `adr-005-schema-versioning/` |
| 006 | transform-composition-order | Transform Composition Order | constraining | DRAFT | 2026-05-08 | - | `adr-006-transform-composition-order/` |
| 007 | hit-testing-implementation | Hit-Testing Implementation | enabling | DRAFT | 2026-05-08 | - | `adr-007-hit-testing-implementation/` |
| 008 | easing-configurability | Easing Function Configurability | enabling | DRAFT | 2026-05-08 | - | `adr-008-easing-configurability/` |
| 009 | unity-ui-framework | Unity UI Framework (IMGUI vs UIToolkit) | constraining | DRAFT | 2026-05-08 | - | `adr-009-unity-ui-framework/` |

### Types
- **constraining** - selects from options, closes alternatives
- **enabling** - adds new capabilities, expands scope

## Statistics

- **Total**: 9
- **Approved**: 0
- **Review**: 0
- **Draft**: 9
- **Rejected**: 0
- **Superseded**: 0

## Categories

### Architecture
- ADR-001: Serialization Type Handling
- ADR-003: Collection Type Standardization
- ADR-005: Schema Versioning
- ADR-006: Transform Composition Order
- ADR-009: Unity UI Framework

### Data Safety
- ADR-004: Atomic Save Strategy

### Data Model
- ADR-002: Culture Enum Expansion

### Rendering
- ADR-006: Transform Composition Order
- ADR-007: Hit-Testing Implementation

### Animation
- ADR-008: Easing Configurability

### UI
- ADR-009: Unity UI Framework

## Relationships

### Dependencies
- ADR-001 depends on ADR-005 (schema versioning needed for v1/v2 detection)
- ADR-002 depends on ADR-005 (culture keys require v2 format)
- ADR-007 depends on ADR-006 (hit-testing needs inverse of transform)

### Supersedes
- (none)

## Priority Recommendations

| Priority | ADRs | Rationale |
|----------|------|-----------|
| High | ADR-004, ADR-005, ADR-006 | Data safety, format foundation, rendering correctness |
| Medium | ADR-001, ADR-002, ADR-007, ADR-009 | Format evolution, interaction, UI framework |
| Low | ADR-003, ADR-008 | Convenience improvements |

---

## Index Maintenance

When creating/updating ADRs:
1. Add entry to table above
2. Update statistics
3. Add to relevant category
4. Note any relationships

**Last updated**: 2026-05-08
**Next ADR number**: 10
