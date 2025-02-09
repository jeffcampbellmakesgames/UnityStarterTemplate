using JCMG.Slate;
using UnityEngine.UIElements;

namespace Game
{
	/// <summary>
	/// A UI document screen to display Game Over information.
	/// </summary>
	public sealed class GameOverUIScreenDocument : UIScreenDocumentBase
	{
		private Button _closeButton;

		protected override void Awake()
		{
			base.Awake();

			_closeButton = _rootElement.Q<Button>("CloseButton");

			_closeButton.clicked += OnCloseButtonClicked;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			_closeButton.clicked -= OnCloseButtonClicked;
		}

		public void OnCloseButtonClicked()
		{
			Hide();

			GameControl.Instance.ExitGame();
		}
	}
}
