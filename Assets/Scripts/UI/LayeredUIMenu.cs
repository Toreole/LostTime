using System.Collections.Generic;
using UnityEngine;

namespace LostTime.UI
{
    public class LayeredUIMenu : MonoBehaviour
    {
        [SerializeField]
        protected UIPanel mainPanel;
        protected Stack<UIPanel> layeredPanels = new Stack<UIPanel>(3); //max of 3 layers.

        protected virtual void Start()
        {

        }

        public virtual void Open()
        {

        }

        public virtual void Close()
        {

        }

        ///<summary>
        ///Adds a UIPanel as a Layer on top of the PauseMenu.
        ///</summary>
        public void AddPanelLayer(UIPanel panel)
        {
            panel.SetActive(true);
            panel.Interactable = true;
            var previousPanel = layeredPanels.Count == 0 ? mainPanel : layeredPanels.Peek();
            if (panel.DeactivatePrevious)
                previousPanel.SetActive(false);
            //activate is just for visibility, interactable is seperate
            previousPanel.Interactable = false;
            layeredPanels.Push(panel);
        }

        /// <summary>
        /// Removes the upper most UIPanel.
        /// </summary>
        public void PopLayer()
        {
            if (layeredPanels.Count > 0)
            {
                var topPanel = layeredPanels.Pop();
                topPanel.SetActive(false);
                var bottompanel = layeredPanels.Count == 0 ? mainPanel : layeredPanels.Peek();
                if (topPanel.DeactivatePrevious)
                    bottompanel.SetActive(true);
                bottompanel.Interactable = true;
            }
        }
    }
}