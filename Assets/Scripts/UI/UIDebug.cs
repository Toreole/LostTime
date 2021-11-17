using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostTime.UI;

namespace LostTime.UI.Test
{
    public class UIDebug : MonoBehaviour
    {
        public PauseMenu pauseMenu;

        bool paused = false;

        private void Update()
        {
            bool input = Input.GetKeyDown(KeyCode.Escape);
            if(paused && input)
            {
                paused = !pauseMenu.Close();
                if(paused is false)
                {
                    Debug.Log("Pause Menu Closed.");
                }
            }   
            else if (input)
            {
                paused = true;
                pauseMenu.Open();
            } 
        }   
    }
}