using IngameDebugConsole;
using NaughtyAttributes;
using UnityEngine;

namespace Game
{
	/// <summary>
	/// A general location for console commands to be added.
	/// </summary>
	public class ConsoleCommands : MonoBehaviour
	{
		[BoxGroup(RuntimeConstants.SYSTEMS)]
		[SerializeField, Required]
		private PauseAppSystem _pauseAppSystem;

		private void Awake()
		{
			// Add all console commands that make use of local fields on this class as they cannot be made static.
			DebugLogConsole.AddCommand(
				"pause-toggle",
				"Toggles the pause state of the game on or off. Only works while the game is loaded.",
				TogglePauseCommand);
		}

		/// <summary>
		/// A console command for toggling the pause state of the game.
		/// </summary>
		public void TogglePauseCommand()
		{
			_pauseAppSystem.TogglePause();

			Debug.Log($"The game is now paused: {_pauseAppSystem.IsGamePaused}.");
		}

		/// <summary>
		/// Hides the main menu and loads the game scene.
		/// </summary>
		[ConsoleMethod("game-enter", "Enters the game from the main menu.")]
		public static void EnterGameCommand()
		{
			GameControl.Instance.EnterGame();
		}

		/// <summary>
		/// Unloads the game scene and shows the main menu.
		/// </summary>
		[ConsoleMethod("game-exit", "Exits the game back to the main menu.")]
		public static void ExitGameCommand()
		{
			GameControl.Instance.ExitGame();
		}
	}
}
