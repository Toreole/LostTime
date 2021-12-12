using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostTime.Core
{
    public class FogToggle : MonoBehaviour
    {
        [SerializeField]
        private Color changedFogColor;
        [SerializeField]
        private float changedFogDensity;


        private bool defaultFog = false;
        private Color inFogColor;
        private float inFogDensity;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(2.5f);
            inFogColor = RenderSettings.fogColor;
            inFogDensity = RenderSettings.fogDensity;

            RenderSettings.fogColor = changedFogColor;
            RenderSettings.fogDensity = changedFogDensity;
        }

        public void ChangeFog()
        {
            defaultFog = !defaultFog;
            RenderSettings.fogColor = defaultFog ? inFogColor : changedFogColor;
            RenderSettings.fogDensity = defaultFog ? inFogDensity : changedFogDensity;
        }
    }
}