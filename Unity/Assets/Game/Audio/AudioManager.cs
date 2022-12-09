using System;
using System.Collections;
using System.Collections.Generic;
using JCMG.Utility;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
	[SerializeField] private AudioSO _audioSettings;	//TODO add [SerializeField] private float _crossfadeTime;
	[SerializeField] private AudioMixer _masterMixer;
	[SerializeField] private AudioMixerGroup _sfxMixer, _bgmMixer;


	private Dictionary<string, AudioSource> _audiosources;
	private string _currentMusic = "";
	private bool _crossfading = false;

	protected override void Awake()
	{
		base.Awake();
		_audiosources = new();
	}

	public void Start()
	{
		SetMusic("Winter");
	}

	[Button()]
	public void SetLodge()
	{
		SetMusic("Lodge");
	}
	
	[Button()]
	public void SetWinter()
	{
		SetMusic("Winter");
	}

	public void SetMusic(string musicName, float crossfadeTime = -1)
	{
		if (musicName == _currentMusic || _crossfading) return;
		if (crossfadeTime < 0) crossfadeTime = _audioSettings.crossfadeTime;
		
		if (!_audiosources.ContainsKey(musicName))
		{
			_audiosources.Add(musicName, gameObject.AddComponent<AudioSource>());
			_audiosources[musicName].clip = _audioSettings.bgmDictionary[musicName];
		}

		AudioSource newMusic = _audiosources[musicName];
		newMusic.Play();
		newMusic.loop = true;
		newMusic.outputAudioMixerGroup = _bgmMixer;

		_crossfading = true;
		LeanTween.value(0.0f, 1.0f, crossfadeTime)
			.setOnUpdate((value) =>
			{		
				if (_currentMusic != "")
					_audiosources[_currentMusic].volume = 1 - value;
				
				_audiosources[musicName].volume = 1;
			})
			.setOnComplete(() =>
			{
				_currentMusic = musicName;
				_crossfading = false;
			});

	}
	
	
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

	/// <summary>
	/// Helper function for setting exposed volume parameters of _masterMixer
	/// </summary>
	/// <param name="volume">Should be between 0 (muted) and 1 (full volume)</param>
	/// <param name="parameterName">Name of exposed parameter, must match exactly, case sensitive</param>
	private void SetVolume(float volume, string parameterName)
	{
		volume = Mathf.Clamp(volume, 0.0001f, 1.0f);
		volume =  Mathf.Log10(volume) * 20; //-80db is muted, 0 is regular "full" volume
		//TODO set to logarithmic scaling
		_masterMixer.SetFloat(parameterName, volume); 
	}
	#endregion
	
}
