using System;
using System.Collections;
using System.Collections.Generic;
using JCMG.Utility;
using NaughtyAttributes;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Game
{
    /// <summary>
    /// The entry-point and app-wide control singleton managing <see cref="IAppSystem"/> instances.
    /// </summary>
    public sealed class AppControl : Singleton<AppControl>
    {
        [Serializable]
        private class AppSystemRef
        {
            /// <summary>
            /// Returns true if this app system ref is using a Monobehavior source or false if it's a ScriptableObject
            /// source.
            /// </summary>
            public bool IsUsingMonobehaviorSource => _sourceType == SourceType.Monobehavior;

            public bool DoesMonobehaviorImplementIAppSystem => //!IsUsingMonobehaviorSource ||
                                                               _appSystemBehavior is IAppSystem;

            private enum SourceType
            {
                Monobehavior = 0,
                ScriptableObject = 1
            }

            [SerializeField]
            private SourceType _sourceType;

            [AllowNesting, ShowIf("IsUsingMonobehaviorSource")]
            [SerializeField]
            private MonoBehaviour _appSystemBehavior;

            [AllowNesting, HideIf("IsUsingMonobehaviorSource")]
            [SerializeField]
            private ScriptableAppSystem _appSystemScriptableObject;

            public IAppSystem GetAppSystem()
            {
                IAppSystem result = null;
                if (IsUsingMonobehaviorSource)
                {
                    Assert.IsNotNull(_appSystemBehavior);
                    Assert.IsTrue(
                        _appSystemBehavior is IAppSystem,
                        $"{_appSystemBehavior.name} does not implement IAppSystem.");

                    result = _appSystemBehavior as IAppSystem;
                }
                else
                {
                    Assert.IsNotNull(_appSystemScriptableObject);
                    result = _appSystemScriptableObject;
                }

                return result;
            }
        }

        /// <summary>
        /// Invoked when setup has completed.
        /// </summary>
        public event Action SetupCompleted;

        [Label("App Systems")]
        [SerializeField]
        private List<AppSystemRef> _appSystemRefs;

        [BoxGroup(RuntimeConstants.EVENTS)]
        [SerializeField, Required]
        private GameEvent _setupCompletedEvent;

        [BoxGroup(RuntimeConstants.SETTINGS)]
        [SerializeField, Scene]
        private string _lobbySceneName;

        private List<IAppSystem> _appSystems;

        protected override void Awake()
        {
            base.Awake();

            _appSystems = new List<IAppSystem>();
            for (var i = 0; i < _appSystemRefs.Count; i++)
            {
                var appSystem = _appSystemRefs[i].GetAppSystem();
                _appSystems.Add(appSystem);
            }
        }

        private void Start()
        {
            // Setup all app systems.
            for (var i = 0; i < _appSystems.Count; i++)
            {
                _appSystems[i].OneTimeSetup();
            }

            // If already setup in this single frame for all app systems then immediately continue on,
            // otherwise create a coroutine to check progress over time.
            if (AreAllAppSystemsSetup())
            {
                SetupCompleted?.Invoke();
                _setupCompletedEvent.Raise();
            }
            else
            {
                StartCoroutine(CheckSetupProgressOverTime());
            }
        }

        private void OnDestroy()
        {
            // Tear down all app systems
            for (var i = 0; i < _appSystems.Count; i++)
            {
                _appSystems[i].OneTimeTeardown();
            }
        }

        /// <summary>
        /// Check for all app systems to be setup over time, then once complete invoke <see cref="SetupCompleted"/>.
        /// </summary>
        private IEnumerator CheckSetupProgressOverTime()
        {
            // Wait for all app systems to be setup
            yield return new WaitWhile(() => !AreAllAppSystemsSetup());

            // Load the lobby scene
            var asyncOp = SceneManager.LoadSceneAsync(_lobbySceneName);
            yield return new WaitUntil(() => asyncOp.isDone);

            // Signal that the app setup has completed.
            SetupCompleted?.Invoke();
            _setupCompletedEvent.Raise();
        }

        /// <summary>
        /// Returns true if all <see cref="IAppSystem"/> instances have completed setup, otherwise false.
        /// </summary>
        private bool AreAllAppSystemsSetup()
        {
            Assert.IsNotNull(_appSystems);

            var result = true;
            for (var i = 0; i < _appSystems.Count; i++)
            {
                result &= _appSystems[i].IsSetupComplete();

                if (!result)
                {
                    break;
                }
            }
            return result;
        }
    }
}
