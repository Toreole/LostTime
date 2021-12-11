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
        private string[] levels;

        readonly int doorTrigger = Animator.StringToHash("doorOpen");
        private string loadedScene;
        private int levelIndex = 0;
        private bool levelInProgress = false;
        private bool canStartNextLevel = true;

        private float startY = 0;

        private void Start()
        {
            Instance = this;
            startY = transform.position.y;
        }

        public void TriggerDoors()
        {
            if(animator)
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
            //StartCoroutine(DoSceneTeleport());
            StartCoroutine(DoMoveElevator());
        }

        public void AllowLevelProgress()
        {
            canStartNextLevel = true;
        }

        private IEnumerator DoMoveElevator()
        {
            yield return new WaitForSeconds(2f);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(loadedScene));
            UnityEngine.LightProbes.Tetrahedralize();

            var startArea = LevelStartArea.Current;
            float targetY = startArea.transform.position.y;
            Vector3 position = transform.position;
            Transform player = Player.Instance.transform;
            player.SetParent(this.transform);

            //Lerp it!. is probably incredibly fast, but doesnt matter to the player.
            for (float t = 0; t < 2f; t += Time.deltaTime)
            {
                position.y = Mathf.Lerp(startY, targetY, t / 2f);
                transform.position = position;
                Physics.SyncTransforms();
                yield return null;
            }
            position.y = targetY;
            transform.position = position;
            Physics.SyncTransforms();

            player.SetParent(null);
        }

        private IEnumerator DoSceneTeleport()
        {
            yield return new WaitForSeconds(2f);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(loadedScene));
            UnityEngine.LightProbes.Tetrahedralize();
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
    }
}