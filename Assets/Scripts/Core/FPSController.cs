using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SebLague.Portals;

namespace LostTime
{
    [RequireComponent(typeof(CharacterController))]
    public class FPSController : PortalTraveller 
    {
    #region SerializedSettings
        [SerializeField]
        private float walkSpeed = 3;
        [SerializeField]
        private float runSpeed = 6;
        [SerializeField]
        private float smoothMoveTime = 0.1f;
        [SerializeField]
        private float jumpForce = 8;
        [SerializeField]
        private float gravity = 18;

        [SerializeField]
        private float mouseSensitivity = 10;
        [SerializeField]
        private Vector2 pitchMinMax = new Vector2 (-40, 85);
        [SerializeField]
        private float rotationSmoothTime = 0.1f;

    #endregion

        CharacterController controller;
        Camera cam;
        float yaw;
        float pitch;
        float smoothYaw;
        float smoothPitch;

        float yawSmoothV;
        float pitchSmoothV;
        float verticalVelocity;
        Vector3 velocity;
        Vector3 smoothV;
        Vector3 rotationSmoothVelocity;
        Vector3 currentRotation;

        bool jumping;
        float lastGroundedTime;

        void Start () 
        {
            cam = Camera.main;

            controller = GetComponent<CharacterController> ();

            yaw = transform.eulerAngles.y;
            pitch = cam.transform.localEulerAngles.x;
            smoothYaw = yaw;
            smoothPitch = pitch;
        }

        void Update () 
        {
            MovementAndRotation();
            
        }

        private void MovementAndRotation()
        {
            Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

            Vector3 inputDir = new Vector3 (input.x, 0, input.y).normalized;
            Vector3 worldInputDir = transform.TransformDirection (inputDir);

            float currentSpeed = (Input.GetKey (KeyCode.LeftShift)) ? runSpeed : walkSpeed;
            Vector3 targetVelocity = worldInputDir * currentSpeed;
            velocity = Vector3.SmoothDamp (velocity, targetVelocity, ref smoothV, smoothMoveTime);

            verticalVelocity -= gravity * Time.deltaTime;
            velocity = new Vector3 (velocity.x, verticalVelocity, velocity.z);

            var flags = controller.Move (velocity * Time.deltaTime);
            if (flags == CollisionFlags.Below) 
            {
                jumping = false;
                lastGroundedTime = Time.time;
                verticalVelocity = 0;
            }

            if (Input.GetKeyDown (KeyCode.Space)) 
            {
                float timeSinceLastTouchedGround = Time.time - lastGroundedTime;
                if (controller.isGrounded || (!jumping && timeSinceLastTouchedGround < 0.15f)) 
                {
                    jumping = true;
                    verticalVelocity = jumpForce;
                }
            }

            float mX = Input.GetAxisRaw ("Mouse X");
            float mY = Input.GetAxisRaw ("Mouse Y");

            // Verrrrrry gross hack to stop camera swinging down at start
            float mMag = Mathf.Sqrt (mX * mX + mY * mY);
            if (mMag > 5) {
                mX = 0;
                mY = 0;
            }

            yaw += mX * mouseSensitivity;
            pitch -= mY * mouseSensitivity;
            pitch = Mathf.Clamp (pitch, pitchMinMax.x, pitchMinMax.y);
            smoothPitch = Mathf.SmoothDampAngle (smoothPitch, pitch, ref pitchSmoothV, rotationSmoothTime);
            smoothYaw = Mathf.SmoothDampAngle (smoothYaw, yaw, ref yawSmoothV, rotationSmoothTime);

            transform.eulerAngles = Vector3.up * smoothYaw;
            cam.transform.localEulerAngles = Vector3.right * smoothPitch;
        }

        public override void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) 
        {
            transform.position = pos;
            Vector3 eulerRot = rot.eulerAngles;
            float delta = Mathf.DeltaAngle (smoothYaw, eulerRot.y);
            yaw += delta;
            smoothYaw += delta;
            transform.eulerAngles = Vector3.up * smoothYaw;
            velocity = toPortal.TransformVector (fromPortal.InverseTransformVector (velocity));
            Physics.SyncTransforms ();
        }
    }
}