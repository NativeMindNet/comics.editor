# Specifications: Unity audio preview

> Version: 1.0  
> Status: DRAFT  
> Last Updated: 2026-05-08  
> Requirements: [01-requirements.md](./01-requirements.md)

## Overview

Introduce a small **Editor audio service** driven by `Scroll` (and optional playhead time) that computes active `SoundAnim` segments and issues play/stop to Unity audio backend.

## Affected Systems

| System | Impact |
|--------|--------|
| `ComicsEditorWindow` | Play/Stop/Mute UI |
| `ComicsEditorSession` | Holds `DisableSound`, previous scroll for FindCurrent |
| New `ComicsAudioPreviewService` | Create |

## Architecture

```
Scroll (t)  ─► evaluator: SoundAnim.FindCurrent / segment coverage
                    │
                    ▼
            scheduler (debounced)
                    │
                    ▼
         AudioSource or PlayClipAtPoint (Editor)
```

## Interfaces (conceptual)

```csharp
interface IAudioPreview {
  void SetScroll(double scroll, double prevScroll);
  void SetSounds(IReadOnlyList<SoundModel> sounds, string tempSoundsFolder);
  void SetDisabled(bool disabled);
  void Tick(); // if using play mode clock
}
```

## Behavior

- On segment enter: play from start or seek policy as per requirements doc.
- On segment exit: stop (or fade if future).
- Debounce rapid scrub: max N transitions/sec (TBD).

## Testing Strategy

- [ ] Unit: evaluator transitions for synthetic segment sets
- [ ] Manual: headphone sanity on two OSes

## Open Design Questions

- [ ] Use `AudioUtil`/`PreviewDriver` internal Unity APIs? (Avoid private APIs if possible.)

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
