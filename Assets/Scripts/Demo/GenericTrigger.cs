using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LostTime.Utility.GizmoExtensions;

public class GenericTrigger : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent e; 

    private void OnTriggerEnter(Collider other) 
    {
        e?.Invoke();
    }

    private void OnDrawGizmos()
    {

        Vector3 offset = new Vector3(0, 0, 0);
        e.DrawDescriptors(transform, Color.green, ref offset, 0.2f);
    }

    public void UnlockJump()
    {
        LostTime.Core.Player.Instance.SetAbilityUnlocked(LostTime.Core.AbilityUnlocks.JUMP);
    }
    public void UnlockSprint()
    {
        LostTime.Core.Player.Instance.SetAbilityUnlocked(LostTime.Core.AbilityUnlocks.SPRINT);
    }
}
