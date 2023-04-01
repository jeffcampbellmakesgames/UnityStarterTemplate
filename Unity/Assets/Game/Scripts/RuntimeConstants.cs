using Newtonsoft.Json;

namespace Game
{
	/// <summary>
	/// Shared constants at runtime.
	/// </summary>
	public static class RuntimeConstants
	{
		// Inspector Groups
		public const string DATA = "Data";
		public const string DEBUG = "Debug";
		public const string EVENTS = "Events";
		public const string PREFAB = "Prefab Refs";
		public const string PROJECT = "Project Refs";
		public const string PHYSICS = "Physics";
		public const string SCENE = "Scene Refs";
		public const string SETTINGS = "Settings";
		public const string SYSTEMS = "Systems";
		public const string UI_REFS = "UI Refs";

		// Json
		public static readonly JsonSerializerSettings JSON_SETTINGS = new ()
		{
			
		};
	}
}
