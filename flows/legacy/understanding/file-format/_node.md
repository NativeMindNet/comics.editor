# Understanding: File Format & Workspace

## Phase: SYNTHESIZING

## Hypothesis

ZIP-based bundle format (.comics/.puzzle) with workspace extraction to temp directory, using external tools (7za, ImageMagick) on WPF and built-in APIs on Unity.

## Sources

- `app/unity_comics.editor/Comics.Editor/IWS/` - WPF workspace management
- `app/unity_comics.editor/Comics.Editor/IWS/Utils/ZipUtils.cs` - 7za wrapper
- `app/unity_comics.editor/Comics.Editor/IWS/Utils/FileManager.cs` - Temp directory handling
- `app/unity_comics.editor/Comics.Editor/IWS/Utils/ImageMagick.cs` - Tile generation
- `app/unity_comics.editor/UnityComicsEditor/Assets/ComicsUnity/Editor/FileManagerUnity.cs`
- `app/unity_comics.editor/UnityComicsEditor/Assets/ComicsUnity/Editor/TileGeneratorUnity.cs`
- `app/unity_comics.editor/UnityComicsEditor/Assets/ComicsUnity/Editor/ZipUtility.cs`

## Validated Understanding

### Bundle Structure

```
.comics / .puzzle (ZIP archive)
├── data.json          # Document metadata (JSON)
├── layers/            # Image tiles
│   ├── image_{scale}_{col}_{row}.jpg  # Tiled images
│   └── image.jpg                       # Single images
└── sounds/            # Audio files
    └── audio.mp3
```

### Workspace Lifecycle

**Open Flow:**
```
Open file → Clean temp folder → Extract ZIP → Create folders → Load data.json → Display
```

**Save Flow:**
```
Save data.json → Delete old file → ZIP temp folder → Write to path
```

### Tile Naming Convention

**Pattern:** `{baseName}_{scale}_{col}_{row}.{ext}`

| Parameter | Description |
|-----------|-------------|
| scale | 1000 = 100%, 500 = 50%, 250 = 25%, 125 = 12.5% |
| col | Tile column (0-based) |
| row | Tile row (0-based) |

**Scales:**
- Comics: [1.0] - single scale
- Puzzle: [1.0, 0.5, 0.25, 0.125] - multi-scale pyramid

**Tile Size:** 512×512 pixels

### External Tool Dependencies (WPF)

| Tool | Location | Purpose |
|------|----------|---------|
| 7za.exe | Utils\7za.exe | ZIP pack/unpack |
| magick.exe | Utils\ImageMagick\magick.exe | Resize, tile, identify |

**ImageMagick Commands:**
- Resize: `magick "{src}" -resize WxH^ -gravity center -extent WxH "{dst}"`
- Tile: `magick "{src}" -crop TxT -set filename:tile "%[fx:page.x/T]_%[fx:page.y/T]" +repage +adjoin "{template}"`
- Size: `identify -format "%[fx:w]x%[fx:h]" "{src}"`

### Unity Replacements

Unity eliminates external dependencies:

| WPF | Unity |
|-----|-------|
| 7za.exe | System.IO.Compression.ZipFile |
| ImageMagick resize | Texture2D + RenderTexture.Blit |
| ImageMagick tile | GetPixels() + SetPixels() |
| JPG/PNG output | EncodeToPNG() only |

### Temp Directory Locations

| Platform | Path |
|----------|------|
| WPF | %LOCALAPPDATA%\Comics Editor\Temp |
| Unity | Application.temporaryCachePath + /ComicsUnityEditor |

**Cleanup:** Retry 10x with 100ms delays (handles file locks)

## Children Identified

| Child | Hypothesis | Status |
|-------|------------|--------|
| (none) | File format is leaf node | - |

## Dependencies

- **Uses**: document-model (data.json schema)
- **Used by**: canvas-rendering (tile loading), workspace-management

## Key Insights

1. **No atomic save**: File can be corrupted if crash during ZIP write
2. **No schema versioning**: data.json has no version field; breaking changes are breaking
3. **Index-based cultures**: [0]=En, [1]=Ru, [2]=Hi - fragile if order changes
4. **Pattern-based tiles**: `image_{0}_{1}_{2}.jpg` discovered by string parsing, not manifest
5. **Unity outputs PNG only**: WPF preserves input format; interop risk
6. **No compression on WPF**: Level 0 = store mode for speed (tiles already compressed)

## ADR Candidates

1. **Atomic save strategy**: Write to temp file, then atomic rename
2. **Schema versioning**: Add `schemaVersion: 2` to data.json
3. **Manifest-driven tiles**: Explicit tile inventory with checksums
4. **Explicit culture mapping**: Use `"images": {"en": {...}}` not array indexes
5. **Image format standardization**: PNG everywhere vs. preserve original
6. **External tool elimination**: Unity approach vs. WPF ImageMagick dependency

## Flow Recommendation

- **Type**: VDD (existing flows: vdd-legacy-format, vdd-legacy-workspace-packaging)
- **Confidence**: high
- **Rationale**: Existing flows partially cover; gaps in implementation details

## Flow Updates Required

| Flow | Action | Additions |
|------|--------|-----------|
| vdd-legacy-workspace-packaging/03-specifications.md | APPEND | Temp directory paths, cleanup retry logic, atomic save gap |
| vdd-legacy-format/03-specifications.md | Already updated | Animation serialization details |

## Synthesis

### Combined Understanding

The file format is a standard ZIP with JSON metadata and asset folders. Key architecture:

- **Container**: ZIP archive (.comics/.puzzle)
- **Metadata**: data.json with Newtonsoft JSON serialization
- **Assets**: layers/ (tiles) and sounds/ (mp3)
- **Extraction**: To temp folder for editing; repack on save

Critical gaps:
- No atomic save (data loss risk)
- No schema version (migration risk)
- External tool dependencies on WPF (cross-platform risk)

Unity implementation is cleaner (built-in APIs) but outputs PNG-only.

## Bubble Up

- ZIP-based bundle format with data.json metadata
- WPF requires external tools (7za, ImageMagick); Unity uses built-in APIs
- No atomic save or schema versioning - risk of data loss/corruption
- Tile pattern `{0}_{1}_{2}` is discovered by string parsing, not manifest
- Unity outputs PNG only vs. WPF preserves format - interop consideration

---

*Phase: SYNTHESIZING | Depth: 2 | Parent: / (root)*
