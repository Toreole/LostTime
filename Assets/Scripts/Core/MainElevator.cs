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
        [SerializeField]
        private string[] levels;

        readonly int doorTrigger = Animator.StringToHash("doorOpen");
        private string loadedScene;
        private int levelIndex = 0;
        private bool levelInProgress = false;
        private bool canStartNextLevel = true;

        private void Start()
        {
            Instance = this;
        }

        public void TriggerDoors()
        {
            animator.SetTrigger(doorTrigger);
        }

        public void StartNextLevel()
        {
            Debug.Log("Start Next Level");
            if (levelInProgress || !canStartNextLevel)
                return;
            levelInProgress = true;
            canStartNextLevel = false;
            if (levelIndex < levels.Length)
            {
                TransitionToScene(levels[levelIndex]);
            }
            else 
            {
                //ALL LEVELS COMPLETED; END GAME
            }
        }

        private void TransitionToScene(string sceneName)
        {
            TriggerDoors();
            loadedScene = sceneName;
            SceneManagement.LoadScene(sceneName, SceneTeleport);
        }

        private void SceneTeleport()
        {
            StartCoroutine(DoSceneTeleport());
        }

        public void AllowLevelProgress()
        {
            canStartNextLevel = true;
        }

        private IEnumerator DoSceneTeleport()
        {
            yield return new WaitForSeconds(2f);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(loadedScene));
            directionalLight.SetActive(false);
            //disgusting hack but ok
            var levelStart = LevelStartArea.Current;
            levelStart.MovePlayerToFrom(levelStart.transform, Player.Instance.transform, this.transform);
            yield return new WaitForSeconds(2f);
            levelStart.TriggerDoors();
        }

        public void CompleteLevel()
        {
            levelInProgress = false;
            levelIndex++;
        }

        public void ReEnableLight()
        {
            directionalLight.SetActive(true);
        }

    }
}