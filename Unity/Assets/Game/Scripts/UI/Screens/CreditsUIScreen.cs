using JCMG.Slate;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	/// <summary>
	/// A <see cref="UIScreen"/> for displaying credits and information.
	/// </summary>
	public sealed class CreditsUIScreen : UIScreen
	{
		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private Button _exitButton;

		protected override void Awake()
		{
			base.Awake();

			_exitButton.onClick.AddListener(OnExitButtonClicked);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			_exitButton.onClick.RemoveListener(OnExitButtonClicked);
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
