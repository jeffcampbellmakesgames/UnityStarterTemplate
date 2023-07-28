using UnityEngine;

namespace Game
{
	/// <summary>
	/// Represents a scriptable game system.
	/// </summary>
	public abstract class ScriptableGameSystem : ScriptableObject, IGameSystem
	{
		protected bool _isSetup;
		
		/// <inheritdoc />
		public virtual void OneTimeSetup()
		{
			// No-Op
			_isSetup = true;
		}

		/// <inheritdoc />
		public virtual void OneTimeTeardown()
		{
			// No-Op
			_isSetup = false;
		}

		/// <inheritdoc />
		public virtual bool IsSetupComplete() => _isSetup;

		/// <inheritdoc />
		public virtual void Setup()
		{
			// No-op
			_isSetup = true;
		}

		/// <inheritdoc />
		public virtual void Teardown()
		{
			// No-op
		}
	}
}
