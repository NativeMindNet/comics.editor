# Implementation Plan: Unity parity gaps overview

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Specifications: [02-specifications.md](./02-specifications.md)

## Summary

Execute child SDD flows in dependency order (see overview specs). This plan only tracks **rollout sequencing** and **exit criteria** for “parity acceptable for release X”.

## Task Breakdown

### Phase 1: Baseline integrity

#### Task 1.1: Complete `sdd-unity-asset-pipeline-fidelity`
- **Description**: Zip + tiling + JSON deserialization validated against real legacy archives.
- **Dependencies**: None
- **Verification**: Golden-file or sample-based tests; manual open in WPF after Unity save.
- **Complexity**: High

### Phase 2: Authoring UX core

#### Task 2.1: Complete `sdd-unity-canvas-preview-transforms`
- **Description**: Single preview canvas reflects translate/rotate/scale/alpha (+ pivot rules).
- **Dependencies**: Task 1.1 (stable assets)
- **Verification**: Side-by-side scroll scrub vs WPF screenshot (tolerance documented).
- **Complexity**: High

#### Task 2.2: Complete `sdd-unity-animation-timeline-ui`
- **Description**: CRUD for all anim types; segment selection; popup image workflow.
- **Dependencies**: Task 2.1
- **Verification**: Create document in Unity; open in WPF without data loss.
- **Complexity**: High

### Phase 3: Media + safety

#### Task 3.1: Complete `sdd-unity-audio-preview`
- **Dependencies**: Task 2.1
- **Complexity**: Medium

#### Task 3.2: Complete `sdd-unity-undo-redo`
- **Dependencies**: Tasks 2.2–3.1 (command boundaries known)
- **Complexity**: High

## Exit criteria (release gate)

- [ ] Asset round-trip: N sample documents pass WPF ↔ Unity ↔ WPF
- [ ] Canvas: scroll reflects composed transforms within documented epsilon
- [ ] Animation: all `AnimTypes` editable without manual JSON edits
- [ ] Audio: preview matches SoundAnim windows at sample rates specified in child spec
- [ ] Undo: at least model + file-backed layer ops reversible

## Risk Assessment

| Risk | Mitigation |
|------|------------|
| Fidelity arguments block shipping | define tolerance tiers in asset-pipeline flow |

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
