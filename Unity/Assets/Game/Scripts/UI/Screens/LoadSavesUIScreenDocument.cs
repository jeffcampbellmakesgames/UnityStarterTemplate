using System.Collections.Generic;
using JCMG.Slate;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Game
{
    /// <summary>
	/// A UI document screen for loading saves.
	/// </summary>
	public sealed class LoadSavesUIScreenDocument : UIScreenDocumentBase
	{
		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private VisualTreeAsset _saveDataEntryTemplate;

		[BoxGroup(RuntimeConstants.SYSTEMS)]
		[SerializeField, Required]
		private SavesAppSystem _savesAppSystem;

		[BoxGroup(RuntimeConstants.SYSTEMS)]
		[SerializeField, Required]
		private StyleSheet _gameStyleSheet;

		private List<SaveData> SaveData => _saveData ??= new List<SaveData>();

		private Button _exitButton;
		private ListView _saveDataListView;
		private List<SaveData> _saveData;
		private SaveData _selectedSaveData;

		protected override void Awake()
		{
			base.Awake();

			_exitButton = _uiDocument.rootVisualElement.Q<Button>("CloseButton");
			_saveDataListView = _uiDocument.rootVisualElement.Q<ListView>("SaveDataListView");

			_exitButton.clicked += OnExitButtonClicked;

			OneTimeSetupSaveFileListView();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			_exitButton.clicked -= OnExitButtonClicked;
			_saveDataListView.selectionChanged -= OnSaveDataSelected;
		}

		/// <inheritdoc />
		public override void Show(bool immediate = false)
		{
			base.Show(immediate);

			SetData(_savesAppSystem.LoadedSaveFiles);
		}

		/// <inheritdoc />
		public override void Hide(bool immediate = false)
		{
			base.Hide(immediate);

			// Clear any local state
			_selectedSaveData = null;

			// Clear the UI state
			SaveData.Clear();
		}

		private void OneTimeSetupSaveFileListView()
		{
			// Assign the delegates for creating and binding data to the list. Without these the listview won't populate.
			_saveDataListView.makeItem = () =>
			{
				var entry = _saveDataEntryTemplate.Instantiate();
				entry.AddToClassList("list-item-container");
				entry.styleSheets.Add(_gameStyleSheet);

				var controller = new SaveDataUIDisplayController();
				controller.profileNameText = entry.contentContainer.Q<Label>("ProfileNameLabel");
				controller.lastUpdatedText = entry.contentContainer.Q<Label>("LastUpdatedLabel");

				entry.userData = controller;
				return entry;
			};

			_saveDataListView.bindItem = (item, index) =>
			{
				((SaveDataUIDisplayController)item.userData).SetData(_saveData[index]);
			};

			// Set the item source.
			_saveDataListView.itemsSource = SaveData;

			_saveDataListView.selectionChanged += OnSaveDataSelected;
		}

		/// <summary>
		/// Sets the data needed to be displayed for this UI.
		/// </summary>
		private void SetData(IReadOnlyList<SaveData> saveData)
		{
			// Cache data locally.
			SaveData.Clear();
			SaveData.AddRange(saveData);
		}

		/// <summary>
		/// Invoked when a player clicks a <see cref="SaveDataUIDisplay"/>.
		/// </summary>
		private void OnSaveDataSelected(IEnumerable<object> enumerable)
		{
			if (_saveDataListView.selectedItem == null)
			{
				return;
			}

			_selectedSaveData = _saveDataListView.selectedItem as SaveData;

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

			var mainMenuUIScreen = UIScreenControl.GetScreen<MainMenuUIScreenDocument>();
			mainMenuUIScreen.Show();
		}

		/// <summary>
		/// Invoked when a player cancels loading a specific save.
		/// </summary>
		private void OnCancelLoadSaveConfirmed()
		{
			_selectedSaveData = null;
			_saveDataListView.ClearSelection();
		}
	}
}
