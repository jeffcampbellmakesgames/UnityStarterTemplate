using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
	/// <summary>
	/// An <see cref="IAppSystem"/> responsible for creating, saving, and loading game settings.
	/// </summary>
	[CreateAssetMenu(
		fileName = "SettingsAppSystem",
		menuName = "Game/Systems/SettingsAppSystem")]
	public sealed class SettingsAppSystem : ScriptableAppSystem
	{
		/// <summary>
		/// Invoked when the player has updated their settings.
		/// </summary>
		public event Action Updated;

		/// <summary>
		/// The player's settings data.
		/// </summary>
		public UserSettings Settings => _settings;

		private UserSettings _settings;
		private string _settingsFilePath;

		private const string USER_SETTINGS_FILENAME = "user_settings.json";

		/// <inheritdoc />
		public override void OneTimeSetup()
		{
			// Format the settings file path
			_settingsFilePath = Path.Combine(Application.persistentDataPath, USER_SETTINGS_FILENAME);

			// Check for an existing file path; if it doesn't exist make one from scratch.
			if (File.Exists(_settingsFilePath))
			{
				var settingsJson = File.ReadAllText(_settingsFilePath);
				_settings = JsonConvert.DeserializeObject<UserSettings>(settingsJson);
			}
			else
			{
				_settings = new UserSettings();
				_settings.FramePerSecondMode = GraphicsTools.GetDefaultFramePerSecondMode();
			}

			_settings.SetUpdatedCallback(Updated);
		}

		/// <inheritdoc />
		public override void OneTimeTeardown()
		{
			FlushSettingsToDisk();
		}

		/// <summary>
		/// Forces the in-memory user settings to be flushed to disk.
		/// </summary>
		public void FlushSettingsToDisk()
		{
			Assert.IsNotNull(_settings);

			// Flush the user settings to disk.
			var settingsJson = JsonConvert.SerializeObject(_settings, Formatting.Indented);
			File.WriteAllText(_settingsFilePath, settingsJson);
		}
	}
}
