# Specifications: Unity undo/redo system

> Version: 2.0
> Status: DRAFT
> Last Updated: 2026-05-08
> Requirements: [01-requirements.md](./01-requirements.md)

## Overview

Implement a command-based undo/redo system with support for model changes, asset operations, and gesture coalescing.

## Affected Systems

| System | Impact |
|--------|--------|
| `ComicsEditorSession` | Add `UndoStack`, execute commands through it |
| `ComicsEditorWindow` | Add keyboard handling, undo/redo buttons |
| New: `UndoStack` | Command history management |
| New: `IEditCommand` + implementations | Command pattern for all edits |

## Architecture

```
ComicsEditorWindow
    │
    ├── Ctrl+Z ──► _session.Undo()
    ├── Ctrl+Y ──► _session.Redo()
    │
    └── Edit operations ──► _session.Execute(command)

ComicsEditorSession
    │
    └── UndoStack
        ├── List<IEditCommand> _undoStack (max 50)
        ├── List<IEditCommand> _redoStack
        │
        └── Execute(cmd) → cmd.Execute() → push to _undoStack → clear _redoStack
```

## Component: IEditCommand

```csharp
public interface IEditCommand
{
    string Description { get; }
    void Execute();
    void Undo();

    // For coalescing
    bool CanMergeWith(IEditCommand other);
    IEditCommand MergeWith(IEditCommand other);
}
```

## Component: UndoStack

```csharp
public class UndoStack
{
    private readonly List<IEditCommand> _undoStack = new();
    private readonly List<IEditCommand> _redoStack = new();
    private readonly int _maxDepth;
    private double _lastCommandTime;
    private const double CoalesceWindowMs = 500;

    public UndoStack(int maxDepth = 50);

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;
    public string UndoDescription => _undoStack.Count > 0 ? _undoStack[^1].Description : null;
    public string RedoDescription => _redoStack.Count > 0 ? _redoStack[^1].Description : null;

    public void Execute(IEditCommand command);
    public void Undo();
    public void Redo();
    public void Clear();
}
```

### Execute Logic

```csharp
public void Execute(IEditCommand command)
{
    command.Execute();

    var now = Time.realtimeSinceStartupAsDouble;

    // Try to coalesce with previous command
    if (_undoStack.Count > 0 &&
        now - _lastCommandTime < CoalesceWindowMs / 1000.0)
    {
        var prev = _undoStack[^1];
        if (prev.CanMergeWith(command))
        {
            _undoStack[^1] = prev.MergeWith(command);
            _lastCommandTime = now;
            return;
        }
    }

    // Add new command
    _undoStack.Add(command);
    _redoStack.Clear();
    _lastCommandTime = now;

    // Enforce depth limit
    while (_undoStack.Count > _maxDepth)
    {
        var evicted = _undoStack[0];
        _undoStack.RemoveAt(0);
        (evicted as IDisposable)?.Dispose(); // cleanup backups
    }
}
```

### Undo/Redo Logic

```csharp
public void Undo()
{
    if (_undoStack.Count == 0) return;
    var cmd = _undoStack[^1];
    _undoStack.RemoveAt(_undoStack.Count - 1);
    cmd.Undo();
    _redoStack.Add(cmd);
}

public void Redo()
{
    if (_redoStack.Count == 0) return;
    var cmd = _redoStack[^1];
    _redoStack.RemoveAt(_redoStack.Count - 1);
    cmd.Execute();
    _undoStack.Add(cmd);
}
```

## Command Implementations

### UpdateAnimRangeCommand (with coalescing)

```csharp
public class UpdateAnimRangeCommand : IEditCommand
{
    private readonly Anim _anim;
    private readonly int _oldStart, _oldEnd;
    private readonly int _newStart, _newEnd;

    public UpdateAnimRangeCommand(Anim anim, int newStart, int newEnd)
    {
        _anim = anim;
        _oldStart = anim.Start;
        _oldEnd = anim.End;
        _newStart = newStart;
        _newEnd = newEnd;
    }

    public string Description => $"Change {_anim.Type} range";

    public void Execute()
    {
        _anim.Start = _newStart;
        _anim.End = _newEnd;
    }

    public void Undo()
    {
        _anim.Start = _oldStart;
        _anim.End = _oldEnd;
    }

    public bool CanMergeWith(IEditCommand other) =>
        other is UpdateAnimRangeCommand u && u._anim == _anim;

    public IEditCommand MergeWith(IEditCommand other)
    {
        var u = (UpdateAnimRangeCommand)other;
        return new UpdateAnimRangeCommand(_anim, u._newStart, u._newEnd)
        {
            // Keep original old values
        };
    }
}
```

### AddAnimCommand / RemoveAnimCommand

```csharp
public class AddAnimCommand : IEditCommand
{
    private readonly IList<Anim> _list;
    private readonly Anim _anim;

    public string Description => $"Add {_anim.Type}";

    public void Execute() => _list.Add(_anim);
    public void Undo() => _list.Remove(_anim);

    public bool CanMergeWith(IEditCommand other) => false;
    public IEditCommand MergeWith(IEditCommand other) => this;
}

public class RemoveAnimCommand : IEditCommand
{
    private readonly IList<Anim> _list;
    private readonly Anim _anim;
    private int _index;

    public string Description => $"Remove {_anim.Type}";

    public void Execute()
    {
        _index = _list.IndexOf(_anim);
        _list.Remove(_anim);
    }

    public void Undo() => _list.Insert(_index, _anim);

    public bool CanMergeWith(IEditCommand other) => false;
    public IEditCommand MergeWith(IEditCommand other) => this;
}
```

