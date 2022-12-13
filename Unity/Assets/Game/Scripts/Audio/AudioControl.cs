using System.Collections.Generic;
using DG.Tweening;
using JCMG.Utility;
using NaughtyAttributes;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.Audio;

namespace Game
{
    /// <summary>
    /// A control singleton for handling music and sfx.
    /// </summary>
    public sealed class AudioControl : Singleton<AudioControl>
    {
        [BoxGroup(RuntimeConstants.SYSTEMS)]
        [SerializeField, Required]
        private SettingsAppSystem _settingsAppSystem;

        [BoxGroup(RuntimeConstants.PREFAB)]
        [SerializeField, Required]
        private AudioSource _audioSourcePrefab;

        [BoxGroup(RuntimeConstants.PROJECT)]
        [SerializeField, Required]
        private AudioMixer _masterMixer;

        [BoxGroup(RuntimeConstants.PROJECT)]
        [SerializeField, Required]
        private AudioMixerGroup _sfxMixer;

        [BoxGroup(RuntimeConstants.PROJECT)]
        [SerializeField, Required]
        private AudioMixerGroup _bgmMixer;

        [BoxGroup(RuntimeConstants.PROJECT)]
        [SerializeField, Required]
        private GameEvent _appSetupCompleted;

        [BoxGroup(RuntimeConstants.SETTINGS)]
        [MinValue(0)]
        [SerializeField]
        private int _initialPoolSize;

        [BoxGroup(RuntimeConstants.SETTINGS)]
        [MinValue(0)]
        [SerializeField]
        private float _defaultCrossFadeTime;

        private HashSet<AudioClip> _2DSoundEffectsPlayedThisFrame;
        private List<AudioSource> _activeAudioSources;
        private ComponentPool<AudioSource> _pool;
        private AudioSource _currentMusicAudioSource;
        private AudioSource _nextMusicAudioSource;
        private bool _isTransitioningMusic;

        protected override void Awake()
        {
            base.Awake();

            // Setup local state
            _2DSoundEffectsPlayedThisFrame = new HashSet<AudioClip>();
            _activeAudioSources = new List<AudioSource>();
            _pool = new ComponentPool<AudioSource>(gameObject, _audioSourcePrefab, _initialPoolSize);

            // Setup static music sources
            _currentMusicAudioSource = _pool.Spawn();
            _currentMusicAudioSource.transform.SetParent(transform);
            _currentMusicAudioSource.outputAudioMixerGroup = _bgmMixer;
            _currentMusicAudioSource.loop = true;

            _nextMusicAudioSource = _pool.Spawn();
            _nextMusicAudioSource.transform.SetParent(transform);
            _nextMusicAudioSource.outputAudioMixerGroup = _bgmMixer;
            _nextMusicAudioSource.loop = true;

            // Events
            _appSetupCompleted.AddListener(OnAppSetupComplete);

            _settingsAppSystem.Updated += OnSettingsUpdated;
        }

        private void OnDestroy()
        {
            // Events
            _appSetupCompleted.RemoveListener(OnAppSetupComplete);

            _settingsAppSystem.Updated -= OnSettingsUpdated;
        }

        private void LateUpdate()
        {
            _2DSoundEffectsPlayedThisFrame.Clear();

            CheckActiveSources();
        }

        /// <summary>
        /// Play's a 2D sound effect.
        /// </summary>
        /// <param name="audioClip"><see cref="AudioClip"/> of the sound to play.</param>
        /// <param name="stereoBlend">Optional argument, -1 is left, +1 is right. 0 by default</param>
        public void PlaySound2D(AudioClip audioClip, float stereoBlend = 0)
        {
            // Don't play multiple of the same 2D SFX on the same frame
            if (_2DSoundEffectsPlayedThisFrame.Contains(audioClip))
            {
                return;
            }

            _2DSoundEffectsPlayedThisFrame.Add(audioClip);

            var audioSource = _pool.Spawn();
            audioSource.transform.SetParent(transform);
            audioSource.spatialBlend = 0f;
            audioSource.clip = audioClip;
            audioSource.outputAudioMixerGroup = _sfxMixer;
            audioSource.Play();

            _activeAudioSources.Add(audioSource);
        }

        /// <summary>
        /// Play's a 3D sound effect.
        /// </summary>
        /// <param name="audioClip"><see cref="AudioClip"/> of the sound to play.</param>
        /// <param name="position">The position the audio source should be played at.</param>
        /// <param name="spatialBlend">Optional param: Controls the amount of 3D audio contribution this SFX should
        /// play at (0-1). Default value is 0.5.</param>
        public void PlaySound3D(AudioClip audioClip, Vector3 position, float spatialBlend = 0.5f)
        {
            // Don't play multiple of the same SFX on the same frame
            if (_2DSoundEffectsPlayedThisFrame.Contains(audioClip))
            {
                return;
            }

            var audioSource = _pool.Spawn();
            audioSource.transform.position = position;
            audioSource.spatialBlend = Mathf.Clamp01(spatialBlend);
            audioSource.clip = audioClip;
            audioSource.outputAudioMixerGroup = _sfxMixer;
            audioSource.Play();

            _activeAudioSources.Add(audioSource);
        }

