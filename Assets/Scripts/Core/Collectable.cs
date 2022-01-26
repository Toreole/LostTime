using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostTime.Core
{
    public class Collectable : Interactable
    {
        [SerializeField]
        private Item item;
        [SerializeField]
        private bool disableObject = true;

        public override UI.CrosshairType GetCrosshairType()
        {
            return UI.CrosshairType.Pickup;
        }
        public override void Interact(Player player)
        {
            player.PickupItem(item, this.gameObject, disableObject);
        }
    }
}