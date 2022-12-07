namespace Game
{
	/// <summary>
	/// Represents an app-wide system that requires startup and teardown as part of the app lifecycle.
	/// </summary>
	public interface IAppSystem
	{
		/// <summary>
		/// A one-time setup method called when the application is started.
		/// </summary>
		void OneTimeSetup();

		/// <summary>
		/// A one-time teardown method called when the application is exited.
		/// </summary>
		void OneTimeTeardown();

		/// <summary>
		/// Returns true if setup has completed, otherwise false.
		/// </summary>
		bool IsSetupComplete();
	}
}
