using JCMG.Slate;
using NaughtyAttributes;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// Represents the main menu UI hub where the player is placed when starting the application and enables easy access
    /// to starting the game, configuring their settings, and viewing the credits.
    /// </summary>
    public sealed class MainMenuUIScreen : UIScreen
    {
        [BoxGroup(RuntimeConstants.UI_REFS)]
        [SerializeField, Required]
        private Button _playGameButton;

        [BoxGroup(RuntimeConstants.UI_REFS)]
        [SerializeField, Required]
        private Button _settingsGameButton;

        [BoxGroup(RuntimeConstants.UI_REFS)]
        [SerializeField, Required]
        private Button _creditsGameButton;

        [BoxGroup(RuntimeConstants.EVENTS)]
        [SerializeField, Required]
        private GameEvent _setupCompletedEvent;

        protected override void Awake()
        {
            base.Awake();

            // Events
            _playGameButton.onClick.AddListener(OnPlayGameButtonClicked);
            _settingsGameButton.onClick.AddListener(OnSettingsButtonClicked);
            _creditsGameButton.onClick.AddListener(OnCreditsButtonClicked);

            _setupCompletedEvent.AddListener(OnAppSetupComplete);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Events
            _playGameButton.onClick.RemoveListener(OnPlayGameButtonClicked);
            _settingsGameButton.onClick.RemoveListener(OnSettingsButtonClicked);
            _creditsGameButton.onClick.RemoveListener(OnCreditsButtonClicked);

            _setupCompletedEvent.RemoveListener(OnAppSetupComplete);
        }

        /// <summary>
        /// Invoked when <see cref="AppControl"/> has completed setup.
        /// </summary>
        private void OnAppSetupComplete()
        {
            Show();
        }

        /// <summary>
        /// Invoked when the play game button is clicked.
        /// </summary>
        private void OnPlayGameButtonClicked()
        {
            // TODO
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Invoked when the settings button is clicked.
        /// </summary>
        private void OnSettingsButtonClicked()
        {
            var settingsUIScreen = UIScreenControl.GetPanel<SettingsUIScreen>();
            settingsUIScreen.Show();
        }

        /// <summary>
        /// Invoked when the credits button is clicked.
        /// </summary>
        private void OnCreditsButtonClicked()
        {
            // TODO
        }
    }
}
