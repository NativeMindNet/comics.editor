using System;

namespace ComicsUnity.Commands
{
	/// <summary>
	/// Interface for undoable commands.
	/// </summary>
	public interface IEditCommand
	{
		/// <summary>
		/// Description for UI display (e.g., "Change Translate range").
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Execute the command.
		/// </summary>
		void Execute();

		/// <summary>
		/// Undo the command.
		/// </summary>
		void Undo();

		/// <summary>
		/// Returns true if this command can be merged with another for coalescing.
		/// </summary>
		bool CanMergeWith(IEditCommand other);

		/// <summary>
		/// Merge with another command (for coalescing rapid edits).
		/// Returns a new merged command.
		/// </summary>
		IEditCommand MergeWith(IEditCommand other);
	}
}
