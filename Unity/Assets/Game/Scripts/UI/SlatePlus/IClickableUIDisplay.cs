using System;

namespace Game
{
	/// <summary>
	/// Represents a UI display that can be clicked.
	/// </summary>
	public interface IClickableUIDisplay<TDataType>
	{
		void SetOnClick(Action<TDataType> onClickHandler);
	}
}
