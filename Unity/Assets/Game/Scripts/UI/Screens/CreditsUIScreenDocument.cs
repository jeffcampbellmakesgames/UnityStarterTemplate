using JCMG.Slate;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game
{
    /// <summary>
    /// A UI document screen for displaying credits and information.
    /// </summary>
    public sealed class CreditsUIScreenDocument : UIScreenDocumentBase
    {
        private Button _exitButton;

        protected override void Awake()
        {
            base.Awake();

            _exitButton = _rootElement.Q<Button>("CloseButton");

            _exitButton.clicked += OnExitButtonClicked;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _exitButton.clicked -= OnExitButtonClicked;
        }

        /// <summary>
        /// Invoked when the exit button is clicked.
        /// </summary>
        private void OnExitButtonClicked()
        {
            Hide();
        }
    }
}
