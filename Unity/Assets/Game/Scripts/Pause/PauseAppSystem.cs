using System;
using NaughtyAttributes;
using ScriptableObjectArchitecture;
using UnityEngine;

namespace Game
{
	/// <summary>
	/// A <see cref="IAppSystem"/> app system for handling pausing.
	/// </summary>
	[CreateAssetMenu(
		fileName = "PauseAppSystem",
		menuName = "Game/Systems/PauseAppSystem")]
	public sealed class PauseAppSystem : ScriptableAppSystem
	{
		/// <summary>
		/// Invoked when the pause state of the game has changed.
		/// </summary>
		public event Action<bool> PauseValueChanged;

		/// <summary>
		/// Returns true if the game is paused, otherwise false.
		/// </summary>
		public bool IsGamePaused => _isGamePausedBoolVariable.Value;

		[BoxGroup(RuntimeConstants.DATA)]
		[SerializeField, Required]
		private BoolVariable _isGamePausedBoolVariable;

		[BoxGroup(RuntimeConstants.DATA)]
		[SerializeField, Required]
		private BoolVariable _isGameLoadedBoolVariable;

		[BoxGroup(RuntimeConstants.SETTINGS)]
		[InfoBox("If enabled, Time.timeScale will be affected by the pause, otherwise it will not be.")]
		[SerializeField]
		private bool _doPauseTimescale;

		/// <inheritdoc />
		public override void OneTimeSetup()
		{
			_isGameLoadedBoolVariable.AddListener(OnGameLoadValueChanged);

			ResetState();
		}

		/// <inheritdoc />
		public override void OneTimeTeardown()
		{
			_isGameLoadedBoolVariable.RemoveListener(OnGameLoadValueChanged);

			ResetState();
		}

		/// <summary>
		/// Toggles the pause state of the game on or off.
		/// </summary>
		public void TogglePause()
		{
			var newPause = !_isGamePausedBoolVariable.Value;

			SetPause(newPause);
		}

		/// <summary>
		/// Sets the pause state of the game. If already set equal to <paramref name="isPaused"/> resolves to a no-op.
		/// </summary>
		public void SetPause(bool isPaused)
		{
			// If the game is not yet loaded or already set to this paused state, do nothing.
			if (!_isGameLoadedBoolVariable.Value || _isGamePausedBoolVariable.Value == isPaused)
			{
				return;
			}

			// If we are affecting timescale, set the timescale value.
			if (_doPauseTimescale)
			{
				Time.timeScale = isPaused ? 0f : 1f;
			}

			// Let external listeners know of pause changes.
			_isGamePausedBoolVariable.Value = isPaused;
			PauseValueChanged?.Invoke(isPaused);
		}

		/// <summary>
		/// Resets all pause state to defaults.
		/// </summary>
		private void ResetState()
		{
			Time.timeScale = 1f;
			_isGamePausedBoolVariable.Value = false;
			PauseValueChanged?.Invoke(false);
		}

		/// <summary>
		/// Invoked when the <see cref="_isGameLoadedBoolVariable"/> value changes.
		/// </summary>
		private void OnGameLoadValueChanged()
		{
			// Whenever the game is loaded, reset the pause state.
			ResetState();
		}
	}
}
