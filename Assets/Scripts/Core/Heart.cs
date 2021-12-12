using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostTime.Core
{
    public class Heart : MonoBehaviour
    {
        //for heartbeat
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private float minInterval = 0.9f, maxInterval = 1.05f;
        [SerializeField]
        private float upScaleSize = 1.1f, scaleFadeTime = 0.1f;
        [SerializeField]
        private AnimationCurve fadeCurve;

        private float lastBeat;
        private float nextBeat;

        // Update is called once per frame
        void Update()
        {
            float time = Time.time;
            if(time >= nextBeat)
            {
                //play the audio
                if(audioSource)
                    audioSource.Play();
                //time step the next beat.
                lastBeat = nextBeat;
                nextBeat = time + Random.Range(minInterval, maxInterval);
            }
            //figure out scale.
            float delta = time - lastBeat;
            //only scale when the delta is within range of the scaleFadeTime.
            if(delta <= scaleFadeTime)
            {
                //from 0 to 1 (roughly)
                float t = delta / scaleFadeTime;
                float scale = Mathf.LerpUnclamped(upScaleSize, 1, fadeCurve.Evaluate(t));
                transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }
}