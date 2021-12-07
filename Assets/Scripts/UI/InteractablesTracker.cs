using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostTime.Core;
using UnityEngine.UI;

namespace LostTime.UI
{
    public class InteractablesTracker : MonoBehaviour
    {
        static readonly List<Interactable> interactables = new List<Interactable>(10);
        
        public static void Register(Interactable interactable)
        {
            interactables.Add(interactable);
        }
        public static void Unregister(Interactable interactable)
        {
            interactables.Remove(interactable);
        }

        private readonly List<InteractableHighlighter> highlighters = new List<InteractableHighlighter>(10);

        [SerializeField]
        private GameObject highlighterPrefab;
        [SerializeField]
        private float interactionRange = 1.75f;
        [SerializeField]
        private float highlightRange = 6f;

        // Start is called before the first frame update
        void Start()
        {
            for(int i = 0; i < highlighters.Capacity; i++)
            {
                var instantiatedObject = Instantiate(highlighterPrefab, this.transform);
                highlighters.Add(new InteractableHighlighter() {
                    image = instantiatedObject.GetComponent<Image>(),
                    transform = instantiatedObject.transform as RectTransform 
                });
            }
        }

        void LateUpdate()
        {
            var camera = Camera.main; //lol
            var resolution = new Vector2(Display.main.renderingWidth, Display.main.renderingHeight);
            var rect = (transform as RectTransform).rect;
            const float halfSize = 75f / 2f;
            float halfResX = rect.width * 0.5f;
            float halfResY = rect.height * 0.5f;
            Vector2 minPos = new Vector2(-halfResX + halfSize, -halfResY + halfSize);
            int i = 0;
            for(; i < interactables.Count; i++)
            {
                if (i >= highlighters.Count)
                    break;
                //get world position
                var worldPos = interactables[i].transform.position;
                var screenPos = camera.WorldToScreenPoint(worldPos);
                //more accurate-ish distance than screenPos.z
                //interactables[i].GetComponent<Collider>().ClosestPoint(camera.transform.position);
                //float colDistance = Vector3.Distance(camera.transform.position, worldPos);
                //^not used because honestly, screenPos.z is a good-enough estimate of the distance.

                //do not highlight this interactable if: outside of screen OR behind camera OR out of range.
                if (screenPos.x < 0 || screenPos.x > resolution.x || screenPos.y < 0 || screenPos.y > resolution.y || screenPos.z < 0 || screenPos.z > highlightRange)
                {
                    highlighters[i].GameObject.SetActive(false);
                    continue;
                }
                //make highlighters outside of the interaction range semi-transparent.
                highlighters[i].Alpha = screenPos.z <= interactionRange ? 1.0f : 0.4f;
                //linear scale from interactionRange to highlightRange
                float scale = 1.0f - ((screenPos.z - interactionRange) / (highlightRange - interactionRange));
                highlighters[i].transform.localScale = new Vector3(scale, scale, scale);
                //enable the gameObject.
                highlighters[i].GameObject.SetActive(true);
                float relativeX = screenPos.x / resolution.x;
                float relativeY = screenPos.y / resolution.y;
                Vector2 anchoredPos;
                anchoredPos.x = Mathf.Lerp(minPos.x, halfResX - halfSize, relativeX);
                anchoredPos.y = Mathf.Lerp(minPos.y, halfResY - halfSize, relativeY);
                //Debug.Log($"{worldPos.x}:>{screenPos.x}:>{rect.width}:>{anchoredPos.x}");
                highlighters[i].transform.anchoredPosition = anchoredPos;
                //Debug.Log(screenPos);
            }
            for(; i < highlighters.Count; i++)
            {
                highlighters[i].GameObject.SetActive(false);
            }
        }

        private class InteractableHighlighter
        {
            public RectTransform transform;
            public Image image;

            public Color Color { get => image.color; set => image.color = value; }
            public float Alpha { get => image.color.a; set { var col = Color;  col.a = value;  image.color = col; } }
            public GameObject GameObject => transform.gameObject;

        }

    }
}