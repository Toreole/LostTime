using System.Collections;
using UnityEngine;

namespace LostTime.Core
{
    public class InspectableObject : MonoBehaviour, IInteractable
    {
        private new MeshRenderer renderer;
        private Mesh mesh;
        [SerializeField]
        private string objectName, description;

        // Use this for initialization
        void Start()
        {
            mesh = GetComponent<MeshFilter>().mesh;
            renderer = GetComponent<MeshRenderer>();
        }

        public void Interact(Player player)
        {
            player.InspectObject(mesh, renderer.sharedMaterials, objectName, description);
        }

    }
}