# Implementation Plan: Unity audio preview

> Version: 2.0
> Status: DRAFT
> Last Updated: 2026-05-08
> Specifications: [02-specifications.md](./02-specifications.md)

## Overview

Implement scroll-driven audio preview with EditorAudioManager, toolbar toggle, and scroll change detection.

## Phases

### Phase 1: EditorAudioManager Core

**Tasks:**
1. Create `Editor/Audio/EditorAudioManager.cs`
2. Implement clip cache dictionary and AudioSource management
3. Implement `Initialize(soundsFolderPath)` - create hidden GameObject, scan folder
4. Implement `LoadClip()` coroutine using UnityWebRequestMultimedia
5. Implement `Dispose()` - cleanup clips and GameObject

**Files:**
- NEW: `Assets/Editor/Audio/EditorAudioManager.cs`

### Phase 2: ProcessScroll Logic

**Tasks:**
1. Implement `ProcessScroll(sounds, prevScroll, currentScroll)`
2. Implement `GetOrCreateSource(sound)` - lazy AudioSource creation
3. Add debounce logic (50ms window)
4. Handle looping vs one-shot based on Start==End
5. Implement `SetEnabled(bool)` and `StopAll()`

**Files:**
- MODIFY: `Assets/Editor/Audio/EditorAudioManager.cs`

### Phase 3: Session Integration

**Tasks:**
1. Add `DisableSound` property to `ComicsEditorSession`
2. Add `PreviousScroll` property for change detection

**Files:**
- MODIFY: `Assets/ComicsEditorSession.cs`

### Phase 4: Window Integration

**Tasks:**
1. Add `_audioManager` field to `ComicsEditorWindow`
2. Add lifecycle: `OnEnable` initialize, `OnDisable` dispose
3. Add Sound toggle to toolbar
4. Add scroll change detection in `OnGUI` or `DrawLeftPanel`
5. Wire `ProcessScroll` call on scroll changes

**Files:**
- MODIFY: `Assets/Editor/ComicsEditorWindow.cs`

## Task Summary

| # | Task | Phase |
|---|------|-------|
| 1 | Create EditorAudioManager class | 1 |
| 2 | Implement clip loading coroutine | 1 |
| 3 | Implement Initialize/Dispose | 1 |
| 4 | Implement ProcessScroll | 2 |
| 5 | Implement GetOrCreateSource | 2 |
| 6 | Add debounce and looping logic | 2 |
| 7 | Add session properties | 3 |
| 8 | Add window lifecycle | 4 |
| 9 | Add toolbar toggle | 4 |
| 10 | Wire scroll detection | 4 |

## Dependencies

- `SoundModel` and `SoundAnim` classes from shared core
- `FileManagerUnity.TempFolder` for sounds path

## Risks

| Risk | Mitigation |
|------|------------|
| Audio clips not loading in editor | Use file:// protocol with UnityWebRequestMultimedia |
| Coroutine in non-MonoBehaviour | Use EditorCoroutineUtility from Unity.EditorCoroutines |

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
