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

        public void PlayVoiceOver(Audio.VoiceOver vo) => Player.Instance.PlayVoiceOver(vo);

        public abstract CrosshairType GetCrosshairType();
    }
}
