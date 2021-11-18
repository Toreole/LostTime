using System.Collections;
using UnityEngine;

namespace LostTime.Core
{
    public class SceneLoadTrigger : SceneLoadBehaviour //not sure if we're going to use this, but can't hurt to have it tbh.
    {
        [SerializeField]
        private string playerTag;

        private void Start()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }


        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag(playerTag))
            {
                LoadUnload();
            }
        }

    }
}