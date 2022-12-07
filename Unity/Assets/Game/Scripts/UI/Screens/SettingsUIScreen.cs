using JCMG.Slate;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	/// <summary>
	/// Displays configurable settings to the player.
	/// </summary>
	public sealed class SettingsUIScreen : UIScreen
	{
		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private Button _exitButton;

		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private Button _gameplaySettingsHeaderButton;

		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private Button _graphicsSettingsHeaderButton;

		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private UIPanel _gameplayUIPanel;

		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private UIPanel _graphicsUIPanel;

		[BoxGroup(RuntimeConstants.SYSTEMS)]
		[SerializeField, Required]
		private SettingsAppSystem _settingsAppSystem;

		protected override void Awake()
		{
			base.Awake();

			_exitButton.onClick.AddListener(OnExitButtonClicked);
			_gameplaySettingsHeaderButton.onClick.AddListener(OnGameplaySettingsHeaderClicked);
			_graphicsSettingsHeaderButton.onClick.AddListener(OnGraphicsSettingsHeaderClicked);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			_exitButton.onClick.RemoveListener(OnExitButtonClicked);
			_gameplaySettingsHeaderButton.onClick.RemoveListener(OnGameplaySettingsHeaderClicked);
			_graphicsSettingsHeaderButton.onClick.RemoveListener(OnGraphicsSettingsHeaderClicked);
		}

		public override void Show(bool immediate = false)
		{
			base.Show(immediate);

			_gameplayUIPanel.Show();
			_graphicsUIPanel.Hide();
		}

		/// <inheritdoc />
		public override void Hide(bool immediate = false)
		{
			base.Hide(immediate);

			// On hiding the settings UI flush all settings changes to disk.
			_settingsAppSystem.FlushSettingsToDisk();
		}

		/// <summary>
		/// Invoked when the exit button is clicked.
		/// </summary>
		private void OnExitButtonClicked()
		{
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
