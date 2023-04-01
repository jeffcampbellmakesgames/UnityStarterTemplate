using System;
using JCMG.Utility;
using NaughtyAttributes;
using ScriptableObjectArchitecture;
using UnityEngine;

namespace Game
{
	/// <summary>
	/// Represents a specific level instance.
	/// </summary>
	[Serializable]
	public sealed class LevelData : ISymbolObject
	{
		/// <inheritdoc />
		public string Symbol => _symbolReference.Value;

		/// <summary>
		/// The name of the scene for this level.
		/// </summary>
		public string SceneName => _sceneName;

		[SerializeField]
		private StringReference _symbolReference;

		[SerializeField, Scene]
		private string _sceneName;
	}
}
