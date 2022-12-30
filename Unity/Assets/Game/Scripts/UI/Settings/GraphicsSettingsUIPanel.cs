using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Game
{
	/// <summary>
	/// A UI panel containing graphics settings.
	/// </summary>
	public sealed class GraphicsSettingsUIPanel : UIPanel
	{
		/// <summary>
		/// Returns true if the apply button should be enabled, otherwise false.
		/// </summary>
		private bool ShouldEnableApplyButton =>
			!_currentResolution.Equals(_desiredResolution) ||
			_currentFullScreenMode != _desiredFullScreenMode;

		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private TMP_Dropdown _framePerSecondModeDropdown;

		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private TMP_Dropdown _resolutionDropdown;

		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private TMP_Dropdown _fullScreenModesDropdown;

		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private Button _applyChangesButton;

		[BoxGroup(RuntimeConstants.SYSTEMS)]
		[SerializeField, Required]
		private SettingsAppSystem _settingsAppSystem;

		[BoxGroup(RuntimeConstants.SYSTEMS)]
		[SerializeField, Required]
		private GraphicsAppSystem _graphicsAppSystem;

		[BoxGroup(RuntimeConstants.DATA)]
		[SerializeField, Required]
		private FloatVariable _delayRevertChangesDurationFloatVariable;

		// Current Settings
		private Resolution? _previousResolution;
		private FullScreenMode? _previousFullScreenMode;

		// Current Settings
		private Resolution _currentResolution;
		private FullScreenMode _currentFullScreenMode;

		// Desired Settings
		private Resolution _desiredResolution;
		private FullScreenMode _desiredFullScreenMode;

		private List<FramePerSecondMode> _framePerSecondModes;
		private List<Resolution> _availableResolutions;
		private List<FullScreenMode> _availableFullScreenModes;

		private AppControl _appControl;
		private Coroutine _delayedRevertCoroutine;

		private void Awake()
		{
			_framePerSecondModes = new List<FramePerSecondMode>();
			_availableResolutions = new List<Resolution>();
			_availableFullScreenModes = new List<FullScreenMode>();

			// Global Event subscription
			_appControl = AppControl.Instance;
			_appControl.SetupCompleted += OnAppSetupCompleted;

			// UI Event subscription
			_framePerSecondModeDropdown.onValueChanged.AddListener(OnFramePerSecondModeDropdownChanged);
			_resolutionDropdown.onValueChanged.AddListener(OnResolutionDropdownChanged);
			_fullScreenModesDropdown.onValueChanged.AddListener(OnFullScreenModeDropdownChanged);

			_applyChangesButton.onClick.AddListener(OnApplyChangesButtonClicked);
		}

		private void OnDestroy()
		{
			_framePerSecondModeDropdown.onValueChanged.RemoveListener(OnFramePerSecondModeDropdownChanged);
			_resolutionDropdown.onValueChanged.RemoveListener(OnResolutionDropdownChanged);
			_fullScreenModesDropdown.onValueChanged.RemoveListener(OnFullScreenModeDropdownChanged);

			_applyChangesButton.onClick.RemoveListener(OnApplyChangesButtonClicked);
		}

		/// <inheritdoc />
		public override void Show()
		{
			// Cache current resolution and fullscreen mode
			_currentResolution = GraphicsTools.GetCurrentResolution();
			_desiredResolution = _currentResolution;

			_currentFullScreenMode = Screen.fullScreenMode;
			_desiredFullScreenMode = _currentFullScreenMode;

			UpdateGraphicsUI();

			base.Show();
		}

		/// <summary>
		/// Updates the local resolution cache and UI accordingly.
		/// </summary>
		private void SetupUI()
		{
			// Get all available frame per second modes
			_framePerSecondModes.Clear();
			_framePerSecondModes.AddRange(GraphicsTools.AvailableFramePerSecondModes);
			_framePerSecondModeDropdown.ClearOptions();
			_framePerSecondModeDropdown.AddOptions(
				_framePerSecondModes.Select(x => new TMP_Dropdown.OptionData(x.ToString())).ToList());

			// Cache current resolution and fullscreen mode
			_currentResolution = GraphicsTools.GetCurrentResolution();
			_desiredResolution = _currentResolution;

			_currentFullScreenMode = Screen.fullScreenMode;
			_desiredFullScreenMode = _currentFullScreenMode;

			// Get all available resolutions and setup options for them in dropdown
			GraphicsTools.GetAvailableResolutionsNonAlloc(ref _availableResolutions);

			_resolutionDropdown.ClearOptions();
			_resolutionDropdown.AddOptions(
				_availableResolutions.Select(x => new TMP_Dropdown.OptionData(x.ToString())).ToList());

			// Set the option for the current resolution
			var resolutionIndex = _availableResolutions.IndexOf(_currentResolution);
			_resolutionDropdown.SetValueWithoutNotify(resolutionIndex);

			// Get all available full screen modes
			_availableFullScreenModes.Clear();
			_availableFullScreenModes.AddRange(GraphicsTools.AvailableFullScreenModes);

			_fullScreenModesDropdown.ClearOptions();
			_fullScreenModesDropdown.AddOptions(_availableFullScreenModes.Select(x => x.ToString()).ToList());
		}

		/// <summary>
		/// Updates the graphics UI based on user input and settings.
		/// </summary>
		private void UpdateGraphicsUI()
		{
			// Set current option for frame per second mode.
			var currentFramePerSecondModeIndex =
				_framePerSecondModes.IndexOf(_settingsAppSystem.Settings.FramePerSecondMode);

			_framePerSecondModeDropdown.SetValueWithoutNotify(currentFramePerSecondModeIndex);

			// Set the option for the current resolution
			var resolutionIndex = _availableResolutions.IndexOf(_currentResolution);
			_resolutionDropdown.SetValueWithoutNotify(resolutionIndex);

			// Set option for current full screen.
			var fullScreenModeIndex = _availableFullScreenModes.IndexOf(Screen.fullScreenMode);
			_fullScreenModesDropdown.SetValueWithoutNotify(fullScreenModeIndex);

			_applyChangesButton.interactable = ShouldEnableApplyButton;
		}

		/// <summary>
		/// Invoked when the <see cref="AppControl.SetupCompleted"/> event has been invoked.
		/// </summary>
		private void OnAppSetupCompleted()
		{
			SetupUI();
		}

		/// <summary>
		/// Invoked when the frame per second dropdown has changed.
		/// </summary>
		private void OnFramePerSecondModeDropdownChanged(int newIndex)
		{
			// When changed set the new FPS mode immediately.
			var newFramePerSecondMode = _framePerSecondModes[newIndex];

			_settingsAppSystem.Settings.FramePerSecondMode = newFramePerSecondMode;
			_graphicsAppSystem.SetFrameMode(newFramePerSecondMode);
		}

		/// <summary>
		/// Invoked when the resolution dropdown had changed.
		/// </summary>
		private void OnResolutionDropdownChanged(int newIndex)
		{
			_desiredResolution = _availableResolutions[newIndex];
			_applyChangesButton.interactable = ShouldEnableApplyButton;
		}

		/// <summary>
		/// Invoked when the full screen mode dropdown has changed.
		/// </summary>
		private void OnFullScreenModeDropdownChanged(int newIndex)
		{
			_desiredFullScreenMode = _availableFullScreenModes[newIndex];
			_applyChangesButton.interactable = ShouldEnableApplyButton;
		}

		/// <summary>
		/// Invoked when the apply changes button is clicked.
		/// </summary>
		private void OnApplyChangesButtonClicked()
		{
			// Set the previous resolution and new resolution
			_previousResolution = _currentResolution;
			_previousFullScreenMode = _currentFullScreenMode;

			Screen.SetResolution(
				_desiredResolution.width,
				_desiredResolution.height,
				_desiredFullScreenMode,
				_desiredResolution.refreshRate);

			// Show a modal prompting the player about the changes.
			var modalWindow = SimpleModalWindow.Create();
			modalWindow
				.SetHeader("Window Change")
				.SetBody(CreateSettingsModalText(_delayRevertChangesDurationFloatVariable))
				.AddButton("Yes", () => StartCoroutine(DelayedCacheOfGraphicsFields()), ModalButtonType.Success)
				.AddButton("No", RevertToPreviousWindowSettings, ModalButtonType.Danger)
				.Show();

			// Start a delayed timer to revert to the previous changes in case it doesn't work well for the player
			// And they can't click it otherwise.
			if (_delayedRevertCoroutine != null)
			{
				StopCoroutine(_delayedRevertCoroutine);
				_delayedRevertCoroutine = null;
			}

			_delayedRevertCoroutine = StartCoroutine(DelayedRevertOfWindowSettings(modalWindow));
		}

		/// <summary>
		/// Invoked when a player chooses to revert their graphics settings.
		/// </summary>
		private void RevertToPreviousWindowSettings()
		{
			Assert.IsTrue(_previousResolution.HasValue);
			Assert.IsTrue(_previousFullScreenMode.HasValue);

			_desiredResolution = _previousResolution.Value;
			_desiredFullScreenMode = _previousFullScreenMode.Value;

			Screen.SetResolution(
				_desiredResolution.width,
				_desiredResolution.height,
				_desiredFullScreenMode,
				_desiredResolution.refreshRate);

			StartCoroutine(DelayedCacheOfGraphicsFields());
		}

		/// <summary>
		/// Check the remaining duration of time for the player to accept the new graphics changes and if not accepted
		/// or cancelled revert those changes and close the modal.
		/// </summary>
		private IEnumerator DelayedRevertOfWindowSettings(SimpleModalWindow modalWindow)
		{
			var counter = 0f;
			while (counter < _delayRevertChangesDurationFloatVariable.Value)
			{
				yield return new WaitForSeconds(1f);
				counter += 1f;

				var remainingDuration = _delayRevertChangesDurationFloatVariable.Value - counter;

				modalWindow.SetBody(CreateSettingsModalText(remainingDuration));
			}

			modalWindow.Close();

			RevertToPreviousWindowSettings();
		}

		/// <summary>
		/// Delays assignment of cached graphics data till after the next frame the screen has been resized.
		/// </summary>
		private IEnumerator DelayedCacheOfGraphicsFields()
		{
			if (_delayedRevertCoroutine != null)
			{
				StopCoroutine(_delayedRevertCoroutine);
				_delayedRevertCoroutine = null;
			}

			_previousResolution = null;
			_previousFullScreenMode = null;

			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();

			_desiredResolution = GraphicsTools.GetCurrentResolution();
			_currentResolution = _desiredResolution;
			_currentFullScreenMode = _desiredFullScreenMode;

			UpdateGraphicsUI();
		}

		/// <summary>
		/// Creates the body text for the modal window of graphics setting changes
		/// </summary>
		private string CreateSettingsModalText(float remainingDuration)
		{
			return string.Format(
				"Do you want to keep these settings? They will revert to previous settings in {0} seconds.",
				remainingDuration);
		}
	}
}
