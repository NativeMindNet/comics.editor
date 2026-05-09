using System;
using ComicsUnity.Models;

namespace ComicsUnity.Commands
{
	/// <summary>
	/// Command to update animation parameters (type-specific fields).
	/// Uses reflection-free approach with captured values.
	/// </summary>
	public class UpdateAnimParamsCommand : IEditCommand
	{
		private readonly Anim _anim;
		private readonly string _fieldName;
		private readonly object _oldValue;
		private object _newValue;
		private readonly Action<object> _setter;
		private readonly Func<object> _getter;

		public UpdateAnimParamsCommand(Anim anim, string fieldName, object newValue, Func<object> getter, Action<object> setter)
		{
			_anim = anim;
			_fieldName = fieldName;
			_oldValue = getter();
			_newValue = newValue;
			_getter = getter;
			_setter = setter;
		}

		public string Description => $"Change {_anim.Type} {_fieldName}";

		public void Execute() => _setter(_newValue);
		public void Undo() => _setter(_oldValue);

		public bool CanMergeWith(IEditCommand other) =>
			other is UpdateAnimParamsCommand u && u._anim == _anim && u._fieldName == _fieldName;

		public IEditCommand MergeWith(IEditCommand other)
		{
			var u = (UpdateAnimParamsCommand)other;
			// Keep original old value, take new new value
			return new UpdateAnimParamsCommand(_anim, _fieldName, u._newValue, _getter, _setter);
		}
	}

	/// <summary>
	/// Factory helpers for creating UpdateAnimParamsCommand.
	/// </summary>
	public static class AnimParamsCommands
	{
		public static UpdateAnimParamsCommand ForTranslateX(TranslateAnim anim, int newValue) =>
			new UpdateAnimParamsCommand(anim, "X", newValue, () => anim.X, v => anim.X = (int)v);

		public static UpdateAnimParamsCommand ForTranslateY(TranslateAnim anim, int newValue) =>
			new UpdateAnimParamsCommand(anim, "Y", newValue, () => anim.Y, v => anim.Y = (int)v);

		public static UpdateAnimParamsCommand ForRotateAngle(RotateAnim anim, float newValue) =>
			new UpdateAnimParamsCommand(anim, "Angle", newValue, () => anim.Angle, v => anim.Angle = (float)v);

		public static UpdateAnimParamsCommand ForRotatePivotX(RotateAnim anim, float newValue) =>
			new UpdateAnimParamsCommand(anim, "PivotX", newValue, () => anim.PivotX, v => anim.PivotX = (float)v);

		public static UpdateAnimParamsCommand ForRotatePivotY(RotateAnim anim, float newValue) =>
			new UpdateAnimParamsCommand(anim, "PivotY", newValue, () => anim.PivotY, v => anim.PivotY = (float)v);

		public static UpdateAnimParamsCommand ForScaleX(ScaleAnim anim, float newValue) =>
			new UpdateAnimParamsCommand(anim, "ScaleX", newValue, () => anim.ScaleX, v => anim.ScaleX = (float)v);

		public static UpdateAnimParamsCommand ForScaleY(ScaleAnim anim, float newValue) =>
			new UpdateAnimParamsCommand(anim, "ScaleY", newValue, () => anim.ScaleY, v => anim.ScaleY = (float)v);

		public static UpdateAnimParamsCommand ForScalePivotX(ScaleAnim anim, float newValue) =>
			new UpdateAnimParamsCommand(anim, "PivotX", newValue, () => anim.PivotX, v => anim.PivotX = (float)v);

		public static UpdateAnimParamsCommand ForScalePivotY(ScaleAnim anim, float newValue) =>
			new UpdateAnimParamsCommand(anim, "PivotY", newValue, () => anim.PivotY, v => anim.PivotY = (float)v);

		public static UpdateAnimParamsCommand ForAlpha(AlphaAnim anim, float newValue) =>
			new UpdateAnimParamsCommand(anim, "Alpha", newValue, () => anim.Alpha, v => anim.Alpha = (float)v);
	}
}
