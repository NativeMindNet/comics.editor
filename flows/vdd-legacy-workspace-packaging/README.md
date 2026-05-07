# workspace & packaging (reliable open/save)

> Client-Facing Documentation  
> Last Updated: 2026-05-08  
> Version: 1.0

## What This Feature Does

This work makes opening and saving projects reliable across platforms by defining how the editor stores files internally and how it writes a final single “project file” safely.

---

## How It Works

**In Simple Terms:**

1. When you open a project file, the editor prepares a working folder (workspace).
2. While you edit, all changes go into that workspace.
3. When you save, the editor writes a brand-new project file and only replaces the old one when the new one is complete.

---

## Key Benefits

- **No silent corruption**: saves are atomic (all-or-nothing).
- **Better recovery**: if something interrupts saving, the editor can recover safely.
- **Cross-platform**: no Windows-only packaging tools.

---

## Getting Started

1. Open a project file.
2. Edit as usual.
3. Save — the editor handles safe packaging automatically.

---

**Note for Stakeholders**: Technical details and progress are tracked in `05-implementation-log.md`.
