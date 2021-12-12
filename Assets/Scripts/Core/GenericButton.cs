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
            if(oneTimeUse)
            {
                InteractablesTracker.Unregister(this);
            }
        }

        private void OnDrawGizmos()
        {
            Vector3 labelPos = transform.position;
            for(int i = 0; i < e.GetPersistentEventCount(); i++)
            {
                var target = e.GetPersistentTarget(i);
                if(target != null && target is Component)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(transform.position, (target as Component).transform.position);
                    string methodName = e.GetPersistentMethodName(i);
                    if(string.IsNullOrEmpty(methodName) is false)
                    {
#if UNITY_EDITOR
                        UnityEditor.Handles.color = Color.red;
                        UnityEditor.Handles.Label(labelPos, methodName);
                        labelPos += new Vector3(0, 0.2f, 0);
#endif
                    }
                }
            }
        }
    }
}