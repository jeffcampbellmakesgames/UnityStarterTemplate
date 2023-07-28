using System;
using System.Collections;
using System.Collections.Generic;
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
	public sealed partial class GameControl : Singleton<GameControl>, IAppSystem
	{
		[Serializable]
		private class GameAppSystemRef
		{
			/// <summary>
			/// Returns true if this app system ref is using a Monobehavior source or false if it's a ScriptableObject
			/// source.
			/// </summary>
			public bool IsUsingMonobehaviorSource => _sourceType == SourceType.Monobehavior;

			private enum SourceType
			{
				Monobehavior = 0,
				ScriptableObject = 1
			}

			[SerializeField]
			private SourceType _sourceType;

			[AllowNesting, ShowIf("IsUsingMonobehaviorSource")]
			[SerializeField]
			private MonoBehaviour _appSystemBehavior;

			[AllowNesting, HideIf("IsUsingMonobehaviorSource")]
			[SerializeField]
			private ScriptableGameSystem _appSystemScriptableObject;

			public IGameSystem GetGameSystem()
			{
				IGameSystem result = null;
				if (IsUsingMonobehaviorSource)
				{
					Assert.IsNotNull(_appSystemBehavior);
					Assert.IsTrue(
						_appSystemBehavior is IGameSystem,
						$"{_appSystemBehavior.name} does not implement IAppSystem.");

					result = _appSystemBehavior as IGameSystem;
				}
				else
				{
					Assert.IsNotNull(_appSystemScriptableObject);
					result = _appSystemScriptableObject;
				}

				return result;
			}
		}

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

		[Label("Game Systems")]
		[SerializeField]
		private List<GameAppSystemRef> _gameAppSystemRefs;

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

		#region IAppSystem

		/// <inheritdoc />
		public void OneTimeSetup()
		{
			for (var i = 0; i < _gameAppSystemRefs.Count; i++)
			{
				var gameSystem = _gameAppSystemRefs[i].GetGameSystem();
				gameSystem.OneTimeSetup();
			}
		}

		/// <inheritdoc />
		public void OneTimeTeardown()
		{
			for (var i = 0; i < _gameAppSystemRefs.Count; i++)
			{
				var gameSystem = _gameAppSystemRefs[i].GetGameSystem();
				gameSystem.OneTimeTeardown();
			}
		}

		/// <inheritdoc />
		public bool IsSetupComplete()
		{
			var result = true;
			for (var i = 0; i < _gameAppSystemRefs.Count; i++)
			{
				var gameSystem = _gameAppSystemRefs[i].GetGameSystem();
				result &= gameSystem.IsSetupComplete();
			}
			return result;
		}

		#endregion

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

			for (var i = 0; i < _gameAppSystemRefs.Count; i++)
			{
				var gameSystem = _gameAppSystemRefs[i].GetGameSystem();
				gameSystem.Setup();
			}

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
			// Load the lobby scene
			var asyncOp = SceneManager.LoadSceneAsync(_lobbySceneName);
			yield return new WaitUntil(() => asyncOp.isDone);

			for (var i = 0; i < _gameAppSystemRefs.Count; i++)
			{
				var gameSystem = _gameAppSystemRefs[i].GetGameSystem();
				gameSystem.Teardown();
			}

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

		/// <summary>
		/// Returns true if a game system can be found of type <typeparamref name="T"/>, otherwise false. If true,
		/// <paramref name="gameSystem"/> will be initialized.
		/// </summary>
		private bool TryGetGameSystem<T>(out T gameSystem)
			where T : class, IGameSystem
		{
			gameSystem = null;
			for (var i = 0; i < _gameAppSystemRefs.Count; i++)
			{
				var localGameSystem = _gameAppSystemRefs[i].GetGameSystem();
				if (localGameSystem is T typedGameSystem)
				{
					gameSystem = typedGameSystem;
					break;
				}
			}

			return gameSystem != null;
		}

		/// <summary>
		/// Returns a local game system of type <typeparamref name="T"/>. If one cannot be found, an exception is thrown.
		/// </summary>
		private T GetGameSystem<T>() where T : class, IGameSystem
		{
			T result = null;
			if (!TryGetGameSystem(out result))
			{
				throw new Exception($"Cannot find GameSystem of type: [{typeof(T)}] in GameControl.");
			}

			return result;
		}
	}
}
