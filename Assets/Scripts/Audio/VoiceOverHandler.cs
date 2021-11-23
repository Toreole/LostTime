using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LostTime.UI;

namespace LostTime.Audio
{
    public class VoiceOverHandler : MonoBehaviour
    {
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private TextMeshProUGUI textElement;

        private VoiceOver[] voiceOverQueue = new VoiceOver[4]; //not expecting more than 4 voiceovers to be queued at any time.
        private int queueLength = 0;

        private bool isPlaying = false;

        public bool IsPlaying => isPlaying;

        private void Awake()
        {
            PauseMenu.OnMenuOpened += OnGamePaused;
            PauseMenu.OnMenuClosed += OnGameResumed;
        }

        private void OnDestroy()
        {
            PauseMenu.OnMenuOpened -= OnGamePaused;
            PauseMenu.OnMenuClosed -= OnGameResumed;
        }

        private void OnGamePaused()
        {
            audioSource.Pause();
        }

        private void OnGameResumed()
        {
            audioSource.Play();
        }

        /// <summary>
        /// Queues up / Plays a voice over. safe to use at all times.
        /// </summary>
        public void QueueVoiceOver(VoiceOver vo)
        {
            voiceOverQueue[queueLength] = vo;
            queueLength++;
            if(!isPlaying)
                Play();
        }

        private void Play()
        {
            isPlaying = true;
            StartCoroutine(DoPlay());
        }

        //NOTE: Since Coroutines break when the GameObject is disabled (which it would be by the Player, via the HUD)
        //this gameobject has to ensure that its always active in the hierarchy.
        //either move this component onto the Camera (always active), or do a massive effort to scale time to 0, and have UI use unscaled for everything.
        /// <summary>
        /// Starts the audioSource, but more importantly displays all the subtitles.
        /// Uses up all VoiceOvers in the "queue".
        /// </summary>
        private IEnumerator DoPlay()
        {
            BGMHandler.SuppressBGM();
            while(queueLength > 0)
            {
                VoiceOver vo = voiceOverQueue[queueLength-1];
                queueLength--;
                audioSource.clip = vo.AudioClip;
                audioSource.Play();
                foreach(var transcript in vo.Transcription)
                {
                    textElement.text = transcript.line;
                    //uses scaled time => is automatically "stopped" by the PauseMenu when it sets the timeScale to 0.
                    yield return new WaitForSeconds(transcript.duration); 
                }
            }
            BGMHandler.FreeBGM();
            //everything has been "dequeued" / handled.
            textElement.text = "";
            isPlaying = false;
        }

    }
}