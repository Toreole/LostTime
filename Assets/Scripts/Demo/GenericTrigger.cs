using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LostTime.Utility.GizmoExtensions;

public class GenericTrigger : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.Events.UnityEvent e;
    [SerializeField]
    private bool onlyActivateOnce = true;

    bool hasBeenAcivated = false;

    private void OnTriggerEnter(Collider other) 
    {
        if (hasBeenAcivated && onlyActivateOnce)
            return;
        hasBeenAcivated = true;
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
