using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostTime.UI;

namespace LostTime.UI.Test
{
    public class UIDebug : MonoBehaviour
    {
        public PauseMenu pauseMenu;

        public bool paused = false;

        void Start()
        {
            PauseMenu.OnMenuClosed += () => paused = false;
        }

        private void Update()
        {
            bool input = Input.GetKeyDown(KeyCode.Escape);
            if(paused && input)
            {
                pauseMenu.Close();
            }   
            else if (input)
            {
                paused = true;
                pauseMenu.Open();
            } 
        }   
    }
}