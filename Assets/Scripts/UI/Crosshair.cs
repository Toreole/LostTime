using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LostTime.UI
{
    [RequireComponent(typeof(Image))]
    public class Crosshair : MonoBehaviour
    {
        [SerializeField, Tooltip("ORDER: Default - Door - Press - Pickup - LookAt")]
        private Sprite[] sprites = new Sprite[5];
        
        private Image uiImage;
        private CrosshairType current = CrosshairType.Default;

        // Use this for initialization
        void Start()
        {
            uiImage = GetComponent<Image>();
            uiImage.sprite = sprites[0];
        }

        public void Set(CrosshairType type)
        {
            if (current != type)
            {
                uiImage.sprite = sprites[(int)type];
                current = type;
            }
        }

    }
    public enum CrosshairType
    {
        Default = 0,
        Door = 1,
        Press = 2,
        Pickup = 3,
        LookAt = 4
    }
}