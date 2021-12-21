using UnityEngine;
using UnityEngine.Events;
using static LostTime.Utility.GizmoExtensions;

namespace LostTime.Core
{
    public class InspectableObject : Interactable
    {
        private new MeshRenderer renderer;
        private Mesh mesh;
        [SerializeField]
        private string objectName, description;
        [SerializeField]
        private UnityEvent onFirstInspect;

        private bool hasBeenInspected = false;

        // Use this for initialization
        void Start()
        {
            mesh = GetComponent<MeshFilter>().mesh;
            renderer = GetComponent<MeshRenderer>();
        }

        public override UI.CrosshairType GetCrosshairType()
        {
            return UI.CrosshairType.LookAt;
        }

        public override void Interact(Player player)
        {
            player.InspectObject(mesh, renderer.sharedMaterials, objectName, description, transform);
            if(hasBeenInspected is false)
            {
                hasBeenInspected = true;
                onFirstInspect?.Invoke();
            }
        }

        private void OnDrawGizmos()
        {
            Vector3 offset = Vector3.zero;
            onFirstInspect.DrawDescriptors(transform, Color.cyan, ref offset, 0.2f);
        }

    }
}