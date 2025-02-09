using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game
{
	/// <summary>
	/// A UI document panel containing gameplay settings.
	/// </summary>
	public sealed class GameplaySettingsUIDocumentPanel : UIDocumentPanel
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

		private VisualElement _targetRootVisualElement;

		protected override VisualElement TargetRootVisualElement
		{
			get
			{
				return _targetRootVisualElement ??= _uiDocument.rootVisualElement.Q("GameplaySettingsGroup");
			}
		}

		private void Awake()
		{
			_masterVolumeSlider = TargetRootVisualElement.Q<Slider>("MasterVolumeSlider");
			_sfxVolumeSlider = TargetRootVisualElement.Q<Slider>("SFXVolumeSlider");
			_musicVolumeSlider = TargetRootVisualElement.Q<Slider>("MusicVolumeSlider");

			_masterVolumeSlider.RegisterCallback<ChangeEvent<float>>(OnMasterVolumeChanged);
			_sfxVolumeSlider.RegisterCallback<ChangeEvent<float>>(OnSFXVolumeChanged);
			_musicVolumeSlider.RegisterCallback<ChangeEvent<float>>(OnMusicVolumeChanged);
		}

		private void OnDestroy()
		{
			_masterVolumeSlider.UnregisterCallback<ChangeEvent<float>>(OnMasterVolumeChanged);
			_sfxVolumeSlider.UnregisterCallback<ChangeEvent<float>>(OnSFXVolumeChanged);
			_musicVolumeSlider.UnregisterCallback<ChangeEvent<float>>(OnMusicVolumeChanged);
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
		private void OnMasterVolumeChanged(ChangeEvent<float> evt)
		{
			_settingsAppSystem.Settings.MasterVolume = evt.newValue;
		}

		/// <summary>
		/// Invoked when the sfx volume slider has been updated by a player.
		/// </summary>
		private void OnSFXVolumeChanged(ChangeEvent<float> evt)
		{
			_settingsAppSystem.Settings.SFXVolume = evt.newValue;
		}

		/// <summary>
		/// Invoked when the music volume slider has been updated by a player.
		/// </summary>
		private void OnMusicVolumeChanged(ChangeEvent<float> evt)
		{
			_settingsAppSystem.Settings.MusicVolume = evt.newValue;
		}
	}
}
