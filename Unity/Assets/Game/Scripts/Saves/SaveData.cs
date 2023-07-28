using System;
using Newtonsoft.Json;

namespace Game
{
	[JsonObject, Serializable]
	public sealed class SaveData
	{
		/// <summary>
		/// A unique guid identifier differentiating this save file from others.
		/// </summary>
		public string id;

		/// <summary>
		/// The datetime this save file was created.
		/// </summary>
		public DateTime created;

		/// <summary>
		/// The last datetime this save file was updated.
		/// </summary>
		public DateTime lastUpdated;

		/// <summary>
		/// The last level completed by the player, if any.
		/// </summary>
		public string lastLevelCompleted;

		/// <summary>
		/// The name of the save profile input by the player.
		/// </summary>
		public string profileName;

		/// <summary>
		/// Returns the formatted file name for this save file.
		/// </summary>
		public string GetSaveFileName() => $"{id}.json";

		public SaveData()
		{

		}
	}
}
