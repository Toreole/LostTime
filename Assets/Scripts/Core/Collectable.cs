using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostTime.Core
{
    public class Collectable : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private Item item;

        public void Interact(Player player)
        {
            gameObject.SetActive(false);
            player.PickupItem(item);
        }
    }
}