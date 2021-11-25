using System.Collections;
using UnityEngine;

namespace LostTime.Audio
{
    public class VoiceOverDebug : MonoBehaviour
    {
        public VoiceOver voiceOverToPlay;

        private void Start()
        {
            GetComponent<VoiceOverHandler>().QueueVoiceOver(voiceOverToPlay);
        }
    }
}