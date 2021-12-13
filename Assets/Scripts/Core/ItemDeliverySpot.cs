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

        private void OnDrawGizmos()
        {
            Vector3 labelPos = transform.position;
            for (int i = 0; i < onItemDelivered.GetPersistentEventCount(); i++)
            {
                Object target = onItemDelivered.GetPersistentTarget(i);
                var tTransform = target.GetType().GetProperty("transform");
                if (target != null && tTransform != null)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(transform.position, (tTransform.GetValue(target) as Transform).position);
                    string methodName = onItemDelivered.GetPersistentMethodName(i);
                    if (string.IsNullOrEmpty(methodName) is false)
                    {
#if UNITY_EDITOR
                        GUI.color = Color.blue;
                        UnityEditor.Handles.Label(labelPos, methodName);
                        labelPos += new Vector3(0, 0.2f, 0);
#endif
                    }
                }
            }
        }
    }
}