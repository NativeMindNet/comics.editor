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

## Legacy Implementation Details
> Added by /legacy on 2026-05-08

### Current Temp Directory Locations

| Platform | Path | Notes |
|----------|------|-------|
| WPF | `%LOCALAPPDATA%\Comics Editor\Temp` | Windows-specific |
| Unity | `Application.temporaryCachePath + /ComicsUnityEditor` | Cross-platform |

### Cleanup Retry Logic

Both platforms use 10-attempt retry with 100ms delays for folder deletion:

```csharp
public static void DeleteFolder()
{
    const int maxRetries = 10;
    const int delayMs = 100;

    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            if (Directory.Exists(TempFolder))
                Directory.Delete(TempFolder, recursive: true);
            return;
        }
        catch (Exception)
        {
            Thread.Sleep(delayMs);  // Handle file locks
        }
    }
}
```

**Purpose:** Handles Windows file handle retention from previous sessions.

### External Tool Dependencies (WPF Only)

| Tool | Command | Purpose |
|------|---------|---------|
| 7za.exe | `a -tzip -mx0 "{output}" "{temp}\*"` | Pack bundle (no compression) |
| 7za.exe | `x "{archive}" -o"{temp}"` | Extract bundle |

**Location:** `Utils\7za.exe` (relative to binary)

**Unity Replacement:** `System.IO.Compression.ZipFile` API

### Current Save Flow (No Atomic Write)

**WPF Current Implementation:**
```csharp
public void Save()
{
    // 1. Save metadata
    Comics.Save();  // Writes data.json to TempFolder

    // 2. Delete old file
    if (File.Exists(FilePath))
        File.Delete(FilePath);

    // 3. Create new ZIP
    ZipUtils.Zip(TempFolder + "/*", FilePath, compressionLevel: 0);
}
```

**GAP IDENTIFIED:** No atomic write strategy. If crash occurs during Step 3:
- Old file deleted (Step 2)
- New file partially written
- Data loss occurs

**Proposed Fix (not yet implemented):**
```csharp
public void Save()
{
    string tempPath = FilePath + ".tmp";

    Comics.Save();
    ZipUtils.Zip(TempFolder + "/*", tempPath, 0);

    // Atomic replace (Windows MoveFileEx or POSIX rename)
    AtomicReplace(tempPath, FilePath);
}
```

### Workspace Folder Structure

```
TempFolder/
├── layers/           # Image tiles
│   ├── image_1000_0_0.jpg
│   ├── image_1000_1_0.jpg
│   ├── image_500_0_0.jpg    # 0.5x scale
│   └── ...
├── sounds/           # Audio files
│   └── music.mp3
└── data.json         # Document metadata
```

### WPF vs Unity Comparison

| Aspect | WPF | Unity |
|--------|-----|-------|
| ZIP library | 7za.exe (external) | ZipFile (built-in) |
| Image processing | ImageMagick (external) | Texture2D API (built-in) |
| Compression level | 0 (store mode) | CompressionLevel.Fastest |
| Output format | Preserves JPG/PNG | PNG only |
| Temp path | %LOCALAPPDATA% | Application.temporaryCachePath |
| Atomic save | Not implemented | Not implemented |

### Missing Recovery Mechanism

**Current State:**
- No detection of incomplete saves
- No temp file preservation
- No user-facing recovery prompt

**Required for v2:**
- Save marker file (e.g., `.saving` flag)
- Recovery prompt on next open if marker exists
- Option to recover workspace or discard

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
