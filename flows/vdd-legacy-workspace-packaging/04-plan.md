# Implementation Plan: workspace & packaging (import/export, atomic save)

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Specifications: [link to 03-specifications.md]

## Summary

Implement a workspace manager and atomic save pipeline that can open and write bundles reliably across platforms, without external CLI tools.

## Task Breakdown

### Phase 1: Foundation

#### Task 1.1: Choose container I/O implementation
- **Description**: Select zip/container library strategy for all platforms.
- **Files**:
  - `flows/vdd-legacy-workspace-packaging/03-specifications.md` - Modify
- **Dependencies**: None
- **Verification**: Prototype reads/writes a small bundle
- **Complexity**: High

#### Task 1.2: Define workspace layout + mapping rules
- **Description**: Decide extracted paths, staging, and derived assets directories.
- **Files**:
  - `lib/...` - Create/Modify
- **Dependencies**: Task 1.1
- **Verification**: Same bundle always maps to same layout
- **Complexity**: Medium

### Phase 2: Save pipeline

#### Task 2.1: Implement atomic writer
- **Description**: temp write + commit rename/replace semantics.
- **Files**:
  - `lib/...` - Create/Modify
  - `test/...` - Create/Modify
- **Dependencies**: Task 1.1
- **Verification**: tests simulate failure and confirm rollback
- **Complexity**: Medium

#### Task 2.2: Implement recovery on open
- **Description**: detect temp artifacts; resume/cleanup safely.
- **Files**:
  - `lib/...` - Create/Modify
- **Dependencies**: Task 2.1
- **Verification**: integration test for crash mid-save
- **Complexity**: Medium

### Phase 3: Performance + polish

#### Task 3.1: Streaming save and skip unchanged assets (optional)
- **Description**: minimize write work for large bundles.
- **Files**:
  - `lib/...` - Modify
- **Dependencies**: Task 2.1
- **Verification**: large tile bundle saves within budget
- **Complexity**: High

## Dependency Graph

```
Task 1.1 -> Task 1.2
Task 1.1 -> Task 2.1 -> Task 2.2 -> Task 3.1
```

## Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| Many-files bundle is slow to pack | Med | High | streaming + skip unchanged assets |
| Platform file limitations | Med | Med | consider fewer files via larger tiles or packed tile pages |

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
