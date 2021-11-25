using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostTime.Audio
{
    public class AnimatedSoundHandler : MonoBehaviour
    {
        [SerializeField]
        private AudioSource audioSource;

        public void PlaySound()
        {
            audioSource.Play();
        }
    }
}