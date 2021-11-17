using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace LostTime.UI
{
    public class SettingsPanel : UIPanel
    {
        [SerializeField]
        private AudioMixer audioMixer;
        [SerializeField]
        private Slider masterVolumeSlider;
        [SerializeField]
        private Slider sfxVolumeSlider;
        [SerializeField]
        private Slider musicVolumeSlider;

        //runtime buffers for the different volume settings.
        private float masterVolume = 0, sfxVolume = 0, musicVolume = 0;
        private float savedMasterVolume = 0, savedSfxVolume = 0, savedMusicVolume = 0;

        internal void Initialize()
        {
            LoadExistingVolumes();
            RegisterSliderChangeEvents();
        }

        private void OnDisable() 
        {
            SaveVolumeOverrides();    
        }

        ///<summary>
        ///Loads the existing volumes from PlayerPrefs, and applies them to the audioMixer and sliders.
        ///</summary>
        private void LoadExistingVolumes()
        {
            //load playerprefs.
            savedMasterVolume = masterVolume = PlayerPrefs.GetFloat(nameof(masterVolume), 0);
            savedSfxVolume    = sfxVolume    = PlayerPrefs.GetFloat(nameof(sfxVolume),    0);
            savedMusicVolume  = musicVolume  = PlayerPrefs.GetFloat(nameof(musicVolume),  0);
            //apply to audioMixer.
            audioMixer.SetFloat(nameof(masterVolume), masterVolume);
            audioMixer.SetFloat(nameof(sfxVolume),    sfxVolume);
            audioMixer.SetFloat(nameof(musicVolume),  musicVolume);
            //update sliders.
            masterVolumeSlider.value = masterVolume;
            sfxVolumeSlider.value    = sfxVolume;
            musicVolumeSlider.value  = musicVolume;
        }

        private void RegisterSliderChangeEvents()
        {
            masterVolumeSlider.onValueChanged.AddListener(
                (value) => UpdateVolume(value, ref masterVolume, nameof(masterVolume))
            );
            sfxVolumeSlider.onValueChanged.AddListener(
                (value) => UpdateVolume(value, ref sfxVolume, nameof(sfxVolume))
            );
            musicVolumeSlider.onValueChanged.AddListener(
                (value) => UpdateVolume(value, ref musicVolume, nameof(musicVolume))
            );
            void UpdateVolume(float value, ref float volume, string name)
            {
                volume = value;
                audioMixer.SetFloat(name, value);
            }
        }

        ///<summary>
        ///saves any changes made to the volumes to playerprefs.
        ///</summary>
        private void SaveVolumeOverrides()
        {
            bool changesMade = false;
            if( !Mathf.Approximately(savedMasterVolume, masterVolume) )
            {
                PlayerPrefs.SetFloat(nameof(masterVolume), masterVolume);
                savedMasterVolume = masterVolume;
                changesMade = true;
            }
            if( !Mathf.Approximately(savedSfxVolume, sfxVolume) )
            {
                PlayerPrefs.SetFloat(nameof(sfxVolume), sfxVolume);
                savedSfxVolume = sfxVolume;
                changesMade = true;
            }
            if( !Mathf.Approximately(savedMusicVolume, musicVolume) )
            {
                PlayerPrefs.SetFloat(nameof(musicVolume), musicVolume);
                savedMusicVolume = musicVolume;
                changesMade = true;
            }

            if(changesMade)
                PlayerPrefs.Save();
        }
    }
}