using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostTime.Core;

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

        private readonly List<RectTransform> highlighters = new List<RectTransform>(10);

        [SerializeField]
        private GameObject highlighterPrefab;

        // Start is called before the first frame update
        void Start()
        {
            for(int i = 0; i < highlighters.Capacity; i++)
            {
                highlighters.Add(Instantiate(highlighterPrefab, this.transform).transform as RectTransform);
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
                if (screenPos.x < 0 || screenPos.x > resolution.x || screenPos.y < 0 || screenPos.y > resolution.y || screenPos.z < 0 || screenPos.z > 8)
                {
                    highlighters[i].gameObject.SetActive(false);
                    continue;
                }
                highlighters[i].gameObject.SetActive(true);
                float relativeX = screenPos.x / resolution.x;
                float relativeY = screenPos.y / resolution.y;
                Vector2 anchoredPos;
                anchoredPos.x = Mathf.Lerp(minPos.x, halfResX - halfSize, relativeX);
                anchoredPos.y = Mathf.Lerp(minPos.y, halfResY - halfSize, relativeY);
                //Debug.Log($"{worldPos.x}:>{screenPos.x}:>{rect.width}:>{anchoredPos.x}");
                highlighters[i].anchoredPosition = anchoredPos;
                //Debug.Log(screenPos);
            }
            for(; i < highlighters.Count; i++)
            {
                highlighters[i].gameObject.SetActive(false);
            }
        }

    }
}