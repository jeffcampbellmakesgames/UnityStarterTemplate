using System;
using System.Linq;
using JCMG.Utility;
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
			ConfigureGraphy();
			ConfigureGraphics();
		}

		/// <summary>
		/// Configures graphy based on any user settings.
		/// </summary>
		private void ConfigureGraphy()
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

		/// <summary>
		/// Configures graphics settings for this device based on the user settings.
		/// </summary>
		private void ConfigureGraphics()
		{
			var settings = _settingsAppSystem.Settings;

			// Set target framerate
			var currentMode = settings.FramePerSecondMode;

			if (!GraphicsTools.AvailableFramePerSecondModes.Contains(currentMode))
			{
				currentMode = GraphicsTools.GetDefaultFramePerSecondMode();
				settings.FramePerSecondMode = currentMode;
			}

			SetFrameMode(currentMode);
		}

		/// <summary>
		/// Sets the appropriate graphics settings to achieve the desired <paramref name="mode"/>.
		/// </summary>
		public void SetFrameMode(FramePerSecondMode mode)
		{
			var targetFrameRate = mode switch
			{
				FramePerSecondMode.Thirty => 30,
				FramePerSecondMode.Sixty => 60,
				FramePerSecondMode.Uncapped => -1,
				_ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
			};

			Application.targetFrameRate = targetFrameRate;
		}
	}
}
