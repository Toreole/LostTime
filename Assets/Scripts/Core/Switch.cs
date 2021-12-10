using LostTime.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LostTime.Core
{
    public class Switch : Interactable
    {
        [SerializeField]
        private UnityEvent onDisable, onEnable;
        [SerializeField]
        private bool turnedOn = false; //switches start as disabled.
        [SerializeField]
        private Transform switchTransform;
        [SerializeField]
        private Vector3 rotationAngle = new Vector3(-15, 0, 0);

        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private AudioClip disableSound, enableSound;

        public override CrosshairType GetCrosshairType()
        {
            return CrosshairType.Press;
        }

        public override void Interact(Player player)
        {
            //simple toggle.
            if(turnedOn)
            {
                onDisable?.Invoke();
                audioSource.clip = disableSound;
                switchTransform.localRotation *= Quaternion.Euler(-rotationAngle);
            }
            else
            {
                onEnable?.Invoke();
                audioSource.clip = enableSound;
                switchTransform.localRotation *= Quaternion.Euler(rotationAngle);
            }
            turnedOn = !turnedOn;
            audioSource.Play();
        }
    }
}