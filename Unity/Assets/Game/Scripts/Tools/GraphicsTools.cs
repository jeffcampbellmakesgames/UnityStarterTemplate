using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	/// <summary>
	/// Static helper methods for Graphics.
	/// </summary>
	public sealed class GraphicsTools
	{
		/// <summary>
		/// All available <see cref="FramePerSecondMode"/> mode values for this platform.
		/// </summary>
		public static IReadOnlyList<FramePerSecondMode> AvailableFramePerSecondModes
		{
			get
			{
				if (_availableFramePerSecondModes == null)
				{
					var list = new List<FramePerSecondMode>
					{
						FramePerSecondMode.Thirty,
						FramePerSecondMode.Sixty
					};

					#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
					list.Add(FramePerSecondMode.Uncapped);
					#endif

					_availableFramePerSecondModes = list;
				}

				return _availableFramePerSecondModes;
			}
		}

		/// <summary>
		/// All available <see cref="FullScreenMode"/> values for this platform.
		/// </summary>
		public static IReadOnlyList<FullScreenMode> AvailableFullScreenModes
		{
			get
			{
				if (_availableFullScreenModes == null)
				{
					var list = new List<FullScreenMode>
					{
						FullScreenMode.Windowed,
						FullScreenMode.FullScreenWindow
					};

					// These full screen modes are only available on windows OR mac.
					#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
					list.Add(FullScreenMode.ExclusiveFullScreen);
					#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
					list.Add(FullScreenMode.MaximizedWindow);
					#endif

					_availableFullScreenModes = list;
				}

				return _availableFullScreenModes;
			}
		}

		private static IReadOnlyList<FullScreenMode> _availableFullScreenModes;
		private static IReadOnlyList<FramePerSecondMode> _availableFramePerSecondModes;

		/// <summary>
		/// Returns the default <see cref="FramePerSecondMode"/> for this platform.
		/// </summary>
		public static FramePerSecondMode GetDefaultFramePerSecondMode()
		{
			#if UNITY_ANDROID || UNITY_IOS
			return FramePerSecondMode.Sixty;
			#else
			return FramePerSecondMode.Sixty;
			#endif
		}

		/// <summary>
		/// Populates <paramref name="resolutions"/> with all available resolutions.
		/// </summary>
		public static void GetAvailableResolutionsNonAlloc(ref List<Resolution> resolutions)
		{
			resolutions.Clear();
			resolutions.AddRange(Screen.resolutions);

			var currentResolution = GetCurrentResolution();
			if (!resolutions.Contains(currentResolution))
			{
				resolutions.Add(currentResolution);
			}
		}

		/// <summary>
		/// Retrieves the current <see cref="Resolution"/>, regardless of the current <see cref="FullScreenMode"/>.
		/// </summary>
		public static Resolution GetCurrentResolution()
		{
			// If in windowed mode, the screen resolution from Unity is reported incorrectly.
			// Instead create a resolution based on the reported screen size.
			if (Screen.fullScreenMode == FullScreenMode.Windowed ||
			    Screen.fullScreenMode == FullScreenMode.MaximizedWindow)
			{
				return new Resolution
				{
					width = Screen.width,
					height = Screen.height,
					refreshRate = Screen.currentResolution.refreshRate
				};
			}

			return Screen.currentResolution;
		}
	}
}
