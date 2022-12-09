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
		/// Invoked when the game has been successfully entered into.
		/// </summary>
		public event Action GameEntered;

		/// <summary>
		/// Invoked when the game has been successfully exited from.
		/// </summary>
		public event Action GameExited;

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

		[BoxGroup(RuntimeConstants.SETTINGS)]
		[SerializeField, Scene]
		private string _gameSceneName;

		private Coroutine _loadCoroutine;

		protected override void Awake()
		{
			base.Awake();

			_isInGameBoolVariable.Value = false;
		}

		/// <summary>
		/// Hides the main menu and loads the game scene.
		/// </summary>
		[ConsoleMethod("game-enter", "Enters the game from the main menu.")]
		public void EnterGame()
		{
			Assert.IsFalse(_isInGameBoolVariable.Value);
			Assert.IsNull(_loadCoroutine);

			_loadCoroutine = StartCoroutine(EnterGameOverTime());
		}

		/// <summary>
		/// Unloads the game scene and shows the main menu.
		/// </summary>
		[ConsoleMethod("game-exit", "Exits the game back to the main menu.")]
		public void ExitGame()
		{
			Assert.IsTrue(_isInGameBoolVariable.Value);
			Assert.IsNull(_loadCoroutine);

			_loadCoroutine = StartCoroutine(ExitGameOverTime());
		}

		/// <summary>
		/// Loads and sets up the game scene over time.
		/// </summary>
		private IEnumerator EnterGameOverTime()
		{
			// Load the game scene
			var asyncOp = SceneManager.LoadSceneAsync(_gameSceneName);
			yield return new WaitUntil(() => asyncOp.isDone);

			// Setup in-game state and signal that the game is loaded.
			_isInGameBoolVariable.Value = true;
			_gameEnteredEvent.Raise();
			GameExited?.Invoke();

			_loadCoroutine = null;
		}

		/// <summary>
		/// Exits the game scene over time.
		/// </summary>
		private IEnumerator ExitGameOverTime()
		{
			// Load the game scene
			var asyncOp = SceneManager.LoadSceneAsync(_lobbySceneName);
			yield return new WaitUntil(() => asyncOp.isDone);

			// Setup in-game state and signal that the game is unloaded.
			_isInGameBoolVariable.Value = false;
			_gameExitedEvent.Raise();
			GameEntered?.Invoke();

			_loadCoroutine = null;
		}
	}
}
