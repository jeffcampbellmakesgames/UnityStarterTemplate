using IngameDebugConsole;
using UnityEngine;

namespace Game
{
	/// <summary>
	/// A general location for console commands to be added.
	/// </summary>
	public class ConsoleCommands : MonoBehaviour
	{
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
