using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LostTime.UI
{
    public class ItemDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField]
        private new RectTransform transform;
        private Vector3 localPosition = Vector3.zero;

        [SerializeField]
        private Image itemImage;
        [SerializeField]
        private Image borderImage;

        public float LocalZ => localPosition.z;
        public Vector3 LocalPosition { get { return localPosition; } set { localPosition = value; transform.localPosition = value; } }
        public Color Color { get => itemImage.color; set => itemImage.color = value; }

        public event System.Action<ItemDisplay> OnClick;

        public void SetSiblingIndex(int i) => transform.SetSiblingIndex(i);
        public void SetScale(float f) => transform.localScale = new Vector3(f, f, f);


        public void OnPointerDown(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
                OnClick?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            borderImage.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            borderImage.enabled = false;
        }
    }
}