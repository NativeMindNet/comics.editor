# rendering & interaction engine (canvas, selection, transforms)

> Client-Facing Documentation  
> Last Updated: 2026-05-08  
> Version: 1.0

## What This Feature Does

This work defines how the editor draws your scene and how you interact with it: selecting layers, moving them, rotating/scaling, and smoothly navigating a large canvas.

---

## How It Works

**In Simple Terms:**

1. Each element in your scene is a “layer”.
2. The editor draws layers in order, like stacking transparent sheets.
3. When you tap/click a layer, the editor selects it and shows handles.
4. Dragging or using handles updates the layer’s position, rotation, and size.

---

## Key Benefits

- **Precise editing**: Reliable selection and transformations.
- **Smooth navigation**: Fast pan/zoom even in complex scenes.
- **Predictable behavior**: The same action always selects the same layer.

---

## Quick Example

### Example Scenario

**Goal**: Move a character in front of the background.

**Steps**:
1. Click the character layer to select it.
2. Drag it to the new position.
3. If needed, rotate/scale using the handles.

**Result**: The scene updates instantly and stays responsive.

---

## Common Questions

### Why does it feel faster on large images?
The editor draws only the visible parts (tiles) instead of loading one huge image all at once.

### Can I still do the same edits as in the old editor?
Yes, the goal is parity for core transforms and selection first, then iterative improvements.

---

## Getting Started

1. Open a document.
2. Select a layer on the canvas.
3. Drag to move; use handles to rotate/scale.

---

**Note for Stakeholders**: This documentation focuses on practical usage and benefits. For technical implementation details, see `05-implementation-log.md`.
