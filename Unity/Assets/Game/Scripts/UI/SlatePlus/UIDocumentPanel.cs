using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game
{
	/// <summary>
	/// Represents a distinct UI panel as part of a larger <see cref="UIScreen"/>.
	/// </summary>
	public abstract class UIDocumentPanel : MonoBehaviour
	{
		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		protected UIDocument _uiDocument;

		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField]
		private string _hideStyleClass;

		/// <summary>
		/// The visual element we are targeting as the root of this <see cref="UIDocumentPanel"/>.
		/// </summary>
		protected abstract VisualElement TargetRootVisualElement { get; }

		/// <summary>
		/// Shows this <see cref="UIDocumentPanel"/> and makes it able to be interacted with.
		/// </summary>
		public virtual void Show()
		{
			if (string.IsNullOrEmpty(_hideStyleClass))
			{
				TargetRootVisualElement.style.display = DisplayStyle.Flex;
			}
			else if(TargetRootVisualElement.ClassListContains(_hideStyleClass))
			{
				TargetRootVisualElement.RemoveFromClassList(_hideStyleClass);
			}

			TargetRootVisualElement.pickingMode = PickingMode.Position;
		}

		/// <summary>
		/// Hides this <see cref="UIDocumentPanel"/> and prevents it from being interacted with.
		/// </summary>
		public virtual void Hide()
		{
			if (string.IsNullOrEmpty(_hideStyleClass))
			{
				TargetRootVisualElement.style.display = DisplayStyle.None;
			}
			else if(!TargetRootVisualElement.ClassListContains(_hideStyleClass))
			{
				TargetRootVisualElement.AddToClassList(_hideStyleClass);
			}

			TargetRootVisualElement.pickingMode = PickingMode.Ignore;
		}
	}
}
