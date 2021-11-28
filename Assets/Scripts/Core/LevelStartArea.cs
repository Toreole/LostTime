using System.Collections;
using UnityEngine;

namespace LostTime.Core
{
    public class LevelStartArea : MonoBehaviour
    {
        public static LevelStartArea Current;

        [SerializeField]
        private Transform defaultStartingLocation;
        [SerializeField]
        private Animator animator;

        readonly int doorTrigger = Animator.StringToHash("doorOpen");

        private void Awake()
        {
            Current = this;
        }

        /// <summary>
        /// moves the player to this start area (more or less seamlessly)
        /// </summary>
        /// <param name="playerTransform">the player's transform</param>
        /// <param name="relativeTo">the transform of the area the player is being teleported away from (elevator)</param>
        public void MovePlayerHere(Transform playerTransform, Transform relativeTo)
        {
            //transform the players position
            Vector3 relativePosition = playerTransform.position - relativeTo.position;
            relativePosition = relativeTo.InverseTransformDirection(relativePosition);
            playerTransform.position = transform.TransformVector(relativePosition);
            //transform the players looking direction
            Vector3 lookDir = relativeTo.InverseTransformDirection(playerTransform.forward);
            playerTransform.forward = transform.TransformDirection(lookDir);

            //stolen matrix magic from the Portal script.
            //Matrix4x4 m = relativeTo.localToWorldMatrix * transform.worldToLocalMatrix * playerTransform.localToWorldMatrix;
            //playerTransform.position = m.GetColumn(3);
            //playerTransform.rotation = m.rotation;
        }
        public void ResetPlayer(Transform playerTransform)
        {
            playerTransform.position = defaultStartingLocation.position;
            playerTransform.rotation = defaultStartingLocation.rotation;
        }

        public void OpenElevatorDoors()
        {
            animator.SetTrigger(doorTrigger);
        }
    }
}