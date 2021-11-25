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

    }

    [System.Serializable]
    public class TranscriptionLine
    {
        public string line;
        public float duration;
    }
}