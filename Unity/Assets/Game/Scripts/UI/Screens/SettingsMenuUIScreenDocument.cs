using JCMG.Slate;
using NaughtyAttributes;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game
{
    /// <summary>
    /// Displays configurable settings to the player.
    /// </summary>
    public sealed class SettingsMenuUIScreenDocument : UIScreenDocumentBase
    {
		private Button _exitButton;
		private Button _gameplaySettingsHeaderButton;
		private Button _graphicsSettingsHeaderButton;

		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private UIDocumentPanel _gameplayUIPanel;

		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private UIDocumentPanel _graphicsUIPanel;

		[BoxGroup(RuntimeConstants.SYSTEMS)]
		[SerializeField, Required]
		private SettingsAppSystem _settingsAppSystem;

		[BoxGroup(RuntimeConstants.DATA)]
		[SerializeField, Required]
		private BoolVariable _isInGameBoolVariable;

		protected override void Awake()
		{
			base.Awake();

			_exitButton = _rootElement.Q<Button>("CloseButton");
			_gameplaySettingsHeaderButton = _rootElement.Q<Button>("GameplaySettingsButton");
			_graphicsSettingsHeaderButton = _rootElement.Q<Button>("GraphicsSettingsButton");

			_exitButton.clicked += OnExitButtonClicked;
			_gameplaySettingsHeaderButton.clicked += OnGameplaySettingsHeaderClicked;
			_graphicsSettingsHeaderButton.clicked += OnGraphicsSettingsHeaderClicked;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			_exitButton.clicked -= OnExitButtonClicked;
			_gameplaySettingsHeaderButton.clicked -= OnGameplaySettingsHeaderClicked;
			_graphicsSettingsHeaderButton.clicked -= OnGraphicsSettingsHeaderClicked;
		}

		public override void Show(bool immediate = false)
		{
			// If already visible, do nothing.
			if (IsVisible)
			{
				return;
			}

			base.Show(immediate);

			_gameplayUIPanel.Show();
			_graphicsUIPanel.Hide();
		}

		/// <inheritdoc />
		public override void Hide(bool immediate = false)
		{
			// If already hidden, do nothing.
			if (!IsVisible)
			{
				return;
			}

			base.Hide(immediate);

			// On hiding the settings UI flush all settings changes to disk.
			_settingsAppSystem.FlushSettingsToDisk();
		}

		/// <summary>
		/// Invoked when the exit button is clicked.
		/// </summary>
		private void OnExitButtonClicked()
		{
			if (!_isInGameBoolVariable)
			{
				var mainMenuUIScreen = UIScreenControl.GetScreen<MainMenuUIScreenDocument>();
				mainMenuUIScreen.Show(immediate:true);
			}

			Hide();
		}

		/// <summary>
		/// Invoked when the gameplay settings header button is clicked.
		/// </summary>
		private void OnGameplaySettingsHeaderClicked()
		{
			_gameplayUIPanel.Show();
			_graphicsUIPanel.Hide();
		}

		/// <summary>
		/// Invoked when the graphics settings header button is clicked.
		/// </summary>
		private void OnGraphicsSettingsHeaderClicked()
		{
			_gameplayUIPanel.Hide();
			_graphicsUIPanel.Show();
		}
    }
}
