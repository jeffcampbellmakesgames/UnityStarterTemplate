using System;
using System.Collections;
using IngameDebugConsole;
using JCMG.Utility;
using NaughtyAttributes;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Game
{
	/// <summary>
	/// The app-wide control singleton managing game-specific setup and teardown.
	/// </summary>
	public sealed class GameControl : Singleton<GameControl>
	{
		/// <summary>
		/// Invoked when the game has begun to be loaded.
		/// </summary>
		public event Action GameLoadingStarted;

		/// <summary>
		/// Invoked when the game has been successfully entered into.
		/// </summary>
		public event Action GameEntered;

		/// <summary>
		/// Invoked when the game has been successfully exited from.
		/// </summary>
		public event Action GameExited;

		/// <summary>
		/// Returns true if in-game, otherwise false.
		/// </summary>
		public bool IsInGame => _isInGameBoolVariable.Value;

		[BoxGroup(RuntimeConstants.SYSTEMS)]
		[SerializeField, Required]
		private SavesAppSystem _savesAppSystem;

		[BoxGroup(RuntimeConstants.DATA)]
		[SerializeField, Required]
		private ProgressionStore _progressionStore;

		[BoxGroup(RuntimeConstants.DATA)]
		[SerializeField, Required]
		private BoolVariable _isInGameBoolVariable;

		[BoxGroup(RuntimeConstants.EVENTS)]
		[SerializeField, Required]
		private GameEvent _gameEnteredEvent;

		[BoxGroup(RuntimeConstants.EVENTS)]
		[SerializeField, Required]
		private GameEvent _gameExitedEvent;

		[BoxGroup(RuntimeConstants.SETTINGS)]
		[SerializeField, Scene]
		private string _lobbySceneName;

		private Coroutine _loadCoroutine;
		private LevelData _currentLevelData;

		protected override void Awake()
		{
			base.Awake();

			_isInGameBoolVariable.Value = false;
		}

		/// <summary>
		/// Unloads the game scene and ends the game state.
		/// </summary>
		[ConsoleMethod("game-exit", "Exits the game back to the main menu.")]
		public void ExitGame()
		{
			Assert.IsTrue(_isInGameBoolVariable.Value);
			Assert.IsNull(_loadCoroutine);

			_loadCoroutine = StartCoroutine(ExitGameOverTime());
		}

		/// <summary>
		/// Creates a new save file and loads the game for the player at the start.
		/// </summary>
		public void CreateNewGame(string profileName)
		{
			var saveData = _savesAppSystem.CreateSaveData(profileName);

			EnterGameForSaveData(saveData);
		}

		/// <summary>
		/// Retrieves the last updated save file and loads it for the player.
		/// </summary>
		public void EnterLastUpdatedGame()
		{
			var lastUpdatedSaveData = _savesAppSystem.GetLastUpdatedSaveData();

			EnterGameForSaveData(lastUpdatedSaveData);
		}

		/// <summary>
		/// Enters the game for a specific set of save data.
		/// </summary>
		public void EnterGameForSaveData(SaveData saveData)
		{
			_savesAppSystem.SetCurrentSaveData(saveData);

			// If there isn't a last level completed, load the first one.
			LevelData levelData = null;
			if (string.IsNullOrEmpty(saveData.lastLevelCompleted))
			{
				levelData = _progressionStore.GetFirstLevel();
			}
			else if (!_progressionStore.TryGetLevel(saveData.lastLevelCompleted, out levelData))
			{
				levelData = _progressionStore.GetFirstLevel();
			}

			LoadLevelData(levelData);
		}

		/// <summary>
		/// Skips the player directly to <paramref name="levelData"/>
		/// </summary>
		public void GoToLevelData(LevelData levelData)
		{
			Assert.IsTrue(_isInGameBoolVariable);
			Assert.IsTrue(_savesAppSystem.HasCurrentSaveDataSet);

			LoadLevelData(levelData);
		}

		/// <summary>
		/// Loads the level for <paramref name="levelData"/> over time and enters the game for the player.
		/// </summary>
		private void LoadLevelData(LevelData levelData)
		{
			_loadCoroutine = StartCoroutine(EnterGameOverTime(levelData));
		}

		/// <summary>
		/// Loads and sets up the game scene over time.
		/// </summary>
		private IEnumerator EnterGameOverTime(LevelData levelData)
		{
			// Broadcast that the game loading has started.
			GameLoadingStarted?.Invoke();

			// Load the game scene
			var asyncOp = SceneManager.LoadSceneAsync(levelData.SceneName);
			yield return new WaitUntil(() => asyncOp.isDone);

			// Setup in-game state and signal that the game is loaded.
			_isInGameBoolVariable.Value = true;
			_currentLevelData = levelData;

			// Invoke any relevant events
			_gameEnteredEvent.Raise();
			GameEntered?.Invoke();

			_loadCoroutine = null;
		}

		/// <summary>
		/// Exits the game scene over time.
		/// </summary>
		private IEnumerator ExitGameOverTime()
		{
			// Load the looby scene
			var asyncOp = SceneManager.LoadSceneAsync(_lobbySceneName);
			yield return new WaitUntil(() => asyncOp.isDone);

			// Setup in-game state and signal that the game is unloaded.
			_isInGameBoolVariable.Value = false;
			_currentLevelData = null;
			_savesAppSystem.UnsetCurrentSaveData();

			// Invoke any relevant events.
			_gameExitedEvent.Raise();
			
			GameExited?.Invoke();

			_loadCoroutine = null;
		}

		/// <summary>
		/// Completes the level for the player and initiates the transition to the next one (if any). If none is found,
		/// the player is returned to the main menu and their progress reset to the beginning.
		/// </summary>
		public void CompleteLevel()
		{
			Assert.IsTrue(_isInGameBoolVariable.Value);
			Assert.IsTrue(_savesAppSystem.HasCurrentSaveDataSet);

			var saveData = _savesAppSystem.CurrentSaveData;

			Assert.IsNotNull(_currentLevelData);

			// If there is a next level, set the current as the last completed for save data and load the next one.
			if (_progressionStore.TryGetNextLevel(_currentLevelData, out var nextLevel))
			{
				saveData.lastLevelCompleted = _currentLevelData.Symbol;

				LoadLevelData(nextLevel);
			}
			// Otherwise reset the players progress and exit back to the main menu.
			else
			{
				saveData.lastLevelCompleted = string.Empty;

				ExitGame();
			}
		}
	}
}
