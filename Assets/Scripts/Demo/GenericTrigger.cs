using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericTrigger : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent e; 

    private void OnTriggerEnter(Collider other) 
    {
        e?.Invoke();
    }
}
