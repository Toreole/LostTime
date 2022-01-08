using LostTime.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using static LostTime.Utility.GizmoExtensions;

namespace LostTime.Core
{
    public class GenericButton : Interactable
    {
        [SerializeField]
        private UnityEvent e;
        [SerializeField]
        private bool oneTimeUse = true;

        private bool used = false;

        public override CrosshairType GetCrosshairType()
        {
            return oneTimeUse && used? CrosshairType.Default : CrosshairType.Press;
        }

        public override void Interact(Player p)
        {
            if (used && oneTimeUse)
                return;
            e.Invoke();
            used = true;
            if(oneTimeUse)
            {
                InteractablesTracker.Unregister(this);
            }
        }

        private void OnDrawGizmos()
        {
            Vector3 offset = new Vector3(0, 0, 0);
            e.DrawDescriptors(transform, Color.red, ref offset, 0.2f);
        }

    }
}