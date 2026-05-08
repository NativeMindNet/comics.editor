# image pipeline (import, tiling, preview)

> Client-Facing Documentation  
> Last Updated: 2026-05-08  
> Version: 1.0

## What This Feature Does

This work makes large images fast and reliable in the editor by automatically splitting them into small “tiles” and only loading what’s visible.

It improves performance when panning, zooming, and working with very large backgrounds or detailed art.

---

## How It Works

**In Simple Terms:**

1. When you import an image, the editor cuts it into many small squares (tiles).
2. While you work, the editor only loads the tiles that are currently on screen.
3. As you pan or zoom, it swaps tiles in and out automatically.

---

## Key Benefits

- **Faster editing**: Smooth pan/zoom on huge images.
- **Lower memory use**: No need to build a single giant bitmap in memory.
- **More stable**: Large assets are less likely to crash the app.

---

## Quick Example

### Example Scenario

**Goal**: Import a very large background image for a puzzle.

**Steps**:
1. Import the image.
2. Choose the tiling option (default).
3. Start editing immediately while tiles load in.

**Result**: The canvas stays responsive even with massive images.

---

## Common Questions

### Will this change how I work?
Not really. Import and editing stay the same; tiling happens automatically (with options if needed).

### Can I cancel tile generation?
Yes. The tiling process is designed to run in the background with progress and cancel.

---

## Getting Started

1. Import an image into a layer.
2. Keep the default tiling settings unless you have a special case.
3. Pan/zoom as usual — tiles will stream automatically.

---

**Note for Stakeholders**: This documentation focuses on practical usage and benefits. For technical implementation details, see `05-implementation-log.md`.
