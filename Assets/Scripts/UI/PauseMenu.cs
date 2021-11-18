using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostTime.Core;

namespace LostTime.UI
{
    public class PauseMenu : MonoBehaviour
    {
        public static event Action OnMenuOpened;
        public static event Action OnMenuClosed;

        [SerializeField]
        CanvasGroup pauseMenuGroup;

        [SerializeField]
        private SettingsPanel settingsPanel;
        [SerializeField]
        private UIPanel mainPanel;
        [SerializeField]
        private string titleMenuScene = "MainMenu";

        private Stack<UIPanel> layeredPanels = new Stack<UIPanel>(3); //max of 3 layers.
        private float lastTimeScale = 1;

        void Start()
        {
            settingsPanel.Initialize();

            //Hide the pause menu by default.
            pauseMenuGroup.interactable = false;
            pauseMenuGroup.alpha = 0;
            pauseMenuGroup.blocksRaycasts = false;
        }

        public void Open()
        {
            lastTimeScale = Time.timeScale;
            Time.timeScale = 0;
            pauseMenuGroup.interactable = true;
            pauseMenuGroup.alpha = 1;
            pauseMenuGroup.blocksRaycasts = true;
            OnMenuOpened?.Invoke();
        }

        ///<summary>
        ///Adds a UIPanel as a Layer on top of the PauseMenu.
        ///</summary>
        public void AddPanelLayer(UIPanel panel)
        {
            panel.SetActive(true);
            panel.Interactable = true;
            var previousPanel = layeredPanels.Count == 0 ? mainPanel : layeredPanels.Peek();
            if(panel.DeactivatePrevious)
                previousPanel.SetActive(false);
            //activate is just for visibility, interactable is seperate
            previousPanel.Interactable = false;
            layeredPanels.Push(panel);
        }

        public void PopLayer()
        {
            if(layeredPanels.Count > 0)
            {
                var topPanel = layeredPanels.Pop();
                topPanel.SetActive(false);
                var bottompanel = layeredPanels.Count == 0 ? mainPanel : layeredPanels.Peek();
                if (topPanel.DeactivatePrevious)
                    bottompanel.SetActive(true);
                bottompanel.Interactable = true;
            }
        }

        ///<summary>
        ///"Closes" the menu in steps/layers.
        ///Returns true if every aspect of the Menu is closed, and control should be returned to the player.
        ///</summary>
        public bool Close()
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
                return false;
            }
            //menu is fully closed now.
            OnMenuClosed?.Invoke();
            pauseMenuGroup.interactable = false;
            pauseMenuGroup.alpha = 0;
            pauseMenuGroup.blocksRaycasts = false;
            Time.timeScale = lastTimeScale;
            return true;
        }

        public void ClosePauseMenu()
        {
            OnMenuClosed?.Invoke();
            pauseMenuGroup.interactable = false;
            pauseMenuGroup.alpha = 0;
            pauseMenuGroup.blocksRaycasts = false;
        }

        public void QuitToTitle()
        {
            SceneManagement.GotoScene(titleMenuScene);
        }
    }
}