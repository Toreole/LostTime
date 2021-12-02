using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostTime.Core;

namespace LostTime.Audio
{
    public class VoiceOverTrigger : MonoBehaviour
    {
        [SerializeField]
        private VoiceOver voiceOver;

        private void OnTriggerEnter(Collider other)
        {
            var x = other.GetComponent<Player>();
            if(x)
            {
                x.PlayVoiceOver(voiceOver);
            }
            gameObject.SetActive(false);
        }
    }
}