# Requirements: Unity Comics Editor undo/redo

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08

## Problem Statement

Editing mutates `ComicsDocument` and files under the temp workspace **without** a safety net. Unlike WPF (which also lacked undo), we want Unity to provide **Undo/Redo** to reduce data loss during experimentation.

## User Stories

**As a** author  
**I want** to undo mistaken layer deletes, bad anim edits, or wrong imports  
**So that** I can work faster with less fear  

**As a** developer  
**I want** edit operations expressed as commands  
**So that** history stays deterministic and testable  

## Acceptance Criteria

### Must Have

1. **Given** a model-only edit (e.g. change anim end time)  
   **When** Undo then Redo  
   **Then** document JSON state matches exactly before/after snapshot  

2. **Given** an asset edit (import/replace image)  
   **When** Undo  
   **Then** filesystem and model references are restored together (no dangling paths)  

3. **Given** a continuous drag gesture (future canvas)  
   **When** Undo  
   **Then** the entire gesture reverts as **one** step  

### Should Have

- History panel with human-readable labels.  
- Configurable max history depth.  

### Won’t Have (This Iteration)

- Persistent undo across Unity restarts.

## Constraints

- Memory bounded; avoid full zip snapshot per step for large docs.
- Must not corrupt `.comics` on disk until explicit Save (undo is session-scoped).

## Open Questions

- [ ] Include “Save” as non-undoable barrier?
- [ ] Coalesce typing in numeric fields?

## References

- VDD: `flows/vdd-legacy-undo-redo/`
- Related: `flows/sdd-unity-animation-timeline-ui/` (commands)

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
