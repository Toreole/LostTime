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

        //this behaves more like a stack innit
        private Queue<VoiceOver> voiceOverQueue = new Queue<VoiceOver>(4); //not expecting more than 4 voiceovers to be queued at any time.

        private bool isPlaying = false;
        private bool isPaused = false;
        private float localTime = 0;
        private float nextLineTime;
        private int lineIndex = 0;

        private VoiceOver currentVoiceOver;

        public bool IsPlaying
        {
            get => isPlaying; 
            private set
            {
                if(isPlaying is true && value is false)
                {
                    BGMHandler.FreeBGM();
                }
                if(isPlaying is false && value is true)
                {
                    BGMHandler.SuppressBGM();
                }
                isPlaying = value;
            }
        }

        private void Awake()
        {
            PauseMenu.OnMenuOpened += this.Pause;
            PauseMenu.OnMenuClosed += this.Resume;
        }

        private void OnDestroy()
        {
            PauseMenu.OnMenuOpened -= this.Pause;
            PauseMenu.OnMenuClosed -= this.Resume;
        }

        /// <summary>
        /// Queues up / Plays a voice over. safe to use at all times.
        /// </summary>
        public void QueueVoiceOver(VoiceOver vo)
        {
            if(!IsPlaying)
                Play(vo);
            else
                voiceOverQueue.Enqueue(vo);
        }

        /// <summary>
        /// Start to play audio and make the subtitles appear please, thanks.
        /// </summary>
        private void Play(VoiceOver vo)
        {
            lineIndex = 0;
            IsPlaying = true;
            currentVoiceOver = vo;
            audioSource.clip = vo.AudioClip;
            var activeLine = vo.Transcription[0];
            textElement.text = activeLine.line;
            nextLineTime = localTime + activeLine.duration;
            if(isPaused is false)
                audioSource.Play();
        }

        //WIP: turn DoPlay coroutine into standard Update method.
        private void Update()
        {
            //if its not playing anything, or is just paused, do nothing.
            if (isPaused || IsPlaying is false)
                return;
            //step localTime.
            localTime += Time.deltaTime;

            //check whether to advance to the next line of subtitles.
            if(localTime > nextLineTime)
            {
                //increase lineIndex.
                lineIndex++;
                //check for out of array bounds.
                //make sure that the audioClip on the audioSource has stopped playing aswell:
                if (lineIndex >= currentVoiceOver.Transcription.Length && audioSource.isPlaying is false)
                {
                    //try to advance to the next voiceOver in the queue.
                    if (voiceOverQueue.Count > 0)
                    {
                        Play(voiceOverQueue.Dequeue());
                    }
                    else //no more queued. stop playing. hide subtitles.
                    {
                        textElement.text = "";
                        IsPlaying = false;
                    }
                }
                //lineIndex still within bounds -> set up next line
                else if(lineIndex < currentVoiceOver.Transcription.Length)
                {
                    var nextLine = currentVoiceOver.Transcription[lineIndex];
                    textElement.text = nextLine.line;
                    nextLineTime = localTime + nextLine.duration;
                }
            }
        }

        public void Pause()
        {
            isPaused = true;
            audioSource.Pause();
        }

        public void Resume()
        {
            isPaused = false;
            if(IsPlaying) //only play audio if we should play something.
                audioSource.Play();
        }

    }
}