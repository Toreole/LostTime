using LostTime.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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
            return CrosshairType.Press;
        }

        public override void Interact(Player p)
        {
            if (used && oneTimeUse)
                return;
            e.Invoke();
            used = true;
        }
    }
}