using UnityEngine;

namespace Game
{
	/// <summary>
	/// Represents a in-scene game system.
	/// </summary>
	public abstract class MonoBehaviorGameSystem : MonoBehaviour, IGameSystem
	{
		/// <inheritdoc />
		public virtual void OneTimeSetup()
		{
			// No-op
		}

		/// <inheritdoc />
		public virtual void OneTimeTeardown()
		{
			// No-op
		}

		/// <inheritdoc />
		public virtual bool IsSetupComplete()
		{
			// No-op
			return true;
		}

		/// <inheritdoc />
		public virtual void Setup()
		{
			// No-op
		}

		/// <inheritdoc />
		public virtual void Teardown()
		{
			// No-op
		}

		/// <inheritdoc />
		public virtual void ExecutePerFrame()
		{
			// No-op
		}
	}
}
