using System;
using System.Collections.Generic;
using UnityEngine;

namespace ComicsUnity.Commands
{
	/// <summary>
	/// Manages undo/redo history with coalescing support.
	/// </summary>
	public class UndoStack
	{
		private readonly List<IEditCommand> _undoStack = new List<IEditCommand>();
		private readonly List<IEditCommand> _redoStack = new List<IEditCommand>();
		private readonly int _maxDepth;
		private double _lastCommandTime;
		private const double CoalesceWindowMs = 500;

		public UndoStack(int maxDepth = 50)
		{
			_maxDepth = maxDepth;
		}

		public bool CanUndo => _undoStack.Count > 0;
		public bool CanRedo => _redoStack.Count > 0;

		public string UndoDescription => _undoStack.Count > 0 ? _undoStack[_undoStack.Count - 1].Description : null;
		public string RedoDescription => _redoStack.Count > 0 ? _redoStack[_redoStack.Count - 1].Description : null;

		/// <summary>
		/// Execute a command and add it to the undo stack.
		/// </summary>
		public void Execute(IEditCommand command)
		{
			command.Execute();

			var now = Time.realtimeSinceStartupAsDouble;

			// Try to coalesce with previous command
			if (_undoStack.Count > 0 && now - _lastCommandTime < CoalesceWindowMs / 1000.0)
			{
				var prev = _undoStack[_undoStack.Count - 1];
				if (prev.CanMergeWith(command))
				{
					_undoStack[_undoStack.Count - 1] = prev.MergeWith(command);
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
				(evicted as IDisposable)?.Dispose();
			}
		}

		/// <summary>
		/// Undo the last command.
		/// </summary>
		public void Undo()
		{
			if (_undoStack.Count == 0) return;
			var cmd = _undoStack[_undoStack.Count - 1];
			_undoStack.RemoveAt(_undoStack.Count - 1);
			cmd.Undo();
			_redoStack.Add(cmd);
		}

		/// <summary>
		/// Redo the last undone command.
		/// </summary>
		public void Redo()
		{
			if (_redoStack.Count == 0) return;
			var cmd = _redoStack[_redoStack.Count - 1];
			_redoStack.RemoveAt(_redoStack.Count - 1);
			cmd.Execute();
			_undoStack.Add(cmd);
		}

		/// <summary>
		/// Clear all history (called on Save/New/Open).
		/// </summary>
		public void Clear()
		{
			foreach (var cmd in _undoStack)
				(cmd as IDisposable)?.Dispose();
			foreach (var cmd in _redoStack)
				(cmd as IDisposable)?.Dispose();

			_undoStack.Clear();
			_redoStack.Clear();
		}
	}
}
