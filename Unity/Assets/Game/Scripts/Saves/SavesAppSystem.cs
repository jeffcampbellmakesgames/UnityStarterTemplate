using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NaughtyAttributes;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
	/// <summary>
	/// A <see cref="IAppSystem"/> app system for handling save files.
	/// </summary>
	[CreateAssetMenu(
		fileName = "SavesAppSystem",
		menuName = "Game/Systems/SavesAppSystem")]
	public sealed class SavesAppSystem : ScriptableAppSystem
	{
		/// <summary>
		/// Returns true if there are any existing save files, otherwise false.
		/// </summary>
		public bool HasAnySaveFiles => LoadedSaveFiles.Count > 0;

		/// <summary>
		/// A read-only collection of all loaded save files.
		/// </summary>
		public IReadOnlyList<SaveData> LoadedSaveFiles => _loadedSaveFiles;

		/// <summary>
		/// Returns true if any save data is currently loaded, otherwise false.
		/// </summary>
		public bool HasCurrentSaveDataSet => _currentSaveData != null;

		/// <summary>
		/// The currently loaded save data.
		/// </summary>
		public SaveData CurrentSaveData => _currentSaveData;

		/// <summary>
		/// The absolute path to the save file directory.
		/// </summary>
		public string SaveFileDirectoryAbsolutePath => _saveFileDirectoryAbsolutePath;

		[BoxGroup(RuntimeConstants.SETTINGS)]
		[SerializeField]
		private string _saveFolderName;

		private List<SaveData> _loadedSaveFiles;
		private Dictionary<string, SaveData> _loadedSaveFileLookupByFilePath;
		private string _saveFileDirectoryAbsolutePath;
		private SaveData _currentSaveData;

		/// <inheritdoc />
		public override void OneTimeSetup()
		{
			// Setup local state
			_loadedSaveFiles = new List<SaveData>();
			_loadedSaveFileLookupByFilePath = new Dictionary<string, SaveData>();
			_saveFileDirectoryAbsolutePath = Path.Combine(Application.persistentDataPath, _saveFolderName);

			// Create directory if doesn't exist
			if (!Directory.Exists(_saveFileDirectoryAbsolutePath))
			{
				Directory.CreateDirectory(_saveFileDirectoryAbsolutePath);
			}

			// Load all save files into memory.
			var jsonFilePaths = Directory.GetFiles(_saveFileDirectoryAbsolutePath, "*.json");
			for (var i = 0; i < jsonFilePaths.Length; i++)
			{
				var filePath = jsonFilePaths[i];
				var saveFile = JsonConvert.DeserializeObject<SaveData>(
					File.ReadAllText(filePath),
					RuntimeConstants.JSON_SETTINGS);
				if (saveFile != null)
				{
					_loadedSaveFiles.Add(saveFile);
					_loadedSaveFileLookupByFilePath.Add(filePath, saveFile);
				}
			}
		}

		/// <summary>
		/// Creates a new save file and returns it.
		/// </summary>
		/// <param name="profileName"></param>
		public SaveData CreateSaveData(string profileName)
		{
			var newSaveData = new SaveData
			{
				id = Guid.NewGuid().ToString(),
				profileName = profileName,
				created = DateTime.Now,
				lastUpdated = DateTime.Now,
				lastLevelCompleted = string.Empty
			};

			var saveFilePath = Path.Combine(_saveFileDirectoryAbsolutePath, newSaveData.GetSaveFileName());

			_loadedSaveFiles.Add(newSaveData);
			_loadedSaveFileLookupByFilePath.Add(saveFilePath, newSaveData);

			return newSaveData;
		}

		/// <inheritdoc />
		public override void OneTimeTeardown()
		{
			// Flush all save files to disk.
			foreach (var kvp in _loadedSaveFileLookupByFilePath)
			{
				var saveFileText = JsonConvert.SerializeObject(
					kvp.Value,
					RuntimeConstants.JSON_SETTINGS);
				File.WriteAllText(kvp.Key, saveFileText);
			}
		}

		/// <summary>
		/// Returns the last updated save data.
		/// </summary>
		public SaveData GetLastUpdatedSaveData()
		{
			Assert.IsTrue(_loadedSaveFiles.Count > 0);

			return _loadedSaveFiles.OrderByDescending(x => x.lastUpdated).First();
		}

		/// <summary>
		/// Sets <paramref name="saveData"/> as being the currently loaded save data.
		/// </summary>
		public void SetCurrentSaveData(SaveData saveData)
		{
			_currentSaveData = saveData;
		}

		/// <summary>
		/// Unsets the current save data as being selected.
		/// </summary>
		public void UnsetCurrentSaveData()
		{
			_currentSaveData = null;
		}

		/// <summary>
		/// Deletes <paramref name="saveData"/> from memory and disk.
		/// </summary>
		public void DeleteSaveData(SaveData saveData)
		{
			var saveFilePath = Path.Combine(_saveFileDirectoryAbsolutePath, saveData.GetSaveFileName());

			_loadedSaveFileLookupByFilePath.Remove(saveFilePath);
			_loadedSaveFiles.Remove(saveData);

			File.Delete(saveFilePath);
		}

		/// <summary>
		/// Nuclear option to delete all save files in-memory and from disk.
		/// </summary>
		public void DeleteAllSaveData()
		{
			for (var i = _loadedSaveFiles.Count - 1; i >= 0; i--)
			{
				var saveData = _loadedSaveFiles[i];
				DeleteSaveData(saveData);
			}

			_loadedSaveFileLookupByFilePath.Clear();
			_loadedSaveFiles.Clear();
		}
	}
}
