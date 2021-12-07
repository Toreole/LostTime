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
        [SerializeField]
        private Slider voiceVolumeSlider;
        [SerializeField]
        private Slider mouseSensitivitySlider;

        //runtime buffers for the different volume settings.
        private float masterVolume = 0, sfxVolume = 0, musicVolume = 0, voiceVolume = 0;
        private float savedMasterVolume = 0, savedSfxVolume = 0, savedMusicVolume = 0, savedVoiceVolume = 0;
        //same for mouse sensitivity;
        private float mouseSensitivity = 1;
        private float savedMouseSensitivity = 1;

        public static event System.Action<float> OnMouseSensitivityChanged;

        internal void Initialize()
        {
            LoadSavedSettings();
            RegisterSliderChangeEvents();
        }

        private void OnDisable() 
        {
            SaveVolumeOverrides();    
        }

        ///<summary>
        ///Loads the existing volumes from PlayerPrefs, and applies them to the audioMixer and sliders.
        ///</summary>
        private void LoadSavedSettings()
        {
            //load playerprefs.
            savedMasterVolume = masterVolume = PlayerPrefs.GetFloat(nameof(masterVolume), 0);
            savedSfxVolume    = sfxVolume    = PlayerPrefs.GetFloat(nameof(sfxVolume),    0);
            savedMusicVolume  = musicVolume  = PlayerPrefs.GetFloat(nameof(musicVolume),  0);
            savedVoiceVolume  = voiceVolume  = PlayerPrefs.GetFloat(nameof(voiceVolume),  0);
            savedMouseSensitivity = mouseSensitivity = PlayerPrefs.GetFloat(nameof(mouseSensitivity), 1);
            //apply to audioMixer.
            audioMixer.SetFloat(nameof(masterVolume), masterVolume);
            audioMixer.SetFloat(nameof(sfxVolume),    sfxVolume);
            audioMixer.SetFloat(nameof(musicVolume),  musicVolume);
            audioMixer.SetFloat(nameof(voiceVolume),  voiceVolume);
            //update sliders.
            masterVolumeSlider.value = Mathf.Pow(10, masterVolume / 20.0f);
            sfxVolumeSlider.value    = Mathf.Pow(10, sfxVolume / 20.0f);
            musicVolumeSlider.value  = Mathf.Pow(10, musicVolume / 20.0f);
            voiceVolumeSlider.value  = Mathf.Pow(10, voiceVolume / 20.0f);
            mouseSensitivitySlider.value = mouseSensitivity;
        }

        private void RegisterSliderChangeEvents()
        {
            //volume sliders.
            masterVolumeSlider.onValueChanged.AddListener(
                (value) => UpdateVolume(value, ref masterVolume, nameof(masterVolume))
            );
            sfxVolumeSlider.onValueChanged.AddListener(
                (value) => UpdateVolume(value, ref sfxVolume, nameof(sfxVolume))
            );
            musicVolumeSlider.onValueChanged.AddListener(
                (value) => UpdateVolume(value, ref musicVolume, nameof(musicVolume))
            );
            voiceVolumeSlider.onValueChanged.AddListener(
                (value) => UpdateVolume(value, ref voiceVolume, nameof(voiceVolume))
            );
            void UpdateVolume(float value, ref float volume, string name)
            {
                value = Mathf.Log10(value) * 20;
                volume = value;
                audioMixer.SetFloat(name, value);
            }
            //mouse sensitivity.
            mouseSensitivitySlider.onValueChanged.AddListener((value) => mouseSensitivity = value);
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
            if( !Mathf.Approximately(savedVoiceVolume, voiceVolume))
            {
                PlayerPrefs.SetFloat(nameof(voiceVolume), voiceVolume);
                savedVoiceVolume = voiceVolume;
                changesMade = true;
            }
            //check if the mouse sensitivity has been changed.
            if( !Mathf.Approximately(savedMouseSensitivity, mouseSensitivity))
            {
                PlayerPrefs.SetFloat (nameof(mouseSensitivity), mouseSensitivity);
                savedMouseSensitivity = mouseSensitivity;
                changesMade = true;
                OnMouseSensitivityChanged?.Invoke(mouseSensitivity);
            }

            if(changesMade)
                PlayerPrefs.Save();
        }
    }
}