using System.Collections.Generic;
using ComicsUnity.Models;

namespace ComicsUnity.Commands
{
	/// <summary>
	/// Command to remove an animation from a list.
	/// </summary>
	public class RemoveAnimCommand : IEditCommand
	{
		private readonly IList<Anim> _list;
		private readonly Anim _anim;
		private int _index;

		public RemoveAnimCommand(IList<Anim> list, Anim anim)
		{
			_list = list;
			_anim = anim;
		}

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
}
