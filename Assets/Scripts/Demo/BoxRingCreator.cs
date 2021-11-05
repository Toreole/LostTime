using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxRingCreator : MonoBehaviour
{
    public int boxCount = 100;
    public float minRadius = 40;
    public float maxRadius = 45;
    public float minAngleDist = 0.5f;
    public float maxAngleDist = 2;
    public float centerRotationSpeed = 5;
    public float minSize = 0.8f;
    public float maxSize = 2f;

    public float verticalVariation = 2;

    Transform[] boxes;
    Transform t;

    // Start is called before the first frame update
    void Start()
    {
        t = transform;
        boxes = new Transform[boxCount];
        float angle = 0;
        Vector3 localPos = new Vector3(Random.Range(minRadius, maxRadius), 0, 0);
        for(int i = 0; i < boxCount; i++)
        {
            var b = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            boxes[i] = b;
            b.parent = t;
            b.localPosition = localPos;
            b.rotation = Random.rotation;
            angle += Random.Range(minAngleDist, maxAngleDist);
            b.localScale = new Vector3(Random.Range(minSize, maxSize),Random.Range(minSize, maxSize),Random.Range(minSize, maxSize));
            localPos = Quaternion.Euler(0, angle, 0) * new Vector3(Random.Range(minRadius, maxRadius), Random.Range(-verticalVariation, verticalVariation), 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        t.Rotate(t.up, centerRotationSpeed*Time.deltaTime, Space.World);
    }
}
