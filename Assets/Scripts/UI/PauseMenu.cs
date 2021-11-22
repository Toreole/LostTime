using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostTime.Core;

namespace LostTime.UI
{
    public class PauseMenu : LayeredUIMenu
    {
        public static event Action OnMenuOpened;
        public static event Action OnMenuClosed;

        [SerializeField]
        private SettingsPanel settingsPanel;
        [SerializeField]
        private string titleMenuScene = "MainMenu";

        private float lastTimeScale = 1;

        protected override void Start()
        {
            settingsPanel.Initialize();

            //Hide the pause menu by default.
            mainPanel.SetActive(false);
        }

        public override void Open()
        {
            lastTimeScale = Time.timeScale;
            Time.timeScale = 0;
            mainPanel.SetActive(true);
            OnMenuOpened?.Invoke();
        }

        ///<summary>
        ///"Closes" the menu in steps/layers.
        ///</summary>
        public override void Close()
        {
            //if any layered UIPanels are active, pop them.
            if(layeredPanels.Count > 0)
            {
                var topPanel = layeredPanels.Pop();
                topPanel.SetActive(false);
                var bottompanel = layeredPanels.Count == 0 ? mainPanel : layeredPanels.Peek();
                if (topPanel.DeactivatePrevious)
                    bottompanel.SetActive(true);
                bottompanel.Interactable = true;
                return;
            }
            //menu is fully closed now.
            OnMenuClosed?.Invoke();
            mainPanel.SetActive(false);
            Time.timeScale = lastTimeScale;
        }

        public void ClosePauseMenu()
        {
            OnMenuClosed?.Invoke();
            mainPanel.SetActive(false);
            Time.timeScale = lastTimeScale;
        }

        public void QuitToTitle()
        {
            SceneManagement.GotoScene(titleMenuScene);
        }
    }
}