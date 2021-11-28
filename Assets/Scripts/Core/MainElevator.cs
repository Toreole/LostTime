using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LostTime.Core
{
    public class MainElevator : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private GameObject directionalLight;

        readonly int doorTrigger = Animator.StringToHash("doorOpen");
        private string loadedScene;

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
            yield return new WaitForSecondsRealtime(1f);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(loadedScene));
            directionalLight.SetActive(false);
            //disgusting hack but ok
            LevelStartArea.Current.MovePlayerHere(FindObjectOfType<Player>().transform, transform);
            yield return new WaitForSecondsRealtime(1f);
            LevelStartArea.Current.OpenElevatorDoors();
        }

    }
}