using LostTime.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LostTime.Utility.GizmoExtensions;

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
            if(requiredItem is null)
                return;
            if(player.HasItem(requiredItem))
            {
                gameObject.SetActive(false);
                onItemDelivered?.Invoke();
            }
        }

        private void OnDrawGizmos()
        {
            Vector3 offset = new Vector3(0, 0, 0);
            onItemDelivered.DrawDescriptors(transform, Color.blue, ref offset, 0.2f);
        }

        public void PlayVoiceOver(LostTime.Audio.VoiceOver vo)
        {
            Player.Instance.PlayVoiceOver(vo);
        }
    }
}