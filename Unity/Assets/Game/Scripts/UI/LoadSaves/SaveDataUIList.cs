
namespace Game
{
	/// <summary>
	/// A UI list for displaying save data.
	/// </summary>
	public class SaveDataUIList : UIListBase<SaveData, SaveDataUIDisplay>
	{
		/// <inheritdoc />
		public override void Clear()
		{
			for (var i = 0; i < _displayBases.Count; i++)
			{
				_displayBases[i].Clear();

				ListItemPool.Recycle(_displayBases[i]);
			}

			_displayBases.Clear();
		}
	}
}
