using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    
    public float speed = 1;

    Vector3 startPos;

    bool towardsOrigin;

    private void Start()
    {
        startPos = transform.localPosition;   
    }

    // Update is called once per frame
    void Update()
    {
        if(towardsOrigin)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, speed* Time.deltaTime);
            if(Vector3.SqrMagnitude(transform.localPosition) < 0.01)
                towardsOrigin = false;
        }
        else 
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, startPos, speed* Time.deltaTime);
            if(Vector3.SqrMagnitude(transform.localPosition - startPos) < 0.01)
                towardsOrigin = true;
        }
    }
}
