using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace LostTime.Audio
{
    public class RandomVoiceOverTrigger : MonoBehaviour
    {
        [SerializeField]
        VoiceOver[] voiceOvers;
        [Tag, SerializeField]
        private string playerTag = "Player";

        VoiceOver lastVoiceOver;
        List<VoiceOver> voiceOverList;

        private void Start()
        {
            voiceOverList = new List<VoiceOver>(voiceOvers);
        }

        private void OnTriggerEnter(Collider other)
        {
            //Play a random voiceover, but not the same one multiple times in a row.
            if (other.CompareTag(playerTag))
            {
                var buffer = voiceOverList[Random.Range(0, voiceOverList.Count)];
                voiceOverList.Remove(buffer);
                if (lastVoiceOver)
                    voiceOverList.Add(lastVoiceOver);
                lastVoiceOver = buffer;
                LostTime.Core.Player.Instance.PlayVoiceOver(buffer);

                gameObject.SetActive(false);
            }
        }
    }
}