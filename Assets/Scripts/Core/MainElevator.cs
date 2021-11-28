using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LostTime.Core
{
    public class MainElevator : MonoBehaviour
    {
        public static MainElevator Instance { get; private set; }

        [SerializeField]
        private Animator animator;
        [SerializeField]
        private GameObject directionalLight;

        readonly int doorTrigger = Animator.StringToHash("doorOpen");
        private string loadedScene;

        private void Start()
        {
            Instance = this;
        }

        public void TriggerDoors()
        {
            animator.SetTrigger(doorTrigger);
        }

        public void TransitionToScene(string sceneName)
        {
            TriggerDoors();
            loadedScene = sceneName;
            SceneManagement.LoadScene(sceneName, SceneTeleport);
        }

        private void SceneTeleport()
        {
            StartCoroutine(DoSceneTeleport());
        }

        private IEnumerator DoSceneTeleport()
        {
            yield return new WaitForSeconds(2f);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(loadedScene));
            directionalLight.SetActive(false);
            //disgusting hack but ok
            LevelStartArea.Current.MovePlayerToFrom(LevelStartArea.Current.transform, Player.Instance.transform, this.transform);
            yield return new WaitForSeconds(2f);
            LevelStartArea.Current.TriggerDoors();
        }

        public void ReEnableLight()
        {
            directionalLight.SetActive(true);
        }

    }
}