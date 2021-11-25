using UnityEngine;
using UnityEngine.UI;

namespace LostTime.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPanel : MonoBehaviour
    {
        [SerializeField]
        private bool deactivatePrevious = true;   
        [SerializeField]
        private CanvasGroup canvasGroup;

        protected virtual void Start()
        {
            if(!canvasGroup)
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public bool DeactivatePrevious => deactivatePrevious;
        public bool Interactable
        {
            get => canvasGroup.interactable;
            set => canvasGroup.interactable = value;
        }

        ///<summary>.gameObject.SetActive(bool)</summary>
        public void SetActive(bool a) => gameObject.SetActive(a);
    }
}