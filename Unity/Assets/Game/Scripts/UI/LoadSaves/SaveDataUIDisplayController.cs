using UnityEngine.UIElements;

namespace Game
{
	/// <summary>
	/// A UI display for save data.
	/// </summary>
	public sealed class SaveDataUIDisplayController
	{
		public Label profileNameText;
		public Label lastUpdatedText;

		public void SetData(SaveData saveData)
		{
			profileNameText.text = saveData.profileName;
			lastUpdatedText.text = saveData.lastUpdated.ToString();
		}
	}
}
