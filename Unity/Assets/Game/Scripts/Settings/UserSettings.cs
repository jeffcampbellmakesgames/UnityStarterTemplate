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

		[JsonProperty("MasterVolume")]
		private float _masterVolume;

		[JsonProperty("SFXVolume")]
		private float _sfxVolume;

		[JsonProperty("MusicVolume")]
		private float _musicVolume;

		private Action _onUpdated;

		public UserSettings()
		{
			// Set all the default values for these settings.
			_masterVolume = 1;
			_sfxVolume = 1;
			_musicVolume = 1;
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
