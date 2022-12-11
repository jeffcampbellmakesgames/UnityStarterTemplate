using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JCMG.Utility;
using NaughtyAttributes;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
	[SerializeField, Required] 
	private AudioSettings _audioSettings;
	
	[SerializeField, Required] 
	private AudioMixer _masterMixer;
	
	[SerializeField, Required] 
	private AudioMixerGroup _sfxMixer, _bgmMixer;

	private Dictionary<string, AudioSource> _audiosources;
	private SerializableDictionaryBase<string, AudioClip> _sfxDictionary;
	private SerializableDictionaryBase<string, AudioClip> _bgmDictionary;
	private string _currentMusic = "";
	private bool _crossfading = false;
	private static string NAME_CONSTRUCTION_PREFIX = "AUTOADDED_";

	protected override void Awake()
	{
		base.Awake();
		_audiosources = new();
		_sfxDictionary = _audioSettings.sfxDictionary;
		_bgmDictionary = _audioSettings.bgmDictionary;
	}
	
	#region Playing music and sound effects

	//I know these overloaded PlaySound functions are a little gross,
	// I figure we can think of a better way when we refactor from strings to enums (or something else)
	/// <summary>
	/// 2D PlaySound. Creates an audiosource if it doesn't already exist, plays it. Adds the clip to your audiosettings
	/// if it's not already there.
	/// if the audiosource is not already playing.
	/// </summary>
	/// <param name="clip">AudioClip of the sound to play</param>
	/// <param name="stereoBlend">Optional argument, -1 is left, +1 is right. 0 by default</param>
	public void PlaySound(AudioClip clip, float stereoBlend = 0)
	{
		string constructedName = NAME_CONSTRUCTION_PREFIX + clip.name;
		if (!_sfxDictionary.ContainsKey(constructedName))
		{
			_sfxDictionary.Add(constructedName, clip);
		}
		PlaySound(constructedName, stereoBlend);
	}
	
	//I know these overloaded PlaySound functions are a little gross,
	// I figure we can think of a better way when we refactor from strings to enums (or something else)
	/// <summary>
	/// 2D PlaySound. Creates an audiosource if it doesn't already exist, plays it. Adds the clip to your audiosettings
	/// if it's not already there.
	/// if the audiosource is not already playing.
	/// </summary>
	/// <param name="clip">AudioClip of the sound to play</param>
	/// <param name="position">World position of the sound to be played.</param>
	public void PlaySound(AudioClip clip, Vector3 position)
	{
		string constructedName = NAME_CONSTRUCTION_PREFIX + clip.name;
		if (!_sfxDictionary.ContainsKey(constructedName))
		{
			_sfxDictionary.Add(constructedName, clip);
		}
		PlaySound(constructedName, position);
	}
	
	
	/// <summary>
	/// 2D PlaySound. Creates an audiosource if it doesn't already exist, plays it
	/// if the audiosource is not already playing.
	/// </summary>
	/// <param name="soundName">Name of the sound to play, case sensitive.</param>
	/// <param name="stereoBlend">Optional argument, -1 is left, +1 is right. 0 by default</param>
	public void PlaySound(string soundName, float stereoBlend = 0)
	{
		AudioSource sound = PlaySoundHelper(soundName);
		sound.spatialBlend = 0; //2d
		sound.panStereo = stereoBlend;
	}

	/// <summary>
	/// 3D PlaySound overload. Creates an audiosource if it doesn't already exist, plays it
	/// if the audiosource is not already playing. 
	/// </summary>
	/// <param name="soundName">Name of the sound to play, case sensitive.</param>
	/// <param name="position">World position of the sound to be played.</param>
	public void PlaySound(string soundName, Vector3 position)
	{
		AudioSource sound = PlaySoundHelper(soundName);
		sound.spatialBlend = 1; //3d
		sound.transform.position = position;
	}
	
	/// <summary>
	/// Plays the music specified on loop. Crossfades between last played music and the new one.
	/// Creates an audiosource if it doesn't already exist. Adds the clip to your audioSettings as a side effect,
	/// if it's not already there.
	/// </summary>
	/// <param name="clip">Audioclip of the music to be played</param>
	/// <param name="crossfadeTime">Time for crossfade to occur. Negative value denotes that it will use the
	/// default crossfade time specified in the audiosettings SO.</param>
	public void SetMusic(AudioClip clip, float crossfadeTime = -1)
	{
		string constructedName = NAME_CONSTRUCTION_PREFIX + clip.name;
		if (!_bgmDictionary.ContainsKey(constructedName))
		{
			_bgmDictionary.Add(constructedName, clip);
		}
		SetMusic(constructedName, crossfadeTime);
	}
	
	/// <summary>
	/// Plays the music specified on loop. Crossfades between last played music and the new one.
	/// Creates an audiosource if it doesn't already exist.
	/// </summary>
	/// <param name="musicName">Name of the music to be played, case sensitive.</param>
	/// <param name="crossfadeTime">Time for crossfade to occur. Negative value denotes that it will use the
	/// default crossfade time specified in the audiosettings SO.</param>
	public void SetMusic(string musicName, float crossfadeTime = -1)
	{
		if (musicName == _currentMusic || _crossfading) return;
		if (crossfadeTime < 0) crossfadeTime = _audioSettings.crossfadeTime;
		
		if (!_audiosources.ContainsKey(musicName))
		{
			_audiosources.Add(musicName, gameObject.AddComponent<AudioSource>());
			_audiosources[musicName].clip = _bgmDictionary[musicName];
		}

		AudioSource newMusic = _audiosources[musicName];
		newMusic.Play();
		newMusic.loop = true;
		newMusic.outputAudioMixerGroup = _bgmMixer;

		_crossfading = true;
		if (_currentMusic != "")
		{
			_audiosources[_currentMusic].DOFade(0, crossfadeTime);
		}
		_audiosources[musicName].volume = 0;
		_audiosources[musicName].DOFade(1, crossfadeTime)
			.OnComplete(() =>
			{
				_currentMusic = musicName;
				_crossfading = false;
			});
	}

	#endregion

	#region Set Volume Functions

	/// <summary>
	/// Sets the master volume, which attenuates both Sound Effects (SFX) and background music (BGM).
	/// </summary>
	/// <param name="volume">Should be between 0 (muted) and 1 (full volume)</param>
	public void SetVolumeMaster(float volume)
	{
		SetVolume(volume, "VolumeMaster");
	}
	
	/// <summary>
	/// Sets the volume for Sound Effects (SFX).
	/// </summary>
	/// <param name="volume">Should be between 0 (muted) and 1 (full volume)</param>
	public void SetVolumeSFX(float volume)
	{
		SetVolume(volume, "VolumeSFX");
	}

	/// <summary>
	/// Sets the volume for background music (BGM).
	/// </summary>
	/// <param name="volume">Should be between 0 (muted) and 1 (full volume)</param>
	public void SetVolumeBGM(float volume)
	{
		SetVolume(volume, "VolumeBGM");
	}

	#endregion
	
	

	/// <summary>
	/// Helper function for the play sound functions. Creates an audiosource if it doesn't already exist, plays it
	/// if the audiosource is not already playing. This function adds the audiosource to the list, but returns a reference
	/// as, well for convenience.
	/// </summary>
	/// <param name="soundName">Name of the sound to play, case sensitive.</param>
	/// <returns>reference to the audiosource created (or reused)</returns>
	private AudioSource PlaySoundHelper(string soundName)
	{
		if (!_audiosources.ContainsKey(soundName))
		{
			GameObject newGO = new GameObject();
			newGO.transform.parent = transform;
			newGO.name = soundName;
			_audiosources.Add(soundName, newGO.AddComponent<AudioSource>());
			_audiosources[soundName].clip = _sfxDictionary[soundName];
		}

		AudioSource sound = _audiosources[soundName];
		sound.outputAudioMixerGroup = _sfxMixer;
		if (!sound.isPlaying) sound.Play();
		return sound;
	}
	
	/// <summary>
	/// Helper function for setting exposed volume parameters of _masterMixer
	/// </summary>
	/// <param name="volume">Should be between 0 (muted) and 1 (full volume)</param>
	/// <param name="parameterName">Name of exposed parameter, must match exactly, case sensitive</param>
	private void SetVolume(float volume, string parameterName)
	{
		volume = Mathf.Clamp(volume, 0.0001f, 1.0f);
		volume =  Mathf.Log10(volume) * 20; //-80db is muted, 0 is regular "full" volume
		_masterMixer.SetFloat(parameterName, volume); 
	}
	
}
