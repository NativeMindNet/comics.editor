using ComicsUnity.Models;

namespace ComicsUnity.Commands
{
	/// <summary>
	/// Command to update an animation's Start/End range.
	/// Supports coalescing for rapid edits.
	/// </summary>
	public class UpdateAnimRangeCommand : IEditCommand
	{
		private readonly Anim _anim;
		private readonly int _oldStart, _oldEnd;
		private int _newStart, _newEnd;

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
			// Keep original old values, take new new values
			return new UpdateAnimRangeCommand(_anim, u._newStart, u._newEnd)
			{
				_newStart = u._newStart,
				_newEnd = u._newEnd
			};
		}
	}
}
