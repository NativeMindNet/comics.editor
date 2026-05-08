# Code to Flow Mapping

## Overview

Maps analyzed code modules to generated flows.

## Flow Type Detection Rules

| Indicator | Flow Type |
|-----------|-----------|
| `*.test.*`, `*.spec.*`, `__tests__/` | TDD |
| `components/`, `*.tsx`, `*.vue`, `templates/`, UI-heavy | VDD |
| `README.md`, public exports, API docs, stakeholder-facing | DDD |
| Internal logic, no UI, no public API | SDD |
| Whole product/program (charter, domain, IA, multi-stack) | PDD |

## Mapping Table

| Code Path | Flow | Type | Action | Status | Notes |
|-----------|------|------|--------|--------|-------|
| [pending analysis] | - | - | - | - | - |

### Action Values
- **CREATED** - New flow created
- **UPDATED** - Existing flow appended to (additive changes only)
- **UNCHANGED** - Flow exists, no new information found
- **CONFLICT** - Analysis contradicts existing documentation (needs reconciliation)

## ADR Mapping

| Code Pattern | ADR | Type | Status |
|--------------|-----|------|--------|
| [pending analysis] | - | - | - |

## Unmapped (needs manual review)

| Code Path | Reason |
|-----------|--------|
| - | - |

---

*Auto-generated. Update as analysis progresses.*
