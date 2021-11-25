using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LostTime.Core;

namespace LostTime.UI
{
    public class MainMenu : LayeredUIMenu
    {
        [SerializeField]
        private Image image;
        [SerializeField]
        private SettingsPanel settingsPanel;
        [SerializeField]
        private string startScene = "";

        protected override void Start()
        {
            settingsPanel.Initialize();
        }

        public override void Open()
        {
            image.enabled = true;
            mainPanel.SetActive(true);
        }

        public void PlayGame()
        {
            SceneManagement.GotoScene(startScene);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}