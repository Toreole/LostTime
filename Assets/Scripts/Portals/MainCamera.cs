using System.Collections.Generic;
using UnityEngine;

namespace SebLague.Portals
{
    public class MainCamera : MonoBehaviour 
    {
        public static MainCamera Instance { get; private set; }
        public Camera Camera { get; private set; }

        List<Portal> portals = new List<Portal>(8);

        private void Awake()
        {
            this.Camera = GetComponent<Camera>();
            Instance = this;
        }

        void OnPreCull () 
        {

            for (int i = 0; i < portals.Count; i++) 
            {
                if(portals[i].gameObject.activeInHierarchy)
                    portals[i].PrePortalRender ();
            }
            for (int i = 0; i < portals.Count; i++)
            {
                if (portals[i].gameObject.activeInHierarchy)
                    portals[i].Render ();
            }

            for (int i = 0; i < portals.Count; i++)
            {
                if (portals[i].gameObject.activeInHierarchy)
                    portals[i].PostPortalRender ();
            }
        }

        public void RegisterPortal(Portal portal) => portals.Add(portal);
        public void UnregisterPortal(Portal portal) => portals.Remove(portal);
    }
}