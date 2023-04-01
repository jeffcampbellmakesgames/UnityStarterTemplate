using System;
using JCMG.Slate;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	/// <summary>
	/// A UI display for save data.
	/// </summary>
	public sealed class SaveDataUIDisplay : UIDisplayBase<SaveData>, IClickableUIDisplay<SaveData>
	{
		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private Button _button;

		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private TMP_Text _profileNameText;

		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private TMP_Text _lastUpdatedText;

		private Action<SaveData> _onClickHandler;

		private void Awake()
		{
			_button.onClick.AddListener(OnClick);
		}

		private void OnDestroy()
		{
			_button.onClick.RemoveListener(OnClick);
		}

		/// <inheritdoc />
		public override void UpdateDisplay()
		{
			_profileNameText.text = _value.profileName;
			_lastUpdatedText.text = _value.lastUpdated.ToString();
		}

		/// <inheritdoc />
		public void SetOnClick(Action<SaveData> onClickHandler)
		{
			_onClickHandler = onClickHandler;
		}

		/// <summary>
		/// Clears any data from this UI display.
		/// </summary>
		public void Clear()
		{
			_onClickHandler = null;
		}

		/// <summary>
		/// Invoked when this UI is clicked.
		/// </summary>
		private void OnClick()
		{
			_onClickHandler?.Invoke(_value);
		}
	}
}
