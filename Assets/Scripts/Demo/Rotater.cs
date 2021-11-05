using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    public Vector3 angleSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(angleSpeed * Time.deltaTime, Space.World);  
    }
}
