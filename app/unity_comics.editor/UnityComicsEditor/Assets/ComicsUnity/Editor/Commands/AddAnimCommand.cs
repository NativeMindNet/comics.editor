using System.Collections.Generic;
using ComicsUnity.Models;

namespace ComicsUnity.Commands
{
	/// <summary>
	/// Command to add an animation to a list.
	/// </summary>
	public class AddAnimCommand : IEditCommand
	{
		private readonly IList<Anim> _list;
		private readonly Anim _anim;

		public AddAnimCommand(IList<Anim> list, Anim anim)
		{
			_list = list;
			_anim = anim;
		}

		public string Description => $"Add {_anim.Type}";

		public void Execute() => _list.Add(_anim);
		public void Undo() => _list.Remove(_anim);

		public bool CanMergeWith(IEditCommand other) => false;
		public IEditCommand MergeWith(IEditCommand other) => this;
	}
}
