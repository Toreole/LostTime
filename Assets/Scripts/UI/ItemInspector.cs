using UnityEngine;
using UnityEngine.UI;
using LostTime.Audio;
using TMPro;

namespace LostTime.UI
{
    public class ItemInspector : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer itemRenderer;
        [SerializeField]
        private MeshFilter itemMesh;
        [SerializeField]
        private Transform itemTransform;
        [SerializeField]
        private Camera itemCamera;

        [SerializeField]
        private float rotationSpeed = 80f;
        [SerializeField]
        private float preferredItemSize = 1f;
        [SerializeField]
        private RawImage rawImage;

        [SerializeField]
        TextMeshProUGUI nameDisplay, descriptionDisplay;

        private RenderTexture renderTexture;

        private void Update()
        {
            if(Input.GetMouseButton(0))
            {
                Vector2 rotationDirection = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                float magnitude = rotationDirection.magnitude;
                Vector3 axis = Vector2.Perpendicular(rotationDirection / magnitude);
                magnitude *= -rotationSpeed * Time.deltaTime;

                itemTransform.Rotate(axis, magnitude, Space.World);
            }
        }

        public void StartInspecting(Mesh mesh, Material[] sharedMaterials, string itemName, string description, Quaternion rotation)
        {
            if(renderTexture is null)
            {
                renderTexture = new RenderTexture(800, 800, 0);
                rawImage.texture = renderTexture;
                itemCamera.targetTexture = renderTexture;
            }
            gameObject.SetActive(true);
            //attempt to normalize the size of the object in bounds.
            var bounds = mesh.bounds;
            var maxBoundLength = Mathf.Max(bounds.size.x, bounds.size.y);
            var objectScale = preferredItemSize / maxBoundLength;
            itemTransform.localScale = new Vector3(objectScale, objectScale, objectScale);

            //Set up the item.
            nameDisplay.text = itemName;
            descriptionDisplay.text = description;

            itemTransform.rotation = rotation;
            itemMesh.mesh = mesh;
            itemRenderer.sharedMaterials = sharedMaterials;

            BGMHandler.SuppressBGM();
        }

        public void StopInspecting()
        {
            gameObject.SetActive(false);
            BGMHandler.FreeBGM();
        }
    }
}