using LostTime.Audio;
using LostTime.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LostTime.UI
{
    public class ComplexItemContainer : MonoBehaviour
    {
        [SerializeField]
        private float radius = 350;
        [SerializeField]
        private GameObject itemDisplayPrefab;
        [SerializeField]
        private TextMeshProUGUI itemName;
        [SerializeField]
        private TextMeshProUGUI itemDescription;
        [SerializeField]
        private float focusDuration = 4.0f;
        [SerializeField]
        private GameObject itemInfo;

        List<ItemDisplay> itemDisplayers = new List<ItemDisplay>(2);
        Queue<ItemDisplay> emptyDisplays;

        private ItemDisplay focusedItemDisplay;

        private GameObject rootObject;
        private float lastFocusTime = 0;
        //just a buffer to check state change from rotating to not rotating.
        private bool rotating = true;

        /// <summary>
        /// Initializes the UI for everything.
        /// </summary>
        public void Initialize()
        {
            rootObject = transform.parent.gameObject;
            for (int i = 0; i < itemDisplayers.Capacity; i++)
            {
                itemDisplayers.Add(this.InstantiateDisplay());
            }
            DistributeDisplays();
            emptyDisplays = new Queue<ItemDisplay>(itemDisplayers);
            itemInfo.SetActive(false);
        }

        readonly static AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        IEnumerator DoRotateTowards(ItemDisplay disp)
        {
            Vector3 startOffset = disp.LocalPosition.normalized;
            Vector3 targetOffset = new Vector3(0, 0, -1);

            if (startOffset == targetOffset)
                yield break;

            if (disp.Item)//update the text.
            {
                itemName.alpha = 1;
                itemName.text = disp.Item.itemName;
                itemDescription.alpha = 1;
                itemDescription.text = disp.Item.itemDescription;
            }
            else
            {
                itemName.alpha = 0;
                itemDescription.alpha = 0;
            }

            //rotate all the items.
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
                SortDisplays();
                yield return null;
            }
        }

        /// <summary>
        /// Sorts the ItemDisplays by their local Z position. farther away items appear behind closer ones.
        /// </summary>
        private void SortDisplays()
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
            //instead of autorotate, check the last time an itemdisplay was focused.
            if (Time.time <= lastFocusTime + focusDuration)
                return;
            //should rotate otherwise:
            if(rotating is false)
            {
                rotating = true;
                //"deactivate" the description and name displays for the items.
                itemDescription.alpha = 0;
                itemName.alpha = 0;
                UnfocusCurrentDisplay();
            }

            Quaternion q = Quaternion.Euler(0, 15 * Time.deltaTime, 0);
            for (int i = 0; i < itemDisplayers.Count; i++)
            {
                itemDisplayers[i].LocalPosition = q * itemDisplayers[i].LocalPosition;
            }
            SortDisplays();
        }

        /// <summary>
        /// Creates an instance of the Display prefab and sets it up.
        /// </summary>
        /// <returns></returns>
        private ItemDisplay InstantiateDisplay()
        {
            var item = Instantiate(itemDisplayPrefab, this.transform);
            var disp = item.GetComponent<ItemDisplay>();
            disp.Color = Color.HSVToRGB(Random.value, 0.7f, 1);
            disp.OnClick += HandleClick;
            
            return disp;
        }

        /// <summary>
        /// switch Focus to the given ItemDisplay.
        /// </summary>
        /// <param name="display"></param>
        private void FocusDisplay(ItemDisplay display)
        {
            if (focusedItemDisplay != null)
                focusedItemDisplay.Focused = false;
            display.Focused = true;
            focusedItemDisplay = display;
            lastFocusTime = Time.time;
            //set buffer
            rotating = false;
            //update item description text
            if (display.Item)
            {
                itemDescription.text = display.Item.itemDescription;
                itemName.text = display.Item.itemName;
                itemDescription.alpha = 1;
                itemName.alpha = 1;
                itemInfo.SetActive(true);
            }
            else
            {
                itemDescription.alpha = 0;
                itemName.alpha = 0;
                itemInfo.SetActive(false);
            }
        }

        /// <summary>
        /// Removes the current active display from focus.
        /// </summary>
        public void UnfocusCurrentDisplay()
        {
            if(focusedItemDisplay)
            {
                focusedItemDisplay.Focused = false;
                focusedItemDisplay = null;
                lastFocusTime = float.NegativeInfinity;
                itemInfo.SetActive(false);
            }
        }

        /// <summary>
        /// Handles OnPointerDown events for the ItemDisplays. Since no additional data is necessary, it just needs the target of the click.
        /// </summary>
        /// <param name="display"></param>
        void HandleClick(ItemDisplay display)
        {
            this.StopAllCoroutines();
            StartCoroutine(DoRotateTowards(display));
            FocusDisplay(display);
        }

        //fibonacci sphere samples based on https://stackoverflow.com/questions/9600801/evenly-distributing-n-points-on-a-sphere 
        private static readonly float phi = Mathf.PI * (3f - Mathf.Sqrt(5f)); //golden angle in rad
        private void DistributeDisplays()
        {
            if (itemDisplayers.Count == 2)
            {
                itemDisplayers[0].LocalPosition = new Vector3(radius, 0, 0);
                itemDisplayers[1].LocalPosition = Vector3.ClampMagnitude(new Vector3(-radius, 0, radius * 0.5f), radius);
            }
            else
            {
                for (int i = 0; i < itemDisplayers.Count; i++)
                {

                    float y = 1f - ((float)i / (float)(itemDisplayers.Capacity - 1)) * 2f;
                    float r = Mathf.Sqrt(1f - y * y);//"radius" at y
                    float theta = phi * (float)i;

                    float x = Mathf.Cos(theta) * r;
                    float z = Mathf.Sin(theta) * r;

                    Vector3 offset = new Vector3(x, y, z) * radius;
                    itemDisplayers[i].LocalPosition = offset;
                }
            }
            SortDisplays();
        }

        /// <summary>
        /// Silently adds the item to the inventory.
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(Item item)
        {
            if(emptyDisplays.Count > 0)
            {
                SetUpItemDisplay(emptyDisplays.Dequeue(), item);
            }
            else
            {
                //instantiate a new display in the UI
                var d = InstantiateDisplay();
                SetUpItemDisplay(d, item);
                itemDisplayers.Add(d);
                DistributeDisplays();
            }
        }

        /// <summary>
        /// Adds the item to the displayed items, then immediately focuses it.
        /// </summary>
        /// <param name="item"></param>
        public void AddItemAndShow(Item item)
        {
            ItemDisplay display = null;
            if (emptyDisplays.Count > 0)
            {
                display = emptyDisplays.Dequeue();
                SetUpItemDisplay(display, item);
            }
            else
            {
                //instantiate a new display in the UI
                display = InstantiateDisplay();
                SetUpItemDisplay(display, item);
                itemDisplayers.Add(display);
                DistributeDisplays();
            }
            //focus it.
            FocusDisplay(display);
            //now rotate the fictional sphere so the focused one is in front.
            Vector3 startOffset = display.LocalPosition.normalized;
            Vector3 targetOffset = new Vector3(0, 0, -1);
            Vector3 axis = Vector3.Cross(startOffset, targetOffset).normalized;
            float angle = Vector3.Angle(startOffset, targetOffset);
            Quaternion rotation = Quaternion.AngleAxis(angle, axis);
            //apply the rotation to every vector.
            for (int i = 0; i < itemDisplayers.Count; i++)
                itemDisplayers[i].LocalPosition = rotation * itemDisplayers[i].LocalPosition;
            SortDisplays();
        }

        private void SetUpItemDisplay(ItemDisplay disp, Item item)
        {
            disp.Item = item;
            disp.Color = Color.white;
        }

        //Show the Inventory UI.
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
            StopAllCoroutines();
            itemName.alpha = 0;
            itemDescription.alpha = 0;
            UnfocusCurrentDisplay();
            rotating = true;
        }
    }
}