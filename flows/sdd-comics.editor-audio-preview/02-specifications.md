# Specifications: Unity audio preview

> Version: 2.0
> Status: DRAFT
> Last Updated: 2026-05-08
> Requirements: [01-requirements.md](./01-requirements.md)

## Overview

Implement scroll-driven audio preview in the Unity editor using `AudioSource` and dynamically loaded `AudioClip` from mp3 files in the temp workspace.

## Affected Systems

| System | Impact |
|--------|--------|
| `ComicsEditorSession` | Add `DisableSound` property, `PreviousScroll` tracking |
| `ComicsEditorWindow` | Add sound toggle to toolbar, call audio manager on scroll |
| New: `EditorAudioManager` | Manages AudioClips, AudioSources, playback state |

## Architecture

```
ComicsEditorWindow
    │
    ├── OnScrollChanged() ──► EditorAudioManager.ProcessScroll(prev, current)
    │
    └── Toolbar: [x] Sound ──► EditorAudioManager.SetEnabled(bool)

EditorAudioManager
    │
    ├── Dictionary<string, AudioClip> _clipCache
    ├── Dictionary<SoundModel, AudioSource> _sources
    │
    └── ProcessScroll(prevScroll, currentScroll)
        │
        └── foreach sound in document.Sounds:
            ├── FindCurrent(sound.Animations, prevScroll, currentScroll)
            │
            ├── If trigger found AND not already playing → Play
            ├── If was looping AND no longer in range → Stop
            └── Debounce via timestamp check
```

## Component: EditorAudioManager

```csharp
public class EditorAudioManager : IDisposable
{
    private readonly Dictionary<string, AudioClip> _clipCache;
    private readonly Dictionary<SoundModel, AudioSource> _sources;
    private GameObject _audioHost;
    private bool _enabled = true;
    private double _lastProcessTime;
    private const double DebounceMs = 50;

    public void Initialize(string soundsFolderPath);
    public void ProcessScroll(IList<SoundModel> sounds, double prevScroll, double currentScroll);
    public void SetEnabled(bool enabled);
    public void StopAll();
    public void Dispose();
}
```

### Methods

#### `Initialize(soundsFolderPath)`
- Create hidden `GameObject` with `HideFlags.HideAndDontSave`
- Preload AudioClips for all mp3 files in folder using `UnityWebRequestMultimedia`

#### `ProcessScroll(sounds, prevScroll, currentScroll)`
```csharp
if (!_enabled) return;
if (Time.realtimeSinceStartupAsDouble - _lastProcessTime < DebounceMs / 1000.0)
    return; // debounce (except for stop)

foreach (var sound in sounds)
{
    var anim = SoundAnim.FindCurrent(sound.Animations, prevScroll, currentScroll);
    var source = GetOrCreateSource(sound);

    if (anim != null)
    {
        if (!source.isPlaying)
        {
            source.loop = anim.Start != anim.End;
            source.Play();
        }
    }
    else if (source.isPlaying && source.loop)
    {
        source.Stop(); // immediate stop, no debounce
    }
}
_lastProcessTime = Time.realtimeSinceStartupAsDouble;
```

#### `GetOrCreateSource(sound)`
- Look up or create `AudioSource` component on `_audioHost`
- Assign `AudioClip` from cache by filename
- Configure: `playOnAwake = false`, `spatialBlend = 0` (2D)

#### `SetEnabled(enabled)`
- If disabling: `StopAll()`
- Update `_enabled` flag

#### `Dispose()`
- Destroy all AudioClips in cache
- Destroy `_audioHost` GameObject

## Audio Loading

```csharp
private IEnumerator LoadClip(string filePath, Action<AudioClip> onLoaded)
{
    using var request = UnityWebRequestMultimedia.GetAudioClip(
        "file://" + filePath, AudioType.MPEG);
    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.Success)
    {
        var clip = DownloadHandlerAudioClip.GetContent(request);
        clip.name = Path.GetFileName(filePath);
        onLoaded(clip);
    }
}
```

## Integration with ComicsEditorWindow

### Toolbar Addition
```csharp
// In toolbar
_session.DisableSound = !GUILayout.Toggle(!_session.DisableSound, "Sound", EditorStyles.toolbarButton);
if (GUI.changed)
    _audioManager?.SetEnabled(!_session.DisableSound);
```

### Scroll Change Detection
```csharp
// In DrawLeftPanel or OnGUI
var prevScroll = _session.PreviousScroll;
var newScroll = _session.Scroll;
if (Math.Abs(newScroll - prevScroll) > 0.1)
{
    _audioManager?.ProcessScroll(_session.Document.Sounds, prevScroll, newScroll);
    _session.PreviousScroll = newScroll;
}
```

### Lifecycle
```csharp
void OnEnable()
{
    _audioManager = new EditorAudioManager();
    if (Directory.Exists(FileManagerUnity.TempFolder))
        _audioManager.Initialize(Path.Combine(FileManagerUnity.TempFolder, "sounds"));
}

void OnDisable()
{
    _audioManager?.Dispose();
    _audioManager = null;
}
```

## Edge Cases

| Case | Behavior |
|------|----------|
| No sounds in document | No-op, no errors |
| Missing mp3 file | Log warning, skip that sound |
| Rapid scroll back-and-forth | Debounce prevents thrash |
| Start==End point trigger | Play once when scroll crosses point (forward only) |
| Scroll backwards through trigger | Don't re-trigger (matches WPF behavior) |
| Window closed during playback | Dispose stops all, destroys clips |

## Testing Strategy

- [ ] Add sound, scroll into range → plays
- [ ] Scroll out of looping range → stops
- [ ] Toggle DisableSound → stops immediately
- [ ] Start==End trigger → plays once on forward scroll
- [ ] Rapid scroll → no audio thrashing
- [ ] Close window → no audio leaks

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
