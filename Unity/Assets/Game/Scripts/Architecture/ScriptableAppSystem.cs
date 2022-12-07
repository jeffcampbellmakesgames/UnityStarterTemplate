using UnityEngine;

namespace Game
{
	/// <summary>
	/// A <see cref="ScriptableObject"/> derived <see cref="IAppSystem"/>.
	/// </summary>
	public abstract class ScriptableAppSystem : ScriptableObject, IAppSystem
	{
		/// <inheritdoc />
		public abstract void OneTimeSetup();

		/// <inheritdoc />
		public abstract void OneTimeTeardown();

		/// <inheritdoc />
		public virtual bool IsSetupComplete()
		{
			// By default, all scriptable app systems return true for setup complete unless overriden.
			return true;
		}
	}
}
