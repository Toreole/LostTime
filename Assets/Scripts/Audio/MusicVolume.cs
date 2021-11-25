using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostTime.Audio
{
    [RequireComponent(typeof(BoxCollider))]
    public class MusicVolume : MonoBehaviour
    {
        [SerializeField]
        private AudioClip musicTrack;
        [SerializeField, Tooltip("Higher importance volumes are played")]
        private int importance;

        public int Importance => importance;
        public AudioClip MusicTrack => musicTrack;

        //check the layer. Important: Set up Layers so nothing collides with the music volumes!
        void Start()
        {
            if(gameObject.layer == 0)
            {
                Debug.LogWarning("Music Volumes should not be on the default layer.", this);
            }
        }
    }
}