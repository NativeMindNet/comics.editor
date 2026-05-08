# ADR-004: Atomic Save Strategy

## Meta

- **Number**: ADR-004
- **Type**: enabling
- **Status**: DRAFT
- **Created**: 2026-05-08
- **Decided**: -
- **Author**: /legacy analysis
- **Reviewers**: -

## Context

Current save implementation has a critical flaw:

```csharp
public void Save()
{
    Comics.Save();              // 1. Write data.json to temp
    File.Delete(FilePath);      // 2. Delete old file
    ZipUtils.Zip(..., FilePath); // 3. Create new ZIP
}
```

**Problem**: If the application crashes or loses power between steps 2 and 3:
- Old file is deleted
- New file is not fully written
- **Data is lost**

This affects both WPF (7za.exe) and Unity (ZipFile) implementations.

## Decision Drivers

- **Data safety**: User work must never be lost due to save failure
- **Reliability**: Save must be atomic (all-or-nothing)
- **Recovery**: If save fails, previous version must remain intact
- **Performance**: Atomic save should not significantly slow down workflow

## Considered Options

### Option 1: Write-to-Temp-then-Rename

**Description**: Write to a temporary file, then atomically rename to target:

```csharp
public void Save()
{
    string tempPath = FilePath + ".tmp";
    Comics.Save();
    ZipUtils.Zip(..., tempPath);

    // Atomic on most filesystems
    File.Delete(FilePath);
    File.Move(tempPath, FilePath);
}
```

**Pros**:
- Simple implementation
- Atomic rename on POSIX and modern Windows
- Minimal performance overhead

**Cons**:
- Rename not fully atomic on all Windows configurations
- Temp file left behind on crash (needs cleanup)

**Estimated Effort**: Low

### Option 2: Backup-before-Write

**Description**: Create backup of existing file before overwriting:

```csharp
public void Save()
{
    string backupPath = FilePath + ".bak";
    if (File.Exists(FilePath))
        File.Copy(FilePath, backupPath);

    Comics.Save();
    ZipUtils.Zip(..., FilePath);

    File.Delete(backupPath);
}
```

**Pros**:
- Previous version preserved during write
- Can recover from backup if needed

**Cons**:
- Doubles disk I/O (copy before write)
- Backup file needs cleanup
- Not truly atomic

**Estimated Effort**: Low

### Option 3: Transactional NTFS (Windows Only)

**Description**: Use Windows Transactional NTFS (TxF) for atomic write.

**Pros**:
- True atomicity
- OS-level guarantee

**Cons**:
- Windows-only (deprecated by Microsoft)
- Complex API
- Not available on Unity platforms

**Estimated Effort**: High

### Option 4: Journal-based Save

**Description**: Write save intent to journal file, perform save, clear journal:

```csharp
public void Save()
{
    File.WriteAllText(FilePath + ".saving", "in_progress");

    string tempPath = FilePath + ".new";
    ZipUtils.Zip(..., tempPath);

    File.Delete(FilePath);
    File.Move(tempPath, FilePath);

    File.Delete(FilePath + ".saving");
}
```

On startup, check for `.saving` marker and recover:
```csharp
if (File.Exists(path + ".saving"))
{
    // Save was interrupted
    if (File.Exists(path + ".new"))
        File.Move(path + ".new", path);  // Complete interrupted save
    File.Delete(path + ".saving");
}
```

**Pros**:
- Recovery on next launch
- Works cross-platform
- Clear intent marker

**Cons**:
- Slightly more complex
- Requires startup check

**Estimated Effort**: Medium

## Decision

**[PENDING DECISION]**

Recommended: **Option 4 (Journal-based Save)** for full robustness, or **Option 1 (Write-to-Temp-then-Rename)** for simpler immediate fix.

## Consequences

### Positive

- User data protected from crashes
- Previous version preserved until new version complete
- Recoverable from interrupted saves

### Negative

- Slight complexity increase
- Need cleanup logic for temp/marker files

### Neutral

- Save time unchanged (write + rename vs write + delete + write)

## Implementation Notes

- Create `AtomicWriter` utility class for reuse
- Add recovery check to application startup
- Log recovery actions for debugging
- Consider auto-save with same atomic guarantees

## Related Decisions

- ADR-005: Schema Versioning (save format changes)

## Related Specs

- `flows/vdd-legacy-workspace-packaging/`: Workspace save behavior

## References

- Atomic file operations: https://lwn.net/Articles/457667/
- File.Move atomicity: https://devblogs.microsoft.com/oldnewthing/20151028-00/?p=91751

## Tags

reliability data-safety filesystem

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
