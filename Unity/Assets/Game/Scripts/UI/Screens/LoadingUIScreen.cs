using System.Collections;
using JCMG.Slate;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	/// <summary>
	/// Represents a loading screen.
	/// </summary>
	public sealed class LoadingUIScreen : UIScreen
	{
		/// <summary>
		/// Returns true if this screen is currently animating, otherwise false.
		/// </summary>
		public bool IsAnimating => _animationCoroutine != null;

		[SerializeField, Required]
		private Image _fadeImage;

		private Coroutine _animationCoroutine;

		/// <inheritdoc />
		public override void Show(bool immediate = false)
		{
			base.Show(immediate);

			if (immediate)
			{
				FadeInImmediate();
			}
			else
			{

				if (_animationCoroutine != null)
				{
					StopCoroutine(_animationCoroutine);
					_animationCoroutine = null;
				}

				_animationCoroutine = StartCoroutine(FadeIn());
			}
		}

		/// <inheritdoc />
		public override void Hide(bool immediate = false)
		{
			if (immediate)
			{
				FadeOutImmediate();
			}
			else
			{

				if (_animationCoroutine != null)
				{
					StopCoroutine(_animationCoroutine);
					_animationCoroutine = null;
				}

				_animationCoroutine = StartCoroutine(FadeOut());
			}
		}

		/// <summary>
		/// Fades out over <paramref name="duration"/> in seconds.
		/// </summary>
		private IEnumerator FadeOut(float duration = 1f)
		{
			_fadeImage.raycastTarget = true;

			float elapsedTime = 0;
			while (elapsedTime < duration)
			{
				_fadeImage.color = Color.Lerp(Color.black, Color.clear, elapsedTime / duration);
				elapsedTime += Time.deltaTime;
				yield return null;
			}

			FadeOutImmediate();
		}

		/// <summary>
		/// Fades in over <paramref name="duration"/> in seconds.
		/// </summary>
		private IEnumerator FadeIn(float duration = 1f)
		{
			_fadeImage.raycastTarget = true;

			float elapsedTime = 0;
			while (elapsedTime < duration)
			{
				_fadeImage.color = Color.Lerp(Color.clear, Color.black, elapsedTime / duration);
				elapsedTime += Time.deltaTime;
				yield return null;
			}

			FadeInImmediate();
		}

		/// <summary>
		/// Sets the fade out state immediately.
		/// </summary>
		private void FadeOutImmediate(bool blocksRaycast = false)
		{
			_fadeImage.color = Color.clear;
			_fadeImage.raycastTarget = blocksRaycast;
			_animationCoroutine = null;

			base.Hide();
		}

		/// <summary>
		/// Sets the fade in state immediately.
		/// </summary>
		private void FadeInImmediate(bool blocksRaycast = false)
		{
			_fadeImage.color = Color.black;
			_fadeImage.raycastTarget = blocksRaycast;
			_animationCoroutine = null;
		}
	}
}
