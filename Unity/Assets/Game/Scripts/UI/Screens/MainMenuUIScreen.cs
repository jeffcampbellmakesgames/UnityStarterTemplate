using JCMG.Slate;
using NaughtyAttributes;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game
{
	/// <summary>
	/// Represents the main menu UI hub where the player is placed when starting the application and enables easy access
	/// to starting the game, configuring their settings, and viewing the credits.
	/// </summary>
	public sealed class MainMenuUIScreen : UIScreenDocumentBase
	{
		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private SavesAppSystem _savesAppSystem;

		[BoxGroup(RuntimeConstants.EVENTS)]
		[SerializeField, Required]
		private GameEvent _setupCompletedEvent;

		[BoxGroup(RuntimeConstants.EVENTS)]
		[SerializeField, Required]
		private GameEvent _gameExitedEvent;

		[BoxGroup(RuntimeConstants.DATA)]
		[SerializeField, Required]
		private BoolVariable _isNewGameStartedBoolVariable;

		private Button _newGameButton;
		private Button _continueButton;
		private Button _loadSavesButton;
		private Button _settingsGameButton;
		private Button _creditsGameButton;
		private GameControl _gameControl;

		protected override void Awake()
		{
			base.Awake();

			_newGameButton = _uiDocument.rootVisualElement.Q<Button>("NewGameButton");
			_settingsGameButton = _uiDocument.rootVisualElement.Q<Button>("SettingsButton");
			_creditsGameButton = _uiDocument.rootVisualElement.Q<Button>("CreditsButton");
			_continueButton = _uiDocument.rootVisualElement.Q<Button>("ContinueButton");
			_loadSavesButton = _uiDocument.rootVisualElement.Q<Button>("LoadSaveButton");

			// UI Events
			_newGameButton.clicked += OnNewGameButtonClicked;
			_settingsGameButton.clicked += OnSettingsButtonClicked;
			_creditsGameButton.clicked += OnCreditsButtonClicked;
			_continueButton.clicked += OnContinueSaveButton;
			_loadSavesButton.clicked += OnLoadSavesButton;

			// Data Events
			_setupCompletedEvent.AddListener(OnAppSetupComplete);
			_gameExitedEvent.AddListener(OnGameExited);

			// Singleton Events
			_gameControl = GameControl.Instance;
			_gameControl.GameLoadingStarted += OnGameLoadingStarted;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			// UI Events
			_newGameButton.clicked -= OnNewGameButtonClicked;
			_settingsGameButton.clicked -= OnSettingsButtonClicked;
			_creditsGameButton.clicked -= OnCreditsButtonClicked;
			_continueButton.clicked -= OnContinueSaveButton;
			_loadSavesButton.clicked -= OnLoadSavesButton;

			// Data Events
			_setupCompletedEvent.RemoveListener(OnAppSetupComplete);
			_gameExitedEvent.RemoveListener(OnGameExited);

			// Singleton Events
			_gameControl.GameLoadingStarted -= OnGameLoadingStarted;
		}


		/// <inheritdoc />
		public new void Show(bool immediate = false)
		{
			_continueButton.style.display = _savesAppSystem.HasAnySaveFiles ? DisplayStyle.Flex : DisplayStyle.None;
			_loadSavesButton.style.display = _savesAppSystem.HasAnySaveFiles ? DisplayStyle.Flex : DisplayStyle.None;

			base.Show(immediate);
		}

		/// <summary>
		/// Invoked when the new game button is clicked.
		/// </summary>
		private void OnNewGameButtonClicked()
		{
			var inputModalDialog = ModalWindow<InputModalWindow>.Create();
			inputModalDialog
				.SetHeader("Input Name")
				.SetBody("Please select a profile name for this save file")
				.SetInputField(OnProfileNameSubmitted, "", "Enter name here...")
				.Show();
		}

		/// <summary>
		/// Invoked when a player submits a profile name.
		/// </summary>
		private void OnProfileNameSubmitted(string profileName)
		{
			// TODO If the profile name is blank, launch another modal indicating a non-blank name is required.

			// TODO Add validation for profile name to ensure it is not:
			// * Blank
			// * Safe for file name

			// Set the flag to indicate it's a new game.
			_isNewGameStartedBoolVariable.Value = true;

			// Otherwise create the new game with this profile and start loading it.
			GameControl.Instance.CreateNewGame(profileName);
		}

		/// <summary>
		/// Invoked when the settings button is clicked.
		/// </summary>
		private void OnSettingsButtonClicked()
		{
			var settingsUIScreen = UIScreenControl.GetScreen<SettingsUIScreen>();
			settingsUIScreen.Show();

			Hide();
		}

		/// <summary>
		/// Invoked when the credits button is clicked.
		/// </summary>
		private void OnCreditsButtonClicked()
		{
			var creditsUIScreen = UIScreenControl.GetScreen<CreditsUIScreen>();
			creditsUIScreen.Show();
		}

		/// <summary>
		/// Invoked when the player chooses to continue a previous save.
		/// </summary>
		private void OnContinueSaveButton()
		{
			_isNewGameStartedBoolVariable.Value = false;

			GameControl.Instance.EnterLastUpdatedGame();
		}

		/// <summary>
		/// Invoked when the player chooses to load a previous save.
		/// </summary>
		private void OnLoadSavesButton()
		{
			var loadSavesUIScreen = UIScreenControl.GetScreen<LoadSavesUIScreen>();
			loadSavesUIScreen.Show();
		}

		/// <summary>
		/// Invoked when <see cref="AppControl"/> has completed setup.
		/// </summary>
		private void OnAppSetupComplete()
		{
			Show();
		}

		/// <summary>
		/// Invoked when the game is exited from back to the main menu.
		/// </summary>
		private void OnGameExited()
		{
			Show();
		}

		/// <summary>
		/// Invoked when the game loading has begun.
		/// </summary>
		private void OnGameLoadingStarted()
		{
			Hide();
		}
	}
}
