# Requirements: undo/redo + transactional editing (new system)

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08

## Problem Statement

The legacy editor does not provide undo/redo as a first-class capability, which makes editing error-prone and slows iteration. In the Flutter rewrite, editing will involve complex operations (transforms, animation edits, asset imports/tiling) that must be reversible.

We need a consistent **undo/redo system** that treats both document model changes and asset/workspace changes as reversible operations.

## User Stories

### Primary

**As a** creator  
**I want** to undo and redo my last actions  
**So that** I can experiment safely and quickly

### Secondary

- **As a** creator  
  **I want** a single drag/multi-step gesture to undo as one action  
  **So that** undo feels natural (not “100 undos” for one move)

- **As a** developer  
  **I want** deterministic action logging and clear transaction boundaries  
  **So that** issues can be reproduced and fixed reliably

## Acceptance Criteria

### Must Have

1. **Given** a user performs an edit action (move/rotate/scale, add/remove layer, edit animation)  
   **When** Undo is invoked  
   **Then** the document returns to the exact previous state

2. **Given** an Undo has been performed  
   **When** Redo is invoked  
   **Then** the document returns to the state before Undo

3. **Given** a continuous gesture (e.g., dragging a layer)  
   **When** Undo is invoked  
   **Then** the whole gesture is undone as a single step

4. **Given** an asset-affecting operation (import image, delete sound, retile)  
   **When** Undo is invoked  
   **Then** both model references and underlying asset state are restored consistently

### Should Have

- “Undo stack” UI list with readable action names.
- Coalescing repeated small edits (typing or nudges) into a single undo step within a time window.

### Won't Have (This Iteration)

- Cross-session persistent undo (history saved into the document).

## Constraints

- **Performance**: undo/redo should be instant for typical edits; large asset operations must remain safe.
- **Correctness**: no partial state (model points to asset that no longer exists).
- **Memory**: history must be bounded; strategy needed for large data.

## Open Questions

- [ ] Architecture: command pattern vs state snapshots/diffs vs hybrid?
- [ ] How to represent asset operations: copy-on-write, reference counting, or journaling?
- [ ] How to handle undo when background jobs (tiling) are running?

## References

- Legacy has no clear undo subsystem; edits happen via ViewModels and direct filesystem changes in temp workspace.
- Rendering and tools flows: `flows/vdd-legacy-rendering/*`
- Image tiling flows: `flows/vdd-legacy-image-pipeline/*`

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
- [ ] Notes: [any conditions or clarifications]
