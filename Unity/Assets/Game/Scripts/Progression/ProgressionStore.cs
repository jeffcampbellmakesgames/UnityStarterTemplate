using System.Collections.Generic;
using JCMG.Utility;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
	/// <summary>
	/// A reference for progression via looking up levels.
	/// </summary>
	[CreateAssetMenu(
		fileName = "ProgressionStore",
		menuName = "Game/Data/Progression Store")]
	public sealed class ProgressionStore : ScriptableObject
	{
		/// <summary>
		/// A read-only collection of all levels in the game.
		/// </summary>
		public IReadOnlyList<LevelData> Levels => _levels;

		[InfoBox("A sequential order of levels in the game denoting it's progression.")]
		[SerializeField]
		private List<LevelData> _levels;

		/// <summary>
		/// Returns the first <see cref="LevelData"/> for matching <paramref name="levelSymbol"/>. Asserts if a match
		/// is not found.
		/// </summary>
		public LevelData GetLevel(string levelSymbol)
		{
			var levelData = _levels.Find(x => x.Symbol == levelSymbol);

			Assert.IsNotNull(levelData);

			return levelData;
		}

		/// <summary>
		/// Returns true if a level is found for matching <paramref name="levelSymbol"/>, otherwise false. If true,
		/// <paramref name="levelData"/> will be initialized.
		/// </summary>
		public bool TryGetLevel(string levelSymbol, out LevelData levelData)
		{
			levelData = null;

			if (_levels.TryGet(levelSymbol, out var level))
			{
				levelData = level;
			}

			return levelData != null;
		}

		/// <summary>
		/// Returns true if a next level is found for matching <paramref name="levelSymbol"/>, otherwise false. If
		/// true, <paramref name="nextLevelData"/> will be initialized.
		/// </summary>
		public bool TryGetNextLevel(string levelSymbol, out LevelData nextLevelData)
		{
			var currentLevelData = _levels.Find(x => x.Symbol == levelSymbol);

			return TryGetNextLevel(currentLevelData, out nextLevelData);
		}

		/// <summary>
		/// Returns true if a next level is found for matching <paramref name="levelData"/>, otherwise false. If
		/// true, <paramref name="nextLevelData"/> will be initialized.
		/// </summary>
		public bool TryGetNextLevel(LevelData levelData, out LevelData nextLevelData)
		{
			nextLevelData = null;
			var currentIndex = _levels.IndexOf(levelData);
			var nextIndex = currentIndex + 1;
			if (nextIndex < _levels.Count)
			{
				nextLevelData = _levels[nextIndex];
			}

			return nextLevelData != null;
		}

		/// <summary>
		/// Returns the first level for the player to enter.
		/// </summary>
		public LevelData GetFirstLevel()
		{
			return _levels[0];
		}
	}
}
