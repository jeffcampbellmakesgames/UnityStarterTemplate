using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game
{
	/// <summary>
	/// A local object pool for objects of type <typeparamref name="TComponent"/>.
	/// </summary>
	/// <typeparam name="TComponent">The type of component being pooled.</typeparam>
	public class ComponentPool<TComponent> where TComponent : Component
	{
		/// <summary>
		/// The total number of instances spawned in this pool (inactive or active).
		/// </summary>
		public int Count => ActiveCount + InactiveCount;

		/// <summary>
		/// The total number of active instances in this pool.
		/// </summary>
		public int ActiveCount => _activeList.Count;

		/// <summary>
		/// The total number of inactive instances in this pool.
		/// </summary>
		public int InactiveCount => _inactiveList.Count;

		private readonly LinkedList<TComponent> _inactiveList;
		private readonly LinkedList<TComponent> _activeList;
		private readonly int _maxSize;
		private GameObject _owner;
		private readonly TComponent _source;

		public ComponentPool(
			GameObject owner,
			TComponent source,
			int defaultCapacity = 10,
			int maxSize = 10000)
		{
			_owner = owner;
			_source = source;

			if (maxSize <= 0)
			{
				throw new ArgumentException("Max Size must be greater than 0", nameof(maxSize));
			}

			_maxSize = maxSize;
			_inactiveList = new LinkedList<TComponent>();
			_activeList = new LinkedList<TComponent>();

			for (var i = 0; i < defaultCapacity; i++)
			{
				var instance = Create();
				Recycle(instance);
			}
		}

		/// <summary>
		/// Gets or creates an instance of <typeparamref name="TComponent"/> from this pool.
		/// </summary>
		public TComponent Spawn()
		{
			TComponent instance;
			if (_inactiveList.Count == 0)
			{
				instance = Create();
			}
			else
			{
				instance = _inactiveList.First.Value;
				_inactiveList.RemoveFirst();
			}

			_activeList.AddLast(instance);

			OnSpawn(instance);

			return instance;
		}

		/// <summary>
		/// Recycles an instance of <typeparamref name="TComponent"/> back to the pool.
		/// </summary>
		public void Recycle(TComponent instance)
		{
			if (_inactiveList.Count > 0 && _inactiveList.Contains(instance))
			{
				throw new InvalidOperationException(
					"Trying to release an object that has already been released to the pool.");
			}

			_activeList.Remove(instance);

			OnRecycle(instance);

			if (InactiveCount < _maxSize)
			{
				_inactiveList.AddLast(instance);
			}
			else
			{
				Destroy(instance);
			}
		}

		/// <summary>
		/// Creates an internal instance of <typeparamref name="TComponent"/> for use.
		/// </summary>
		protected virtual TComponent Create() => Object.Instantiate(_source);

		/// <summary>
		/// Handles destroying instances of <typeparamref name="TComponent"/> if the max capacity of the pool is reached.
		/// </summary>
		protected virtual void Destroy(TComponent instance)
		{
			Object.Destroy(instance);
		}

		/// <summary>
		/// Handles configuring instances of <typeparamref name="TComponent"/> before made available for consumers.
		/// </summary>
		protected virtual void OnSpawn(TComponent instance)
		{
			// Un-parent the spawned instance from the owner of the pool.
			instance.transform.SetParent(null);
			instance.gameObject.SetActive(true);
		}

		/// <summary>
		/// Handles configuring instances of <typeparamref name="TComponent"/> after they are arecycled back to this pool.
		/// </summary>
		protected virtual void OnRecycle(TComponent instance)
		{
			// Re-parent the recycled instance to the owner of the pool.
			instance.transform.SetParent(_owner.transform);
			instance.gameObject.SetActive(false);
		}
	}
}
