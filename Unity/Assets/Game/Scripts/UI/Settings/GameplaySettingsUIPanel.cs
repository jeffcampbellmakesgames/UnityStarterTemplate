using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	/// <summary>
	/// A UI panel containing gameplay settings.
	/// </summary>
	public sealed class GameplaySettingsUIPanel : UIPanel
	{
		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private Slider _masterVolumeSlider;

		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private Slider _sfxVolumeSlider;

		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private Slider _musicVolumeSlider;

		[BoxGroup(RuntimeConstants.SYSTEMS)]
		[SerializeField, Required]
		private SettingsAppSystem _settingsAppSystem;

		private void Awake()
		{
			_masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
			_sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
			_musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
		}

		private void OnDestroy()
		{
			_masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
			_sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
			_musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
		}

		/// <inheritdoc />
		public override void Show()
		{
			_masterVolumeSlider.SetValueWithoutNotify(_settingsAppSystem.Settings.MasterVolume);
			_sfxVolumeSlider.SetValueWithoutNotify(_settingsAppSystem.Settings.SFXVolume);
			_musicVolumeSlider.SetValueWithoutNotify(_settingsAppSystem.Settings.MusicVolume);

			base.Show();
		}

		/// <summary>
		/// Invoked when the master volume slider has been updated by a player.
		/// </summary>
		private void OnMasterVolumeChanged(float newValue)
		{
			_settingsAppSystem.Settings.MasterVolume = newValue;
		}

		/// <summary>
		/// Invoked when the sfx volume slider has been updated by a player.
		/// </summary>
		private void OnSFXVolumeChanged(float newValue)
		{
			_settingsAppSystem.Settings.SFXVolume = newValue;
		}

		/// <summary>
		/// Invoked when the music volume slider has been updated by a player.
		/// </summary>
		private void OnMusicVolumeChanged(float newValue)
		{
			_settingsAppSystem.Settings.MusicVolume = newValue;
		}
	}
}
