using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    /// <summary>
    /// An <see cref="IAppSystem"/> that preloads several scenes additively as part of it's setup. It does not do any
    /// work post-setup to ensure those scenes remain loaded. This can be useful for preloading UI that persist through
    /// the entire app lifecycle, but are more manageable in separate scenes.
    /// </summary>
    public sealed class ScenePreloader : MonoBehaviour, IAppSystem
    {
        [Label("Scenes")]
        [SerializeField, Scene]
        private List<string> _sceneNames;

        private bool _areScenesLoaded;

        #region IAppSystem

        /// <inheritdoc />
        public void OneTimeSetup()
        {
            // If there are not any scenes to load, immediately quit setup.
            if (_sceneNames == null || _sceneNames.Count == 0)
            {
                _areScenesLoaded = true;
            }
            // Otherwise start loading each scene over time
            else
            {
                StartCoroutine(LoadScenesOverTime());
            }
        }

        /// <inheritdoc />
        public void OneTimeTeardown()
        {
            // No-Op
        }

        /// <inheritdoc />
        public bool IsSetupComplete() => _areScenesLoaded;

        #endregion

        /// <summary>
        /// Loads each scene in <see cref="_sceneNames"/> one at a time asynchronously.
        /// </summary>
        private IEnumerator LoadScenesOverTime()
        {
            // For each scene
            for (var i = 0; i < _sceneNames.Count; i++)
            {
                var sceneName = _sceneNames[i];
                var sceneAsset = SceneManager.GetSceneByName(sceneName);

                // Check to see if loaded already and if so skip.
                if (sceneAsset.isLoaded)
                {
                    continue;
                }

                // Otherwise load and continue on.
                var asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                yield return new WaitUntil(() => asyncOp.isDone);
            }

            _areScenesLoaded = true;
        }
    }
}
