namespace Game
{
	/// <summary>
	/// Represents an app-wide system that requires startup and teardown as part of the app lifecycle.
	/// </summary>
	public interface IGameSystem : IAppSystem
	{
		/// <summary>
		/// A one-time setup method called when the game is started.
		/// </summary>
		void Setup();

		/// <summary>
		/// A one-time teardown method called when the game is exited.
		/// </summary>
		void Teardown();
	}
}
