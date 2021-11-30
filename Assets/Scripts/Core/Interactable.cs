using UnityEngine;
using LostTime.UI;

namespace LostTime.Core
{
    public abstract class Interactable : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            InteractablesTracker.Register(this);
        }
        protected virtual void OnDisable()
        {
            InteractablesTracker.Unregister(this);
        }

        public abstract void Interact(Player player);
    }
}
