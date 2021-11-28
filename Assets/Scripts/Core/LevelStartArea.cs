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
        [SerializeField]
        private Item completionItem;
        [SerializeField]
        private string mySceneName;

        readonly int doorTrigger = Animator.StringToHash("doorOpen");

        private bool levelCompleted = false;

        private void Awake()
        {
            Current = this;
        }

        /// <summary>
        /// moves the player to this start area (more or less seamlessly)
        /// </summary>
        /// <param name="playerTransform">the player's transform</param>
        /// <param name="relativeOrigin">the transform of the area the player is being teleported away from (elevator)</param>
        public void MovePlayerToFrom(Transform relativeTarget, Transform playerTransform, Transform relativeOrigin)
        {
            //transform the players position
            //Vector3 relativePosition = playerTransform.position - relativeOrigin.position;
            //relativePosition = relativeOrigin.InverseTransformPoint(relativePosition);
            //playerTransform.position = relativeTarget.TransformPoint(relativePosition);
            //transform the players looking direction
            //Vector3 lookDir = relativeOrigin.InverseTransformDirection(playerTransform.forward);
            //playerTransform.forward = relativeTarget.TransformDirection(lookDir);

            //stolen matrix magic from the Portal script.
            //            linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * travellerT.localToWorldMatrix;
            Matrix4x4 m = relativeTarget.localToWorldMatrix * relativeOrigin.worldToLocalMatrix * playerTransform.localToWorldMatrix;
            playerTransform.position = m.GetColumn(3);
            playerTransform.rotation = m.rotation;
        }
        public void ResetPlayer(Transform playerTransform)
        {
            playerTransform.position = defaultStartingLocation.position;
            playerTransform.rotation = defaultStartingLocation.rotation;
        }

        public void GoBackToHub()
        {
            if (!Player.Instance.HasItem(completionItem) || levelCompleted)
                return;

            levelCompleted = true;
            StartCoroutine(DoGoBackToHub());

            IEnumerator DoGoBackToHub()
            {
                TriggerDoors();
                yield return new WaitForSeconds(2);
                var elevator = MainElevator.Instance;
                MovePlayerToFrom(elevator.transform, Player.Instance.transform, this.transform);
                elevator.TriggerDoors();
                elevator.ReEnableLight();
                SceneManagement.UnloadScene(mySceneName);
            }
        }

        public void TriggerDoors()
        {
            animator.SetTrigger(doorTrigger);
        }
    }
}