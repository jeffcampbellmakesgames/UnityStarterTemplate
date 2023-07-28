using JCMG.Slate;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	/// <summary>
	/// A UI screen to display Game Over information.
	/// </summary>
	public sealed class GameOverUIScreen : UIScreen
	{
		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private Button _continueButton;

		protected override void Awake()
		{
			base.Awake();

			_continueButton.onClick.AddListener(OnContinueButtonClicked);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			_continueButton.onClick.RemoveListener(OnContinueButtonClicked);
		}

		public void OnContinueButtonClicked()
		{
			Hide();

			GameControl.Instance.ExitGame();
		}
	}
}
