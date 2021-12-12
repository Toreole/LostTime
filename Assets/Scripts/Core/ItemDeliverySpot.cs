using LostTime.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostTime.Core
{
    public class ItemDeliverySpot : Interactable
    {
        [SerializeField]
        private Item requiredItem;
        [SerializeField]
        private UnityEngine.Events.UnityEvent onItemDelivered;

        public override CrosshairType GetCrosshairType()
        {
            return CrosshairType.Default; //TODO: Make crosshair type for deliveries
        }

        public override void Interact(Player player)
        {
            if(player.HasItem(requiredItem))
            {
                gameObject.SetActive(false);
                onItemDelivered?.Invoke();
            }
        }
    }
}