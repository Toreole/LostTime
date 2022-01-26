using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostTime.UI
{
    public class Credits : MonoBehaviour
    {
        [SerializeField]
        private RectTransform parent;
        [SerializeField]
        private float scrollSpeed = 20f;
        [SerializeField]
        private float fastScrollSpeed = 50f;
        [SerializeField]
        private UnityEngine.UI.LayoutGroup layoutProvider;

        [SerializeField]
        private MainMenu menu;
        [SerializeField, NaughtyAttributes.Scene]
        private string menuSceneName;
        [SerializeField]
        private bool loadSceneOnFinish = false;

        private new RectTransform transform;

        private float contentHeight;
        private float endPositionY;

        private bool fullyInitialized = false;
        private bool mayLoadScene = true;

        private IEnumerator Start()
        {
            GetTransform();
            var pos = transform.anchoredPosition;
            pos.y = -9999f;
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(transform);
            yield return null;
            contentHeight = layoutProvider.preferredHeight;
            endPositionY = 0.5f * contentHeight + parent.rect.height;
            //reset position to below view.
            pos.y = -(contentHeight * 0.5f);
            transform.anchoredPosition = pos;

            fullyInitialized = true;
        }

        private void OnEnable()
        {
            if(fullyInitialized)
            {
                var pos = transform.anchoredPosition;
                pos.y = -(contentHeight * 0.5f);
                transform.anchoredPosition = pos;
            }
        }

        private void GetTransform()
        {
            if (!transform)
            {
                transform = base.transform as RectTransform;
            }
        }

        private void Update()
        {
            var pos = transform.anchoredPosition;
            //move with the correct speed
            pos.y += (Input.anyKey ? fastScrollSpeed : scrollSpeed) * Time.deltaTime;
            transform.anchoredPosition = pos;
            if(pos.y >= endPositionY || Input.GetKeyDown(KeyCode.Escape))
            {
                if (menu)
                    menu.PopLayer();
                if(!string.IsNullOrEmpty(menuSceneName) && loadSceneOnFinish && mayLoadScene)
                {
                    mayLoadScene = false;
                    LostTime.Core.SceneManagement.GotoScene(menuSceneName);
                }
            }
        }
    }
}