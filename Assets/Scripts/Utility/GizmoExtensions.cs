using UnityEngine;
using UnityEngine.Events;

namespace LostTime.Utility
{
    public static class GizmoExtensions
    {
        public static void DrawDescriptors(this UnityEvent e, Transform transform, Color color, ref Vector3 offset, float spacing)
        {
            Vector3 startPos = transform.position;
            for (int i = 0; i < e.GetPersistentEventCount(); i++)
            {
                Object target = e.GetPersistentTarget(i);
                var tTransform = target.GetType().GetProperty("transform");
                if (target != null && tTransform != null)
                {
                    Vector3 labelPos = startPos + offset;
                    Gizmos.color = color;
                    Gizmos.DrawLine(transform.position, (tTransform.GetValue(target) as Transform).position);
                    string methodName = e.GetPersistentMethodName(i);
                    if (string.IsNullOrEmpty(methodName) is false)
                    {
#if UNITY_EDITOR
                        GUI.color = color;
                        UnityEditor.Handles.Label(labelPos, methodName);
                        offset += new Vector3(0, spacing, 0);
#endif
                    }
                }
            }
        }
    }
}
