# legacy document format v2 (comics/puzzle)

> Client-Facing Documentation  
> Last Updated: 2026-05-08  
> Version: 1.0

## What This Feature Does

This work defines a new, safer file format for comics and puzzle documents so they can be opened and edited consistently on any device.

It keeps all images, sounds, and animations bundled together, but makes the format easier to evolve without breaking older content.

---

## How It Works

**In Simple Terms:**

1. Think of a document like a “folder in one file”.
2. Inside it there’s a description of the scene (a JSON file) and the media files (images and sounds).
3. The editor reads that bundle, checks if anything is missing, and then opens it.
4. If the document is from an older version, the editor upgrades it safely before editing.

---

## Key Benefits

- **Fewer broken files**: Versioning and validation catch problems early.
- **Works everywhere**: No Windows-only tools are required.
- **Safer upgrades**: Older documents keep working as the editor evolves.

---

## Quick Example

### Example Scenario

**Goal**: Open an old episode file and keep editing it.

**Steps**:
1. Choose the `.comics` file in the editor.
2. The editor detects it’s an older format and upgrades it automatically.
3. You edit and save as usual.

**Result**: Your file remains readable and future-proof.

---

## Common Questions

### Will my old files still work?
Yes. The editor supports importing older documents and upgrading them when opened.

### What happens if some images are missing?
The editor will show a clear warning and explain what’s missing. Depending on the chosen policy, it can either block opening the file or open it in a “best effort” mode so you can repair it.

### Do I need to do anything differently?
No. You still open, edit, and save documents the same way. The format improvements are mostly behind the scenes.

---

## What's Next

After the format is finalized, we’ll connect it to the rendering and editing engine so large scenes load quickly (by streaming tiles) and edits stay fast.

---

## Getting Started

1. Open a `.comics` or `.puzzle` file.
2. If prompted, allow an automatic upgrade.
3. Edit and save as normal.

---

**Note for Stakeholders**: This documentation focuses on practical usage and benefits. For technical implementation details, see `05-implementation-log.md`.
