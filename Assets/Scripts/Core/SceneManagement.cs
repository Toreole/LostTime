using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LostTime.Core
{
    //Add this component to some disconnected object in the main menu.
    public class SceneManagement : MonoBehaviour
    {
        private static SceneManagement instance;

        [SerializeField]
        private CanvasGroup loadScreen;

        private void Start()
        {
            if (instance == null)
                instance = this;
            else return;
            DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// Loads a single scene. (Yes, this unloads all currently active scenes)
        /// Also includes a loading screen.
        /// </summary>
        public static void GotoScene(string sceneName)
        {
            instance._GotoScene(sceneName);
        }
        private void _GotoScene(string sceneName)
        {
            StartCoroutine(Load());
            IEnumerator Load()
            {
                for(float t = 0; t < 0.5f; t += Time.deltaTime)
                {
                    loadScreen.alpha = t / 0.5f;
                    yield return null;
                }

                AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
                operation.allowSceneActivation = false;

                yield return new WaitUntil(() => operation.progress >= 0.9f);
                operation.allowSceneActivation = true;

                for (float t = 0; t < 1f; t += Time.deltaTime)
                {
                    loadScreen.alpha = 1.0f - t;
                    yield return null;
                }
                loadScreen.alpha = 0;
                loadScreen.blocksRaycasts = false;
                loadScreen.interactable = false;
            }
        }

        /// <summary>
        /// Loads a scene in the background. 
        /// </summary>
        public static void LoadScene(string sceneName)
            => SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        public static void LoadScene(string sceneName, System.Action onLoadComplete)
        {
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            op.completed += (x) => onLoadComplete();
        }

        /// <summary>
        /// Asynchronously unloads the target scene.
        /// </summary>
        public static void UnloadScene(string sceneName)
            => SceneManager.UnloadSceneAsync(sceneName);

    }
}