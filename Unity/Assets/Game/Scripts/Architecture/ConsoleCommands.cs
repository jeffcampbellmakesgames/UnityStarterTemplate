using IngameDebugConsole;
using NaughtyAttributes;
using Tayx.Graphy;
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

		[BoxGroup(RuntimeConstants.SYSTEMS)]
		[SerializeField, Required]
		private SettingsAppSystem _settingsAppSystem;

		private void Awake()
		{
			// Add all console commands that make use of local fields on this class as they cannot be made static.
			// Pause commands
			DebugLogConsole.AddCommand(
				"pause-toggle",
				"Toggles the pause state of the game on or off. Only works while the game is loaded.",
				TogglePauseCommand);

			// Graphics commands
			DebugLogConsole.AddCommand(
				"perf-ui-fps",
				"Configures the performance metrics UI for FPS on or off. This does not hide or show the overlay.",
				ToggleGraphyUIFPSCommand);

			DebugLogConsole.AddCommand(
				"perf-ui-audio",
				"Configures the performance metrics UI for Audio on or off. This does not hide or show the overlay.",
				ToggleGraphyUIAudioCommand);

			DebugLogConsole.AddCommand(
				"perf-ui-ram",
				"Configures the performance metrics UI for RAM on or off. This does not hide or show the overlay.",
				ToggleGraphyUIMemoryCommand);

			DebugLogConsole.AddCommand(
				"perf-ui-advanced",
				"Configures the performance metrics UI for Advanced on or off. This does not hide or show the overlay.",
				ToggleGraphyUIAdvancedCommand);
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

		/// <summary>
		/// A console command for toggling the pause state of the game.
		/// </summary>
		[ConsoleMethod("perf-ui-toggle", "Toggles the performance metrics UI on or off.")]
		public static void ToggleGraphyUICommand()
		{
			GraphyManager.Instance.ToggleActive();
		}

		/// <summary>
		/// A console command for configuring the performance metrics FPS UI of the game.
		/// </summary>
		public void ToggleGraphyUIFPSCommand()
		{
			_settingsAppSystem.Settings.ShowFPSPerfUI = !_settingsAppSystem.Settings.ShowFPSPerfUI;

			var nextModuleState = _settingsAppSystem.Settings.ShowFPSPerfUI
				? GraphyManager.ModuleState.FULL
				: GraphyManager.ModuleState.OFF;

			GraphyManager.Instance.SetModuleMode(GraphyManager.ModuleType.FPS, nextModuleState);
		}

		/// <summary>
		/// A console command for configuring the performance metrics Audio UI of the game.
		/// </summary>
		public void ToggleGraphyUIAudioCommand()
		{
			_settingsAppSystem.Settings.ShowAudioPerfUI = !_settingsAppSystem.Settings.ShowAudioPerfUI;

			var nextModuleState = _settingsAppSystem.Settings.ShowAudioPerfUI
				? GraphyManager.ModuleState.FULL
				: GraphyManager.ModuleState.OFF;

			GraphyManager.Instance.SetModuleMode(GraphyManager.ModuleType.AUDIO, nextModuleState);
		}

		/// <summary>
		/// A console command for configuring the performance metrics RAM UI of the game.
		/// </summary>
		public void ToggleGraphyUIMemoryCommand()
		{
			_settingsAppSystem.Settings.ShowRAMPerfUI = !_settingsAppSystem.Settings.ShowRAMPerfUI;

			var nextModuleState = _settingsAppSystem.Settings.ShowRAMPerfUI
				? GraphyManager.ModuleState.FULL
				: GraphyManager.ModuleState.OFF;

			GraphyManager.Instance.SetModuleMode(GraphyManager.ModuleType.RAM, nextModuleState);
		}

		/// <summary>
		/// A console command for configuring the performance metrics Advanced UI of the game.
		/// </summary>
		public void ToggleGraphyUIAdvancedCommand()
		{
			_settingsAppSystem.Settings.ShowAdvancedPerfUI = !_settingsAppSystem.Settings.ShowAdvancedPerfUI;

			var nextModuleState = _settingsAppSystem.Settings.ShowAdvancedPerfUI
				? GraphyManager.ModuleState.FULL
				: GraphyManager.ModuleState.OFF;

			GraphyManager.Instance.SetModuleMode(GraphyManager.ModuleType.ADVANCED, nextModuleState);
		}
	}
}
