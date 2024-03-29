﻿using System.Collections;
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
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private GameObject whiteroomCeiling;
        [SerializeField]
        private GameObject creditsScreen;
        [SerializeField]
        private AudioClip doorSound;
        [SerializeField]
        private AudioClip dingSound;

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
            if (animator)
            {
                audioSource.PlayOneShot(doorSound);
                animator.SetTrigger(doorTrigger);
            }
        }

        //Called from button in scene
        public void OnElevatorButtonPress()
        {
            if(levelInProgress) //currently in level. check return-to-hub condition.
            {
                var level = LevelStartArea.Current;
                if(level) //check if the level exists.
                {
                    //check if the player has the required item to complete the level.
                    if(Player.Instance.HasItem(level.CompletionItem))
                    {
                        //go back to the hub.
                        levelInProgress = false;
                        StartCoroutine(DoReturnToHub());
                    }
                }
            }
            else if(canStartNextLevel) //in hub. check if player should be able to go to level.
            {
                StartNextLevel();
            }
            else //cant interact.
            {

            }
        }

        //Start the next level or finish the game, whichever it is depends on you state of the game.
        private void StartNextLevel()
        {
            Debug.Log("Start Next Level");
            levelInProgress = true;
            canStartNextLevel = false;
            if (levelIndex < levels.Length)
            {
                TransitionToScene(levels[levelIndex]);
            }
            else 
            {
                //ALL LEVELS COMPLETED; END GAME
                StartCoroutine(FinishGame());
            }
        }

        private void TransitionToScene(string sceneName)
        {
            //close elevator doors.
            TriggerDoors();
            whiteroomCeiling.SetActive(false);
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

        /// <summary>
        /// Moves the elevator (including player) from the hub to the new scene that was loaded.
        /// </summary>
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
            for (float t = 0; t < 4f; t += Time.deltaTime)
            {
                position.y = Mathf.Lerp(startY, targetY, t / 4f);
                transform.position = position;
                Physics.SyncTransforms();
                yield return null;
            }
            position.y = targetY;
            transform.position = position;
            Physics.SyncTransforms();

            player.SetParent(null);
            //open the doors of the elevator.
            yield return new WaitForSeconds(0.5f);
            audioSource.PlayOneShot(dingSound);
            TriggerDoors();
        }

        private IEnumerator DoReturnToHub()
        {
            //Close the elevator doors and wait for the animation to finish
            TriggerDoors();
            //stash the voiceover before anything else happens idk
            var vo = LevelStartArea.Current.OnLevelCompleteVoiceOver;
            yield return new WaitForSeconds(2f);
            //4 seconds is the minimum amount of time it takes to transition.
            float moveTime = 4f;
            //if the scene has a on complete voice over set, play it, and dont let the elevator finish until it has fully played.
            
            if (vo)
            {
                moveTime += vo.GetTotalDuration();
                Player.Instance.PlayVoiceOver(vo);
            }
            //unload the level.
            SceneManagement.UnloadScene(loadedScene);

            //make sure the player follows along.
            Transform player = Player.Instance.transform;
            player.SetParent(transform);

            //buffer the position, save the starting y for lerp
            Vector3 position = transform.position;
            float previousY = position.y;

            //do the actual moving.
            for(float t = 0; t < moveTime; t += Time.deltaTime)
            {
                position.y = Mathf.Lerp(previousY, startY, t / moveTime);
                transform.position = position;
                Physics.SyncTransforms();
                yield return null;
            }
            //finalize movement
            position.y = startY;
            transform.position = position;
            Physics.SyncTransforms();

            //unparent player, open the doors.
            player.SetParent(null);
            yield return new WaitForSeconds(0.5f);
            audioSource.PlayOneShot(dingSound);
            whiteroomCeiling.SetActive(true);
            TriggerDoors();
        }

        private IEnumerator FinishGame()
        {
            TriggerDoors();
            yield return new WaitForSeconds(2f);

            creditsScreen.SetActive(true);
            Player.Instance.CharacterController.enabled = false;
            Player.Instance.enabled = false;
        }

        //called when placing key items in the hub.
        public void CompleteLevel()
        {
            levelIndex++;
            AllowLevelProgress();
        }
    }
}