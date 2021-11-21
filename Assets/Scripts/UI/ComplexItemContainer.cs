using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostTime.Core;
using LostTime.Audio;

namespace LostTime.UI
{
    public class ComplexItemContainer : MonoBehaviour
    {
        [SerializeField]
        private float radius = 350;
        [SerializeField]
        private GameObject itemDisplayPrefab;

        List<ItemDisplay> itemDisplayers = new List<ItemDisplay>(20);

        private ItemDisplay focusedItemDisplay;

        private bool autoRotate = true;
        private GameObject rootObject;

        // Start is called before the first frame update
        void Start()
        {
            rootObject = transform.parent.gameObject;
            float phi = Mathf.PI * (3f - Mathf.Sqrt(5f)); //golden angle in rad

            for (int i = 0; i < itemDisplayers.Capacity; i++)
            {
                var item = Instantiate(itemDisplayPrefab, this.transform);
                var disp = item.GetComponent<ItemDisplay>();
                disp.Color = Color.HSVToRGB(Random.value, 0.7f, 1);
                //set the position.
                //fibonacci sphere samples based on https://stackoverflow.com/questions/9600801/evenly-distributing-n-points-on-a-sphere 
                float y = 1f - ((float)i / (float)(itemDisplayers.Capacity - 1)) * 2f;
                float r = Mathf.Sqrt(1f - y * y);//"radius" at y
                float theta = phi * (float)i;

                float x = Mathf.Cos(theta) * r;
                float z = Mathf.Sin(theta) * r;

                Vector3 offset = new Vector3(x, y, z) * radius;
                itemDisplayers.Add(disp);
                disp.OnClick += (x) => { //the only time where this { placement is acceptable.
                    this.StopAllCoroutines(); 
                    StartCoroutine(DoRotateTowards(x));
                    if (focusedItemDisplay)
                        focusedItemDisplay.Focused = false;
                    focusedItemDisplay = x;
                    x.Focused = true;
                };
                itemDisplayers[i].LocalPosition = offset;
            }
        }

        readonly static AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        IEnumerator DoRotateTowards(ItemDisplay disp)
        {
            autoRotate = false;
            Vector3 startOffset = disp.LocalPosition.normalized;
            Vector3 targetOffset = new Vector3(0, 0, -1);

            if (startOffset == targetOffset)
                yield break;

            Vector3 axis = Vector3.Cross(startOffset, targetOffset).normalized;
            float angle = Vector3.Angle(startOffset, targetOffset);
            float lastAngle = 0;
            for (float t = 0; t < 0.5f; t += Time.deltaTime)
            {
                var nt = t / 0.5f;
                var p = curve.Evaluate(nt);
                float realAngle = p * angle;
                float deltaAngle = realAngle - lastAngle;
                lastAngle = realAngle;
                Quaternion rotation = Quaternion.AngleAxis(deltaAngle, axis);
                //this is kinda expensive to do, but its the only way
                for (int i = 0; i < itemDisplayers.Count; i++)
                {
                    itemDisplayers[i].LocalPosition = rotation * itemDisplayers[i].LocalPosition;
                }
                SortItems();
                yield return null;
            }
            yield return new WaitForSeconds(4);
            autoRotate = true;
            focusedItemDisplay.Focused = false;
            focusedItemDisplay = null;
        }

        private void SortItems()
        {
            itemDisplayers.Sort((a, b) =>
            {
                return a.LocalZ < b.LocalZ ? 1 : -1;
            });
            for (int i = 0; i < itemDisplayers.Count; i++)
            {
                itemDisplayers[i].SetSiblingIndex(i);
                float iLerp = Mathf.InverseLerp(radius, -radius, itemDisplayers[i].LocalZ);
                float scale = Mathf.Lerp(0.5f, 1f, iLerp);
                itemDisplayers[i].SetScale(scale);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (autoRotate is false)
                return;

            Quaternion q = Quaternion.Euler(0, 15 * Time.deltaTime, 0);
            for (int i = 0; i < itemDisplayers.Count; i++)
            {
                itemDisplayers[i].LocalPosition = q * itemDisplayers[i].LocalPosition;
            }
            SortItems();
        }

        public void AddItem(Item item)
        {
            itemDisplayers[0].Sprite = item.Sprite;
            itemDisplayers[0].Color = Color.white;
        }

        public void Show()
        {
            BGMHandler.SuppressBGM();
            if (rootObject is null)
                rootObject = transform.parent.gameObject;
            rootObject.SetActive(true);
        }

        public void Hide()
        {
            BGMHandler.FreeBGM();
            rootObject.SetActive(false);
        }
    }
}