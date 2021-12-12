using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericTrigger : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent e; 

    private void OnTriggerEnter(Collider other) 
    {
        e?.Invoke();
    }

    private void OnDrawGizmos()
    {
        Vector3 labelPos = transform.position;
        for (int i = 0; i < e.GetPersistentEventCount(); i++)
        {
            var target = e.GetPersistentTarget(i);
            if (target != null && target is Component)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, (target as Component).transform.position);
                string methodName = e.GetPersistentMethodName(i);
                if (string.IsNullOrEmpty(methodName) is false)
                {
#if UNITY_EDITOR
                    UnityEditor.Handles.color = Color.green;
                    UnityEditor.Handles.Label(labelPos, methodName);
                    labelPos += new Vector3(0, 0.2f, 0);
#endif
                }
            }
        }
    }
}
