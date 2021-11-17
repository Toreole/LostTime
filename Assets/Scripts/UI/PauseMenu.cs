using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        private GameObject mainPanel;

        private Stack<UIPanel> layeredPanels = new Stack<UIPanel>(3); //max of 3 layers.

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
            pauseMenuGroup.interactable = true;
            pauseMenuGroup.alpha = 1;
            pauseMenuGroup.blocksRaycasts = true;
            OnMenuOpened?.Invoke();
        }

        public void GotoSettingsPanel()
        {
            AddPanelLayer(settingsPanel);
        }

        ///<summary>
        ///Adds a UIPanel as a Layer on top of the PauseMenu.
        ///</summary>
        private void AddPanelLayer(UIPanel panel)
        {
            panel.SetActive(true);
            if(panel.DeactivatePrevious)
            {
                if(layeredPanels.Count == 0)
                    mainPanel.SetActive(false);
                else
                    layeredPanels.Peek().SetActive(false);
            }
            if(layeredPanels.Count > 0)
                layeredPanels.Peek().Interactable = false;
            layeredPanels.Push(panel);
        }

        public void PopLayer()
        {
            if(layeredPanels.Count > 0)
            {
                var topPanel = layeredPanels.Pop();
                topPanel.SetActive(false);
                if(topPanel.DeactivatePrevious)
                {
                    if(layeredPanels.Count == 0)
                        mainPanel.SetActive(true);
                    else 
                        layeredPanels.Peek().SetActive(true);
                }
                if(layeredPanels.Count > 0)
                    layeredPanels.Peek().Interactable = false;
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
                if(topPanel.DeactivatePrevious)
                {
                    if(layeredPanels.Count == 0)
                        mainPanel.SetActive(true);
                    else 
                        layeredPanels.Peek().SetActive(true);
                }
                if(layeredPanels.Count > 0)
                    layeredPanels.Peek().Interactable = false;
                return false;
            }
            //menu is fully closed now.
            OnMenuClosed?.Invoke();
            pauseMenuGroup.interactable = false;
            pauseMenuGroup.alpha = 0;
            pauseMenuGroup.blocksRaycasts = false;
            return true;
        }

        public void ClosePauseMenu()
        {
            OnMenuClosed?.Invoke();
            pauseMenuGroup.interactable = false;
            pauseMenuGroup.alpha = 0;
            pauseMenuGroup.blocksRaycasts = false;
        }
    }
}