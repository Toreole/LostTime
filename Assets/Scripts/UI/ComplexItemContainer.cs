using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexItemContainer : MonoBehaviour
{
    [SerializeField]
    private float radius = 350;

    RectTransform[] itemTransforms = new RectTransform[20];
    Vector3[] localOffset = new Vector3[20];

    private bool autoRotate = true;

    // Start is called before the first frame update
    void Start()
    {
        float phi = Mathf.PI * (3f - Mathf.Sqrt(5f)); //golden angle in rad

        for(int i = 0; i < itemTransforms.Length; i++)
        {
            var go = new GameObject("Item");
            go.AddComponent<CanvasRenderer>();
            go.AddComponent<UnityEngine.UI.Image>().color = Color.HSVToRGB(Random.value, 0.7f, 1);
            var button = go.AddComponent<UnityEngine.UI.Button>();
            int fixIndex = i;
            button.onClick.AddListener(() => { this.StopAllCoroutines(); StartCoroutine(DoRotateTowards(fixIndex)); });
            itemTransforms[i] = (RectTransform)go.transform;
            itemTransforms[i].SetParent( this.transform );
            //set the position.
            //itemTransforms[i].localPosition = Random.onUnitSphere * radius;
            //fibonacci sphere samples based on https://stackoverflow.com/questions/9600801/evenly-distributing-n-points-on-a-sphere 
            float y = 1f - ((float)i / (float)(itemTransforms.Length - 1)) * 2f;
            float r = Mathf.Sqrt(1f - y * y);//"radius" at y
            float theta = phi * (float) i;

            float x = Mathf.Cos(theta) * r;
            float z = Mathf.Sin(theta) * r;

            Vector3 offset = new Vector3(x, y, z) * radius;

            itemTransforms[i].localPosition = offset;
            itemTransforms[i].localScale = Vector3.one;
            localOffset[i] = offset;
        }
    }

    readonly AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    IEnumerator DoRotateTowards(int index)
    {
        autoRotate = false;
        Vector3 startOffset = localOffset[index].normalized;
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
            for(int i = 0; i < localOffset.Length; i++)
            {
                localOffset[i] = rotation * localOffset[i];
                itemTransforms[i].localPosition = localOffset[i];
            }
            SortItems();
            yield return null;
        }
        yield return new WaitForSeconds(4);
        autoRotate = true;
    }

    private void SortItems()
    {
        List<RectTransform> items = new List<RectTransform>(itemTransforms);
        items.Sort((a, b) =>
        {
            return a.localPosition.z < b.localPosition.z ? 1 : -1;
        });
        for(int i = 0; i < items.Count; i++)
        {
            items[i].SetSiblingIndex(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (autoRotate is false)
            return;

        Quaternion q = Quaternion.Euler(0, 15 * Time.deltaTime, 0);
        for(int i = 0; i < itemTransforms.Length; i++)
        {
            localOffset[i] = q * localOffset[i];
            itemTransforms[i].localPosition = localOffset[i];
        }
        SortItems();
    }
}
