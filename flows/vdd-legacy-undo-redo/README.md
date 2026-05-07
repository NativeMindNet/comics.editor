# undo/redo (safe editing)

> Client-Facing Documentation  
> Last Updated: 2026-05-08  
> Version: 1.0

## What This Feature Does

This work adds Undo and Redo to the editor so you can safely experiment while editing scenes, animations, and assets.

---

## How It Works

**In Simple Terms:**

1. Every action you do is recorded as a step in a history list.
2. Undo goes one step back.
3. Redo goes one step forward.
4. A long action like dragging an object is treated as a single step (not hundreds).

---

## Key Benefits

- **Safer editing**: mistakes are easy to revert.
- **Faster iteration**: try ideas without fear.
- **More predictable UX**: one gesture = one undo step.

---

## Quick Example

**Goal**: Try a new position for a character.

**Steps**:
1. Move the character.
2. Decide it looked better before.
3. Press Undo.

**Result**: The character returns exactly to the previous position.

---

## Getting Started

1. Use **Undo** after any edit you want to revert.
2. Use **Redo** if you undo too far.

---

**Note for Stakeholders**: Technical details and progress are tracked in `05-implementation-log.md`.
