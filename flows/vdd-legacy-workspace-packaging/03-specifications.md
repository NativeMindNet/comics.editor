# Specifications: workspace & packaging (import/export, atomic save)

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Requirements: [link to 01-requirements.md]

## Overview

Define a cross-platform workspace model and packaging strategy that supports:
- open/extract or stream-based access
- atomic save and recovery
- large bundle performance (many tiles)
- safe integration with undo/redo for asset operations

## Affected Systems

| System | Impact | Notes |
|--------|--------|-------|
| Bundle I/O | Create/Modify | read/write container without 7zip CLI |
| Workspace manager | Create | maps bundle to editable workspace |
| Save pipeline | Create | atomic write + staging |
| Recovery | Create | detects partial saves and repairs/rolls back |

## Architecture

### Component Diagram

```
[Bundle Reader] -> [Workspace Manager] -> [Editor]
                        |
                        v
                  [Save Pipeline]
                        |
                        v
                  [Atomic Writer]
```

### Data Flow

```
open -> read bundle -> materialize workspace (files or virtual) -> edit -> save -> write temp -> rename
```

## Interfaces

### New Interfaces (conceptual)

```cpp
interface WorkspaceManager {
  Workspace open(bundlePathOrBytes);
  SaveResult save(Workspace ws, outputPath);
}

interface AtomicWriter {
  void writeTempAndCommit(targetPath, writeFn);
}
```

## Behavior Specifications

### Atomic save

- Save writes to a temporary file in the same directory (or safe staging area).
- On success, rename/replace final bundle atomically.
- On failure, old bundle remains unchanged.

### Recovery

- If a temp save artifact exists on open:
  - detect whether it is complete
  - either resume/commit or discard with explicit user-facing message

### Performance

- For large tile sets, saving should stream data where possible.
- Avoid re-encoding unchanged assets (content-addressed or checksum-based skip, if feasible).

## Dependencies

- Document format/container decisions (`vdd-legacy-format`).
- Asset/tiling output behavior (`vdd-legacy-image-pipeline`).

## Testing Strategy

### Unit Tests

- [ ] Atomic writer commit/rollback behavior
- [ ] Workspace path mapping determinism

### Integration Tests

- [ ] Simulate crash mid-save; confirm old bundle remains valid and recovery works

## Open Design Questions

- [ ] Extract-to-disk workspace vs virtual filesystem abstraction?
- [ ] How to bound number of files (tiles) and handle platform file limits?

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
