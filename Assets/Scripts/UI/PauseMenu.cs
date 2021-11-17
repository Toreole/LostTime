using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostTime.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField]
        CanvasGroup pauseMenuGroup;

        [SerializeField]
        private SettingsPanel settingsPanel;

        void Start()
        {
            settingsPanel.Initialize();

            //Hide the pause menu by default.
            pauseMenuGroup.interactable = false;
            pauseMenuGroup.alpha = 0;
            pauseMenuGroup.blocksRaycasts = false;
        }
    }
}