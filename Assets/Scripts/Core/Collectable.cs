using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostTime.Core
{
    public class Collectable : Interactable
    {
        [SerializeField]
        private Item item;

        public override void Interact(Player player)
        {
            player.PickupItem(item, this.gameObject);
        }
    }
}