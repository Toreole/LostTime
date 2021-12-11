using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostTime.Core
{
    public class SkyboxChanger : MonoBehaviour
    {
        [SerializeField]
        private Material skyboxMaterial;
        public void SetSkybox()
        {
            RenderSettings.fog = true;
            //RenderSettings.skybox = skyboxMaterial;
            //DynamicGI.UpdateEnvironment();
        }
    }
}