        /// <summary>
        /// Play's a 3D sound effect.
        /// </summary>
        /// <param name="audioClip"><see cref="AudioClip"/> of the sound to play.</param>
        /// <param name="targetTransform">The transform the audio source should be parented to.</param>
        /// <param name="spatialBlend">Optional param: Controls the amount of 3D audio contribution this SFX should
        /// play at (0-1). Default value is 0.5.</param>
        public void PlaySound3D(AudioClip audioClip, Transform targetTransform, float spatialBlend = 0.5f)
        {
            // Don't play multiple of the same SFX on the same frame
            if (_2DSoundEffectsPlayedThisFrame.Contains(audioClip))
            {
                return;
            }

            var audioSource = _pool.Spawn();
            audioSource.transform.SetParent(targetTransform);
            audioSource.transform.localPosition = Vector3.zero;
            audioSource.spatialBlend = Mathf.Clamp01(spatialBlend);
            audioSource.clip = audioClip;
            audioSource.outputAudioMixerGroup = _sfxMixer;
            audioSource.Play();

            _activeAudioSources.Add(audioSource);
        }

        /// <summary>
        /// Plays the music specified on loop. If music is already playing, a cross fade will occur between the last
        /// played music and this new clip.
        /// </summary>
        /// <param name="audioClip"><see cref="AudioClip"/> of the music to be played</param>
        /// <param name="crossFadeTime">Time for cross-fade to occur. Negative value denotes that it will use the
        /// default crossfade time.</param>
        public void PlayMusic(AudioClip audioClip, float crossFadeTime = -1)
        {
            // If in the middle of switching between music tracks or starting a new one, do nothing.
            if (_isTransitioningMusic)
            {
                return;
            }

            _isTransitioningMusic = true;
            var finalCrossFadeTime = crossFadeTime < 0 ? _defaultCrossFadeTime : crossFadeTime;

            // If there is a music track playing, fade it out, then fade in the new clip
            if (_currentMusicAudioSource.isPlaying)
            {
                var sequence = DOTween.Sequence();

                // Fade old music out
                var fadeOne = _currentMusicAudioSource.DOFade(0, finalCrossFadeTime);

                // Fade new music in. On completion swap the audio sources so the new music is set as current.
                _nextMusicAudioSource.clip = audioClip;
                var fadeTwo = _nextMusicAudioSource.DOFade(1, finalCrossFadeTime);
                _nextMusicAudioSource.Play();

                sequence.Append(fadeOne)
                    .Append(fadeTwo)
                    .OnComplete(() =>
                    {
                        _currentMusicAudioSource.Stop();
                        _currentMusicAudioSource.clip = null;

                        // Swap the current and next music audio sources.
                        (_currentMusicAudioSource, _nextMusicAudioSource) = (_nextMusicAudioSource, _currentMusicAudioSource);

                        // Stop cross-fading
                        _isTransitioningMusic = false;
                    });

                sequence.Play();
            }
            else
            {
                // Fade new music in
                _currentMusicAudioSource.clip = audioClip;
                _currentMusicAudioSource.DOFade(1, finalCrossFadeTime).OnComplete(() =>
                {
                    // Stop cross-fading
                    _isTransitioningMusic = false;
                }).Play();
                _currentMusicAudioSource.Play();
            }
        }

        /// <summary>
        /// Fades out any music playing currently.
        /// </summary>
        public void StopMusic(float fadeTime = -1)
        {
            // If not playing music or not transitioning music, do nothing.
            if (!_currentMusicAudioSource.isPlaying || _isTransitioningMusic)
            {
                return;
            }

            var finalFadeTime = fadeTime < 0 ? _defaultCrossFadeTime : fadeTime;
            _currentMusicAudioSource.DOFade(0, finalFadeTime).OnComplete(() =>
            {
                _currentMusicAudioSource.Stop();
                _currentMusicAudioSource.clip = null;
            }).Play();
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
            _masterMixer.SetFloat(parameterName, volume);
        }

        #endregion

        /// <summary>
        /// Recycles any inactive audio sources that are no longer playing back into the pool.
        /// </summary>
        private void CheckActiveSources()
        {
            for (var i = _activeAudioSources.Count - 1; i >= 0; --i)
            {
                var source = _activeAudioSources[i];

                // If the audio source itself is not null AND it's either still playing or being loaded in, continue.
                if (source != null && (source.isPlaying || source.clip == null))
                {
                    continue;
                }

                _activeAudioSources.RemoveAt(i);

                if (source != null)
                {
                    _pool.Recycle(source);
                }
            }
        }

        /// <summary>
        /// Invoked when the app setup has completed.
        /// </summary>
        private void OnAppSetupComplete()
        {
            OnSettingsUpdated();
        }

        /// <summary>
        /// Invoked when the user settings have been updated.
        /// </summary>
        private void OnSettingsUpdated()
        {
            var userSettings = _settingsAppSystem.Settings;
            SetVolumeMaster(userSettings.MasterVolume);
            SetVolumeBGM(userSettings.MusicVolume);
            SetVolumeSFX(userSettings.SFXVolume);
        }
    }
}
