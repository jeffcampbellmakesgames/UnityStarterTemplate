using JCMG.Slate;
using JCMG.Utility;
using NaughtyAttributes;
using UnityEngine;

namespace Game
{
	/// <summary>
	/// Represents a distinct UI panel as part of a larger <see cref="UIScreen"/>.
	/// </summary>
	public abstract class UIPanel : MonoBehaviour
	{
		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private CanvasGroup _canvasGroup;

		/// <summary>
		/// Shows this UI panel and makes it able to be interacted with.
		/// </summary>
		public virtual void Show()
		{
			_canvasGroup.ToggleVisibilityAndInput(true);
		}

		/// <summary>
		/// Hides this UI panel and prevents it from being interacted with.
		/// </summary>
		public virtual void Hide()
		{
			_canvasGroup.ToggleVisibilityAndInput(false);
		}
	}
}
