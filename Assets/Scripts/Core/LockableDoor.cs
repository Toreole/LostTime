using UnityEngine;

namespace LostTime.Core
{
    public class LockableDoor : Door
    {
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private bool isLocked = true;

        public void Unlock()
        {
            if (isLocked is false)
                return;
            if (audioSource != null)
                audioSource.Play();
            isLocked = false;
        }

        //Add: check if the door is unlocked.
        public override void Interact(Player player)
        {
            if (isLocked is false)
                base.Interact(player);
            else
                player.PlayVoiceOver(null); //"Impossible"
        }

    }
}
