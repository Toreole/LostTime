using System.Collections;
using UnityEngine;

namespace LostTime.Core
{
    [RequireComponent(typeof(BoxCollider))]
    public class SceneLoadBehaviour : MonoBehaviour
    {
        [SerializeField]
        private string loadScene;
        [SerializeField]
        private string unloadScene;

        protected void LoadUnload()
        {
            if (string.IsNullOrEmpty(unloadScene))
                SceneManagement.UnloadScene(unloadScene);
            if (string.IsNullOrEmpty(loadScene))
                SceneManagement.LoadScene(loadScene);
        }

    }
}