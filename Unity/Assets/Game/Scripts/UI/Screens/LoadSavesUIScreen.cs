using System.Collections.Generic;
using JCMG.Slate;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Game
{
	/// <summary>
	/// A <see cref="UIScreen"/> for loading saves.
	/// </summary>
	public sealed class LoadSavesUIScreen : UIScreen
	{
		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private Button _exitButton;

		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private SaveDataUIList _saveDataUIList;

		[BoxGroup(RuntimeConstants.SYSTEMS)]
		[SerializeField, Required]
		private SavesAppSystem _savesAppSystem;

		private IReadOnlyList<SaveData> _saveData;
		private SaveData _selectedSaveData;

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

		/// <inheritdoc />
		public override void Show(bool immediate = false)
		{
			base.Show(immediate);

			SetData(_savesAppSystem.LoadedSaveFiles);

			_saveDataUIList.UpdateDisplay();
			_saveDataUIList.SetFirstAsSelected();
		}

		/// <inheritdoc />
		public override void Hide(bool immediate = false)
		{
			base.Hide(immediate);

			// Clear any local state
			_saveData = null;
			_selectedSaveData = null;

			// Clear the UI state
			_saveDataUIList.Clear();
		}

		/// <summary>
		/// Sets the data needed to be displayed for this UI.
		/// </summary>
		public void SetData(IReadOnlyList<SaveData> saveData)
		{
			// Cache data locally.
			_saveData = saveData;

			// Set data into UI list.
			_saveDataUIList.Set(_saveData, OnSaveDataSelected);
		}

		/// <summary>
		/// Invoked when a player clicks a <see cref="SaveDataUIDisplay"/>.
		/// </summary>
		private void OnSaveDataSelected(SaveData saveData)
		{
			_selectedSaveData = saveData;

			var modalDialog = ModalWindow<SimpleModalWindow>.Create();

			modalDialog
				.SetHeader("Load Save?")
				.SetBody("Are you sure you want to load this save?")
				.AddButton("Cancel", OnCancelLoadSaveConfirmed)
				.AddButton("Yes", OnLoadSaveConfirmed, ModalButtonType.Success)
				.Show();
		}

		/// <summary>
		/// Invoked when a player confirms they would like to load a save.
		/// </summary>
		private void OnLoadSaveConfirmed()
		{
			Assert.IsNotNull(_selectedSaveData);

			GameControl.Instance.EnterGameForSaveData(_selectedSaveData);

			Hide();
		}

		/// <summary>
		/// Invoked when the exit button is clicked.
		/// </summary>
		private void OnExitButtonClicked()
		{
			Hide();
		}

		/// <summary>
		/// Invoked when a player cancels loading a specific save.
		/// </summary>
		private void OnCancelLoadSaveConfirmed()
		{
			_selectedSaveData = null;
		}
	}
}
