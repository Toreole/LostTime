using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostTime.Audio
{
    [CreateAssetMenu(menuName = "CustomAsset/VoiceOver")]
    public class VoiceOver : ScriptableObject
    {
        [SerializeField]
        private AudioClip audioClip;
        [SerializeField]
        private TranscriptionLine[] transcription;

        public AudioClip AudioClip => audioClip;
        public TranscriptionLine[] Transcription => transcription;

        public float GetTotalDuration()
        {
            //the duration based on the sum of durations of the transcription.
            float duration = 0;
            for (int i = 0; i < transcription.Length; i++)
                duration += transcription[i].duration;

            if (audioClip == null)
                return duration;
            return Mathf.Max(duration, audioClip.length);
        }

    }

    [System.Serializable]
    public class TranscriptionLine
    {
        public string line;
        public float duration;
    }
}