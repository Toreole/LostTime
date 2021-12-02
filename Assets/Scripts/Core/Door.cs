using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostTime.Core
{
    public class Door : Interactable
    {
        [SerializeField]
        private new Transform transform;
        [SerializeField]
        private float closedRotation = 0;
        [SerializeField]
        private float openRotationAngle = 95;

        private bool isOpened = false;
        private float currentAngle = 0;

        readonly AnimationCurve openCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        const float openTime = 1f;

        private void Start()
        {
            SetRotation(closedRotation);
        }

        private void SetRotation(float rotation)
        {
            currentAngle = rotation;
            transform.localRotation = Quaternion.Euler(0, rotation, 0);
        }

        public override void Interact(Player player)
        {
            StopAllCoroutines();
            StartCoroutine(DoOpenClose());
        }

        IEnumerator DoOpenClose()
        {
            float targetRotation = isOpened? closedRotation : openRotationAngle;
            float baseRotation = isOpened? openRotationAngle : closedRotation;
            float startRotation = currentAngle;
            //toggle isOpened
            isOpened = !isOpened;
            //the relative progress right now
            float relativeProgress = Mathf.InverseLerp(baseRotation, targetRotation, currentAngle);
            float operationTime = openTime - (relativeProgress * openTime);

            for(float t = 0; t < operationTime; t += Time.deltaTime)
            {
                float nt = t / operationTime;
                float lerpValue = openCurve.Evaluate(nt);
                var angle = Mathf.Lerp(startRotation, targetRotation, lerpValue);
                SetRotation(angle);
                yield return null;
            }
        }
    }
}