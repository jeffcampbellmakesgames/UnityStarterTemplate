using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
	/// <summary>
	/// Represents the player's local settings.
	/// </summary>
	public sealed class UserSettings
	{
		/// <summary>
		/// The normalized master volume level for the game (0-1).
		/// </summary>
		[JsonIgnore]
		public float MasterVolume
		{
			get => _masterVolume;
			set
			{
				_masterVolume = Mathf.Clamp01(value);

				_onUpdated?.Invoke();
			}
		}

		/// <summary>
		/// The normalized sfx volume level for the game (0-1).
		/// </summary>
		[JsonIgnore]
		public float SFXVolume
		{
			get => _sfxVolume;
			set
			{
				_sfxVolume = Mathf.Clamp01(value);

				_onUpdated?.Invoke();
			}
		}

		/// <summary>
		/// The normalized music volume level for the game (0-1)
		/// </summary>
		[JsonIgnore]
		public float MusicVolume
		{
			get => _musicVolume;
			set
			{
				_musicVolume = Mathf.Clamp01(value);

				_onUpdated?.Invoke();
			}
		}

		/// <summary>
		/// Returns true if the FPS performance UI should be shown when active or false if should be hidden.
		/// </summary>
		[JsonIgnore]
		public bool ShowFPSPerfUI
		{
			get => _showFPSPerfUI;
			set => _showFPSPerfUI = value;
		}

		/// <summary>
		/// Returns true if the Audio performance UI should be shown when active or false if should be hidden.
		/// </summary>
		[JsonIgnore]
		public bool ShowAudioPerfUI
		{
			get => _showAudioPerfUI;
			set => _showAudioPerfUI = value;
		}

		/// <summary>
		/// Returns true if the RAM performance UI should be shown when active or false if should be hidden.
		/// </summary>
		[JsonIgnore]
		public bool ShowRAMPerfUI
		{
			get => _showRAMPerfUI;
			set => _showRAMPerfUI = value;
		}

		/// <summary>
		/// Returns true if the Advanced performance UI should be shown when active or false if should be hidden.
		/// </summary>
		[JsonIgnore]
		public bool ShowAdvancedPerfUI
		{
			get => _showAdvancedPerfUI;
			set => _showAdvancedPerfUI = value;
		}

		/// <summary>
		/// Returns the targeted frame per second mode.
		/// </summary>
		[JsonIgnore]
		public FramePerSecondMode FramePerSecondMode
		{
			get => _framePerSecondMode;
			set => _framePerSecondMode = value;
		}

		[JsonProperty("MasterVolume")]
		private float _masterVolume;

		[JsonProperty("SFXVolume")]
		private float _sfxVolume;

		[JsonProperty("MusicVolume")]
		private float _musicVolume;

		[JsonProperty("ShowFPSPerfUI")]
		private bool _showFPSPerfUI;

		[JsonProperty("ShowAudioPerfUI")]
		private bool _showAudioPerfUI;

		[JsonProperty("ShowRAMPerfUI")]
		private bool _showRAMPerfUI;

		[JsonProperty("ShowAdvancedPerfUI")]
		private bool _showAdvancedPerfUI;

		[JsonProperty("FramePerSecondMode")]
		private FramePerSecondMode _framePerSecondMode;

		private Action _onUpdated;

		public UserSettings()
		{
			// Set all the default values for these settings.
			_masterVolume = 1;
			_sfxVolume = 1;
			_musicVolume = 1;

			// Graphy Settings
			_showFPSPerfUI = true;
			_showAudioPerfUI = true;
			_showRAMPerfUI = true;
			_showAdvancedPerfUI = true;
		}

		/// <summary>
		/// Sets the callback to be invoked when this data is updated.
		/// </summary>
		public void SetUpdatedCallback(Action onUpdated)
		{
			_onUpdated = onUpdated;
		}
	}
}
