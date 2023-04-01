using System;
using System.Collections.Generic;
using JCMG.Slate;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
	/// <summary>
	/// An abstract list UI capable of showing a general purpose list of <typeparam name="TSetDataType"></typeparam> as
	/// a series of <see cref="TUIDisplayType"/> instances.
	/// </summary>
	public abstract class UIListBase<TSetDataType, TUIDisplayType> : MonoBehaviour
		where TUIDisplayType : UIDisplayBase<TSetDataType>
	{
		/// <summary>
		/// Lazy-load property for the item pool.
		/// </summary>
		protected ComponentPool<TUIDisplayType> ListItemPool
		{
			get
			{
				if (_displayPool == null)
				{
					_displayPool = new ComponentPool<TUIDisplayType>(
						gameObject,
						_uiDisplay);
				}

				return _displayPool;
			}
		}

		/// <summary>
		/// Lazy-loaded list of <typeparamref name="TUIDisplayType"/>.
		/// </summary>
		private List<TUIDisplayType> DisplayBases
		{
			get
			{
				if (_displayBases == null)
				{
					_displayBases = new List<TUIDisplayType>();
				}

				return _displayBases;
			}
		}

		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		protected GameObject _listItemParent;

		[BoxGroup(RuntimeConstants.UI_REFS)]
		[SerializeField, Required]
		private TUIDisplayType _uiDisplay;

		[SerializeField, Required]
		protected ComponentPool<TUIDisplayType> _displayPool;

		protected List<TUIDisplayType> _displayBases;
		protected EventSystem _eventSystem;
		private bool _isApplicationQuitting;

		protected virtual void Awake()
		{
			_eventSystem = EventSystem.current;
			_displayBases = new List<TUIDisplayType>();
		}

		protected virtual void OnDestroy()
		{
			if (!_isApplicationQuitting)
			{
				Clear();
			}
		}

		private void OnApplicationQuit()
		{
			_isApplicationQuitting = true;
		}

		/// <summary>
		/// Sets a collection of <see cref="TSetDataType"/> values for this list to display.
		/// </summary>
		public virtual void Set(IReadOnlyList<TSetDataType> values)
		{
			Clear();

			for (var i = 0; i < values.Count; ++i)
			{
				var displayBase = ListItemPool.Spawn();
				displayBase.transform.SetParent(_listItemParent.transform, false);
				displayBase.transform.localScale = Vector3.one;
				displayBase.Set(values[i]);

				DisplayBases.Add(displayBase);
			}
		}

		/// <summary>
		/// Sets a collection of <see cref="TSetDataType"/> values for this list to display.
		/// </summary>
		/// <param name="onClickHandler">A click-handler that will be used if
		/// <typeparamref name="TUIDisplayType"/> implements <see cref="IClickableUIDisplay{TDataType}"/></param>.
		public virtual void Set(IReadOnlyList<TSetDataType> values, Action<TSetDataType> onClickHandler)
		{
			Clear();

			for (var i = 0; i < values.Count; ++i)
			{
				var displayBase = ListItemPool.Spawn();
				displayBase.transform.SetParent(_listItemParent.transform, false);
				displayBase.transform.localScale = Vector3.one;
				displayBase.Set(values[i]);

				if (displayBase is IClickableUIDisplay<TSetDataType> clickableUIDisplay)
				{
					clickableUIDisplay.SetOnClick(onClickHandler);
				}

				DisplayBases.Add(displayBase);
			}
		}

		/// <summary>
		/// Updates all display items in the list
		/// </summary>
		public virtual void UpdateDisplay()
		{
			for (var i = 0; i < DisplayBases.Count; i++)
			{
				DisplayBases[i].UpdateDisplay();
			}
		}

		/// <summary>
		/// If there are any items in the list, sets the first one as the selected item for the event system for
		/// controller/keyboard navigation support.
		/// </summary>
		public virtual void SetFirstAsSelected()
		{
			if (DisplayBases.Count == 0)
			{
				return;
			}

			_eventSystem.SetSelectedGameObject(DisplayBases[0].gameObject);
		}

		/// <summary>
		/// Clears and recycles all items in the list.
		/// </summary>
		public virtual void Clear()
		{
			for (var i = 0; i < DisplayBases.Count; i++)
			{
				ListItemPool.Recycle(DisplayBases[i]);
			}

			DisplayBases.Clear();
		}
	}

	/// <summary>
	/// An abstract list UI capable of showing a general purpose list of <typeparamref name="TRawDataType"></typeparamref> as a
	/// series of <see cref="TUIDisplayType"/> instances where <typeparamref name="TRawDataType"/> values are translated to
	/// <typeparamref name="TSetDataType"/> values.
	/// </summary>
	public abstract class UIListBase<TRawDataType, TSetDataType, TUIDisplayType> : MonoBehaviour
		where TUIDisplayType : UIDisplayBase<TSetDataType>
	{
		/// <summary>
		/// Lazy-load property for the item pool.
		/// </summary>
		protected ComponentPool<TUIDisplayType> ListItemPool
		{
			get
			{
				if (_displayPool == null)
				{
					_displayPool = GetComponent<ComponentPool<TUIDisplayType>>();
				}

				return _displayPool;
			}
		}

		[SerializeField, Required]
		protected GameObject _listItemParent;

		[SerializeField, Required]
		private ComponentPool<TUIDisplayType> _displayPool;

		protected List<TUIDisplayType> _displayBases;
		protected EventSystem _eventSystem;

		protected virtual void OnEnable()
		{
			if (_displayBases == null)
			{
				_displayBases = new List<TUIDisplayType>();
			}
		}

		protected virtual void Awake()
		{
			_eventSystem = EventSystem.current;
		}

		protected virtual void OnDestroy()
		{
			Clear();
		}

		/// <summary>
		/// Sets a collection of <see cref="TSetDataType"/> values for this list to display.
		/// </summary>
		/// <param name="values"></param>
		public virtual void Set(IReadOnlyList<TRawDataType> values)
		{
			for (var i = 0; i < values.Count; ++i)
			{
				var displayBase = ListItemPool.Spawn();
				displayBase.transform.SetParent(_listItemParent.transform, false);
				displayBase.transform.localScale = Vector3.one;

				SetDataOnDisplay(values[i], displayBase);

				_displayBases.Add(displayBase);
			}
		}

		/// <summary>
		/// Sets <typeparamref name="TSetDataType"/> data onto an appropriate <see cref="TUIDisplayType"/> based on a
		/// <typeparamref name="TRawDataType"/> value.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="displayBase"></param>
		protected abstract void SetDataOnDisplay(TRawDataType value, TUIDisplayType displayBase);

		/// <summary>
		/// Updates all display items in the list
		/// </summary>
		public virtual void UpdateDisplay()
		{
			for (var i = 0; i < _displayBases.Count; i++)
			{
				_displayBases[i].UpdateDisplay();
			}
		}

		/// <summary>
		/// If there are any items in the list, sets the first one as the selected item for the event system for
		/// controller/keyboard navigation support.
		/// </summary>
		public virtual void SetFirstAsSelected()
		{
			if (_displayBases.Count == 0)
			{
				return;
			}

			_eventSystem.SetSelectedGameObject(_displayBases[0].gameObject);
		}

		/// <summary>
		/// Clears and recycles all items in the list.
		/// </summary>
		public virtual void Clear()
		{
			for (var i = 0; i < _displayBases.Count; i++)
			{
				ListItemPool.Recycle(_displayBases[i]);
			}

			_displayBases.Clear();
		}
	}
}
