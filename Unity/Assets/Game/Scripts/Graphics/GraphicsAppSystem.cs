using NaughtyAttributes;
using Tayx.Graphy;
using UnityEngine;

namespace Game
{
	/// <summary>
	/// A <see cref="IAppSystem"/> app system for handling graphics related settings.
	/// </summary>
	[CreateAssetMenu(
		fileName = "GraphicsAppSystem",
		menuName = "Game/Systems/GraphicsAppSystem")]
	public sealed class GraphicsAppSystem : ScriptableAppSystem
	{
		[BoxGroup(RuntimeConstants.SYSTEMS)]
		[SerializeField, Required]
		private SettingsAppSystem _settingsAppSystem;

		/// <inheritdoc />
		public override void OneTimeSetup()
		{
			AppControl.Instance.SetupCompleted += OnSetupCompleted;
		}

		/// <inheritdoc />
		public override void OneTimeTeardown()
		{
			// No-Op
			// No need to call AppControl.SetupCompleted as app is quitting.
		}

		/// <summary>
		/// Invoked when the app setup has completed.
		/// </summary>
		private void OnSetupCompleted()
		{
			var userSettings = _settingsAppSystem.Settings;
			var graphyManager = GraphyManager.Instance;

			// Turn on the perf UI
			graphyManager.ToggleActive();

			// FPS
			graphyManager.SetModuleMode(
				GraphyManager.ModuleType.FPS,
				userSettings.ShowFPSPerfUI
					? GraphyManager.ModuleState.FULL
					: GraphyManager.ModuleState.OFF);

			// FPS
			graphyManager.SetModuleMode(
				GraphyManager.ModuleType.AUDIO,
				userSettings.ShowAudioPerfUI
					? GraphyManager.ModuleState.FULL
					: GraphyManager.ModuleState.OFF);

			// FPS
			graphyManager.SetModuleMode(
				GraphyManager.ModuleType.RAM,
				userSettings.ShowRAMPerfUI
					? GraphyManager.ModuleState.FULL
					: GraphyManager.ModuleState.OFF);

			// FPS
			graphyManager.SetModuleMode(
				GraphyManager.ModuleType.ADVANCED,
				userSettings.ShowAdvancedPerfUI
					? GraphyManager.ModuleState.FULL
					: GraphyManager.ModuleState.OFF);

			// Turn off the perf UI
			graphyManager.ToggleActive();
		}
	}
}