### SetLayerImageCommand (with backup)

```csharp
public class SetLayerImageCommand : IEditCommand, IDisposable
{
    private readonly LayerModel _layer;
    private readonly Cultures _culture;
    private readonly string _oldFile;
    private readonly string _newFilePath;
    private readonly string _backupPath;
    private readonly bool _isPuzzle;
    private readonly bool _isPopup;

    public string Description => $"Change layer image ({_culture})";

    public SetLayerImageCommand(LayerModel layer, Cultures culture,
        string newFilePath, bool isPuzzle, bool isPopup)
    {
        _layer = layer;
        _culture = culture;
        _isPuzzle = isPuzzle;
        _isPopup = isPopup;
        _newFilePath = newFilePath;

        // Capture old state
        var oldImage = _layer.GetImage(_culture, false);
        _oldFile = oldImage?.File;

        // Backup old file if exists
        if (!string.IsNullOrEmpty(_oldFile))
        {
            var oldPath = Path.Combine(FileManagerUnity.TempFolder,
                FileManagerUnity.FolderLayers, _oldFile);
            if (File.Exists(oldPath))
            {
                var undoDir = Path.Combine(FileManagerUnity.TempFolder, ".undo");
                Directory.CreateDirectory(undoDir);
                _backupPath = Path.Combine(undoDir, Guid.NewGuid() + Path.GetExtension(_oldFile));
                File.Copy(oldPath, _backupPath);
            }
        }
    }

    public void Execute()
    {
        _layer.SetImage(_culture, _newFilePath, _isPuzzle, _isPopup);
    }

    public void Undo()
    {
        if (_backupPath != null && File.Exists(_backupPath))
        {
            var destPath = Path.Combine(FileManagerUnity.TempFolder,
                FileManagerUnity.FolderLayers, _oldFile);
            File.Copy(_backupPath, destPath, true);
        }
        var image = _layer.GetImage(_culture, false);
        if (image != null) image.File = _oldFile;
    }

    public void Dispose()
    {
        if (_backupPath != null && File.Exists(_backupPath))
            File.Delete(_backupPath);
    }

    public bool CanMergeWith(IEditCommand other) => false;
    public IEditCommand MergeWith(IEditCommand other) => this;
}
```

## Integration with ComicsEditorSession

```csharp
public sealed class ComicsEditorSession
{
    public UndoStack UndoStack { get; } = new UndoStack(50);

    public void Execute(IEditCommand command)
    {
        UndoStack.Execute(command);
    }

    public void Undo() => UndoStack.Undo();
    public void Redo() => UndoStack.Redo();

    public void New(bool puzzle)
    {
        // ... existing code ...
        UndoStack.Clear();
    }

    public void Open(string path)
    {
        // ... existing code ...
        UndoStack.Clear();
    }

    public void Save()
    {
        // ... existing code ...
        UndoStack.Clear(); // Save is barrier
    }
}
```

## Integration with ComicsEditorWindow

### Keyboard Handling

```csharp
void HandleKeyboard()
{
    var evt = Event.current;
    if (evt.type != EventType.KeyDown) return;

    // Undo: Ctrl+Z (Cmd+Z on macOS)
    if (evt.control && evt.keyCode == KeyCode.Z && !evt.shift)
    {
        if (_session.UndoStack.CanUndo)
        {
            _session.Undo();
            InvalidatePreviews();
            evt.Use();
        }
    }
    // Redo: Ctrl+Y or Ctrl+Shift+Z
    else if ((evt.control && evt.keyCode == KeyCode.Y) ||
             (evt.control && evt.shift && evt.keyCode == KeyCode.Z))
    {
        if (_session.UndoStack.CanRedo)
        {
            _session.Redo();
            InvalidatePreviews();
            evt.Use();
        }
    }
    // ... existing Delete handling
}
```

### Toolbar Buttons

```csharp
// In toolbar, after Save button
GUI.enabled = _session.UndoStack.CanUndo;
if (GUILayout.Button("Undo", EditorStyles.toolbarButton, GUILayout.Width(50)))
{
    _session.Undo();
    InvalidatePreviews();
}
GUI.enabled = _session.UndoStack.CanRedo;
if (GUILayout.Button("Redo", EditorStyles.toolbarButton, GUILayout.Width(50)))
{
    _session.Redo();
    InvalidatePreviews();
}
GUI.enabled = true;
```

## Edge Cases

| Case | Behavior |
|------|----------|
| Undo with empty stack | No-op |
| Redo after new edit | Redo stack cleared |
| Save | Clears both stacks |
| New/Open | Clears both stacks |
| Evicted command with backup | Dispose deletes backup file |
| Drag gesture | Single command for entire drag (created on mouse up) |
| Rapid field edits | Coalesced within 500ms window |

## Testing Strategy

- [ ] Add anim → Undo → anim removed
- [ ] Add anim → Undo → Redo → anim restored
- [ ] Change range multiple times quickly → single undo step
- [ ] Replace image → Undo → old image restored
- [ ] Save → Undo disabled
- [ ] 51 operations → oldest evicted, backup cleaned

---

## Approval

- [ ] Reviewed by: [name]
- [ ] Approved on: [date]
