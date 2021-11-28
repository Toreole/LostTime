using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostTime.Audio
{
    public class BGMHandler : MonoBehaviour
    {
        [SerializeField]
        private AudioSource musicSourceA, musicSourceB;

        [SerializeField]
        private LayerMask layerMask;

        [SerializeField]
        private float fadeTime = 1f;

        [SerializeField, Tooltip("what the volume should be reduced to (relatively) when music is sent to the background (when voiceover is playing / item menu is showing)")]
        private float backgroundLoudness = 0.5f;

        private Collider[] volumeColliders = new Collider[3]; //i assume we're never going to have 3 volumes in a scene. especially not 3 overlapping.
        private MusicVolume currentMusicVolume;
        private bool musicIsPlaying = false;
        private bool fadingOutMusic = false;

        //Current volume is only update by events, and will at the same time also force update the audioSource volume.
        //renamed "loudness" because confusing use of Volume (as in 3D space) and volume (how loud sound is)
        private float currentLoudness = 1;

        private Coroutine coroutine;

        private int bgmSuppressorCount = 0;

        private static BGMHandler instance;

        void Awake()
        {
            if(instance)
            {
                Destroy(gameObject);
                return;
            }
            musicSourceA.Stop();
            musicSourceB.Stop();
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            int n = Physics.OverlapSphereNonAlloc(transform.position, 0.1f, volumeColliders, layerMask);
            //No volumes anymore. Stop playing music.
            if (n == 0 && musicIsPlaying && !fadingOutMusic)
            {
                coroutine = StartCoroutine(DoStopMusic());
            }
            else if(n > 0)
            {
                int importance = int.MinValue;
                MusicVolume nextMusicVol = null;
                for (int i = 0; i < n; i++)
                {
                    //get the volume, check if it should override the current one.
                    MusicVolume vol = volumeColliders[i].GetComponent<MusicVolume>();
                    if (vol && vol.Importance > importance)
                    {
                        nextMusicVol = vol;
                        importance = vol.Importance;
                    }
                }
                if (nextMusicVol != currentMusicVolume)
                {
                    currentMusicVolume = nextMusicVol;
                    if (musicIsPlaying)
                        coroutine = StartCoroutine(DoCrossFade());
                    else
                        coroutine = StartCoroutine(DoStartMusic());
                }
            }
        }

        /// <summary>
        /// Fade the music currently playing to the clip on the new music volume.
        /// </summary>
        private IEnumerator DoCrossFade()
        {
            Debug.Log("Crossfade");
            AudioSource previousSource = (musicSourceA.isPlaying) ? musicSourceA : musicSourceB;
            AudioSource nextSource = (musicSourceA.isPlaying) ? musicSourceB : musicSourceA;

            nextSource.clip = currentMusicVolume.MusicTrack;
            nextSource.Play();

            for (float t = 0; t < fadeTime; t += Time.deltaTime)
            {
                float nt = t / fadeTime; //0.....1
                float ntVolume = nt * currentLoudness; //the current max volume of the audio sources multiplied by the fade nt;
                previousSource.volume = Mathf.Clamp01(currentLoudness - ntVolume);
                nextSource.volume = Mathf.Clamp01(nt);
                yield return null;
            }
            previousSource.Stop();
            nextSource.volume = currentLoudness;
        }

        /// <summary>
        /// Fades in music on the first audio source.
        /// </summary>
        private IEnumerator DoStartMusic()
        {
            musicSourceA.clip = currentMusicVolume.MusicTrack;
            musicSourceA.Play();
            //Debug.Log("Start Music");
            musicIsPlaying = true;

            for (float t = 0; t < fadeTime; t += Time.deltaTime)
            {
                float nt = t / fadeTime; //0.....1
                float ntVolume = nt * currentLoudness; //the current max volume of the audio sources multiplied by the fade nt;
                musicSourceA.volume = ntVolume;
                yield return null;
            }
        }

        /// <summary>
        /// Fades out music on the playing audio source.
        /// </summary>
        private IEnumerator DoStopMusic()
        {
            var activeSource = musicSourceA.isPlaying ? musicSourceA : musicSourceB;
            fadingOutMusic = true;
            //Debug.Log("Stop Music");
            
            for (float t = 0; t < fadeTime; t += Time.deltaTime)
            {
                float nt = t / fadeTime; //0.....1
                float ntVolume = (1.0f - nt) * currentLoudness; //the current max volume of the audio sources multiplied by the fade nt;
                musicSourceA.volume = ntVolume;
                yield return null;
            }
            activeSource.Stop();
            musicIsPlaying = false;
            fadingOutMusic = false;
            currentMusicVolume = null;
            yield break;
        }

        /// <summary>
        /// Puts the BGM into the background (makes it more quite while other stuff is happening)
        /// </summary>
        public static void SuppressBGM()
        {
            if (instance)
                instance._SuppressBGM();
            else
                Debug.LogError("No BGMHandler active in scene.");
        }
        private void _SuppressBGM()
        {
            bgmSuppressorCount += 1;
            currentLoudness = backgroundLoudness;
            musicSourceA.volume = currentLoudness;
            musicSourceB.volume = currentLoudness;
        }

        /// <summary>
        /// Reverses the effects of SuppressBGM
        /// </summary>
        public static void FreeBGM()
        {
            if (instance)
                instance._FreeBGM();
            else
                Debug.LogError("No BGMHandler active in scene.");
        }
        private void _FreeBGM()
        {
            bgmSuppressorCount -= 1;
            if (bgmSuppressorCount <= 0)
            {
                currentLoudness = 1.0f;
                musicSourceA.volume = currentLoudness;
                musicSourceB.volume = currentLoudness;
            }
        }

    }
}