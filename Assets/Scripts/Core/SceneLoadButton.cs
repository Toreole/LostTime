using System.Collections;
using UnityEngine;

namespace LostTime.Core
{
    //Extremely basic implementation of this concept. Expand on it as needed.
    public class SceneLoadButton : SceneLoadBehaviour, IInteractable
    {
        public void Interact(Player player)
        {
            LoadUnload();
        }
    }
}