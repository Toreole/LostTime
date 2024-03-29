﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SebLague.Portals;
using LostTime.UI;

namespace LostTime.Core
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
        private new Camera camera;
        [SerializeField]
        private float normalFov = 60f, sprintFov = 64f;
        [SerializeField]
        private float fovTransitionTime = 0.5f;

        [SerializeField]
        private float stepLength;
        [SerializeField]
        private AudioSource stepAudio;

        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("mouseSensitivity")]
        private float lookSpeed = 10;
        [SerializeField]
        private Vector2 pitchMinMax = new Vector2 (-40, 85);
        [SerializeField]
        private float rotationSmoothTime = 0.1f;

    #endregion

        CharacterController controller;
        float yaw;
        float pitch;
        float smoothYaw;
        float smoothPitch;

        float yawSmoothV;
        float pitchSmoothV;
        float verticalVelocity;
        Vector3 velocity;
        Vector3 smoothV;

        bool jumping;
        float lastGroundedTime;
        Vector3 lastGroundedPosition;

        float mouseSensitivity = 1;
        float travelledDist = 0f;
        private float activeSprintTime01 = 0f;

        protected override void Start () 
        {
            base.Start();
            mouseSensitivity = PlayerPrefs.GetFloat(nameof(mouseSensitivity), 1);

            controller = GetComponent<CharacterController> ();

            yaw = transform.eulerAngles.y;
            pitch = camera.transform.localEulerAngles.x;
            smoothYaw = yaw;
            smoothPitch = pitch;

            //prepare to get changed settings.
            SettingsPanel.OnMouseSensitivityChanged += SetMouseSensitivity;
        }

        private void OnDestroy()
        {
            //unbind the event to be clean.
            SettingsPanel.OnMouseSensitivityChanged -= SetMouseSensitivity;
        }

        private void SetMouseSensitivity(float value) => mouseSensitivity = value;

        internal void MovementAndRotation(AbilityUnlocks abilities)
        {
            Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

            Vector3 inputDir = new Vector3 (input.x, 0, input.y).normalized;
            Vector3 worldInputDir = transform.TransformDirection (inputDir);

            float currentSpeed;
            if (abilities.HasFlag(AbilityUnlocks.SPRINT) && Input.GetKey(KeyCode.LeftShift) && input.sqrMagnitude > 0.2f)
            {
                currentSpeed = runSpeed;
                activeSprintTime01 += Time.deltaTime / fovTransitionTime;
            }
            else
            {
                currentSpeed = walkSpeed;
                activeSprintTime01 -= Time.deltaTime / fovTransitionTime;
            }
            //clamp the sprinttime between 0 and 1
            activeSprintTime01 = Mathf.Clamp01(activeSprintTime01);
            //adjust fov
            camera.fieldOfView = Mathf.Lerp(normalFov, sprintFov, activeSprintTime01);

            Vector3 targetVelocity = worldInputDir * currentSpeed;
            velocity = Vector3.SmoothDamp (velocity, targetVelocity, ref smoothV, smoothMoveTime);

            //calculate the horizontal distance that will be travelled in this frame.
            if(!jumping) //while not on ground, disregard this
                travelledDist += new Vector3(velocity.x, 0, velocity.z).magnitude * Time.deltaTime;
            if(travelledDist >= stepLength)
            {
                travelledDist -= stepLength;
                stepAudio.pitch = Random.Range(0.95f, 1.05f);
                stepAudio.Play();
            }

            verticalVelocity -= gravity * Time.deltaTime;
            velocity = new Vector3 (velocity.x, verticalVelocity, velocity.z);

            var flags = controller.Move (velocity * Time.deltaTime);
            if (flags == CollisionFlags.Below)
            {
                jumping = false;
                lastGroundedTime = Time.time;
                verticalVelocity = 0;
                lastGroundedPosition = transform.position;
            }

            //if the JUMP flag is set, check for key input
            if (abilities.HasFlag(AbilityUnlocks.JUMP) && Input.GetKeyDown (KeyCode.Space)) 
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
            float rotationSpeed = lookSpeed * mouseSensitivity;
            yaw += mX * rotationSpeed;
            pitch -= mY * rotationSpeed;
            pitch = Mathf.Clamp (pitch, pitchMinMax.x, pitchMinMax.y);
            smoothPitch = Mathf.SmoothDampAngle (smoothPitch, pitch, ref pitchSmoothV, rotationSmoothTime);
            smoothYaw = Mathf.SmoothDampAngle (smoothYaw, yaw, ref yawSmoothV, rotationSmoothTime);

            transform.eulerAngles = Vector3.up * smoothYaw;
            camera.transform.localEulerAngles = Vector3.right * smoothPitch;
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

        private void OnTriggerEnter(Collider other)
        {
            //hacky way to implement this, but easy to do.
            if(other.CompareTag("OutOfBounds"))
            {
                controller.enabled = false;
                transform.position = lastGroundedPosition;
                controller.enabled = true;
            }
        }
    }
}