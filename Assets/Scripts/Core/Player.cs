using System.Collections.Generic;
using UnityEngine;
using LostTime.UI;
using LostTime.Audio;
using System;

namespace LostTime.Core
{
    public class Player : MonoBehaviour
    {
        [SerializeField]
        PauseMenu pauseMenu;
        [SerializeField]
        FPSController controller;
        [SerializeField]
        GameObject ingameOverlay;
        [SerializeField]
        GameObject inventoryIcon;
        [SerializeField]
        Crosshair crosshair;
        [SerializeField]
        ItemInspector itemInspector;
        [SerializeField]
        ComplexItemContainer inventoryUIContainer;
        [SerializeField]
        VoiceOverHandler voiceOverHandler;
        [SerializeField]
        CharacterController characterController;

        [SerializeField]
        private new Transform camera;
        [SerializeField]
        private float interactionRange;
        [SerializeField]
        private LayerMask interactionMask;
        [SerializeField]
        private Camera screenshotCamera;
        private RenderTexture screenshotTexture;

        private AbilityUnlocks unlockedAbilities = AbilityUnlocks.NONE;
        private List<Item> inventory = new List<Item>(15); //15 for now, might not need more, but it will adapt to it if needed.
        private ControlMode currentControlMode = ControlMode.Player;
        public CharacterController CharacterController => characterController;

        public static Player Instance { get; private set; }

        private ControlMode ActiveControlMode
        {
            get => currentControlMode;
            set
            {
                currentControlMode = value;
                bool playerActive = currentControlMode == ControlMode.Player;
                if(controller)
                    controller.enabled = playerActive;
                Cursor.lockState = playerActive ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = !playerActive;
                //ingameOverlay.SetActive(playerActive);
            }
        }

        // Use this for initialization
        void Start()
        {
            Instance = this;
            ActiveControlMode = ControlMode.Player;
            PauseMenu.OnMenuClosed += () => ActiveControlMode = ControlMode.Player;
            //set up the screenshot stuff.
            screenshotTexture = new RenderTexture(256, 256, 1, RenderTextureFormat.ARGB32);
            screenshotTexture.useMipMap = false;
            screenshotCamera.targetTexture = screenshotTexture;
            //inventory stuff
            inventoryIcon.SetActive(false);
            inventoryUIContainer.Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            MenuFunctionality();
            if (ActiveControlMode is ControlMode.Player)
            {
                CheckInteraction();
                //make the controller update based on the unlocked abilities.
                controller.MovementAndRotation(unlockedAbilities);
            }
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F7))
                this.SetAbilityUnlocked(AbilityUnlocks.ALL);
#endif
        }

        private void MenuFunctionality()
        {
            //Escape: exiting open menus, pause menu.
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                switch (ActiveControlMode)
                {
                    case ControlMode.Player:
                        ActiveControlMode = ControlMode.PauseMenu;
                        pauseMenu.Open();
                        break;
                    case ControlMode.PauseMenu:
                        pauseMenu.Close();
                        break;
                    case ControlMode.InspectItem:
                        itemInspector.StopInspecting();
                        ActiveControlMode = ControlMode.Player;
                        break;
                    case ControlMode.Inventory:
                        CloseInventory();
                        break;
                    case ControlMode.None:
                        break;
                }
            }
            //I: inventory keybind.
            if(unlockedAbilities.HasFlag(AbilityUnlocks.INVENTORY) && Input.GetKeyDown(KeyCode.I))
            {
                switch(ActiveControlMode)
                {
                    case ControlMode.Player:
                        OpenInventory();
                        break;
                    case ControlMode.Inventory:
                        CloseInventory();
                        break;
                }
            }
        }

        private void CheckInteraction()
        {
            if (!crosshair) //temp: remove
                return;
            if (Physics.SphereCast(camera.position, 0.1f, camera.forward, out RaycastHit hit, interactionRange, interactionMask))
            {
                var interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        interactable.Interact(this);
                    }
                    crosshair.Set(interactable.GetCrosshairType());
                }
                else
                    crosshair.Set(CrosshairType.Default);
            }
            else
                crosshair.Set(CrosshairType.Default);

        }

        /// <summary>
        /// Start inspecting an object with the specified properties.
        /// </summary>
        public void InspectObject(Mesh mesh, Material[] sharedMaterials, string objectName, string description, Transform t)
        {
            Matrix4x4 transformationMatrix = t.localToWorldMatrix * camera.worldToLocalMatrix;
            itemInspector.StartInspecting(mesh, sharedMaterials, objectName, description, transformationMatrix.rotation);
            ActiveControlMode = ControlMode.InspectItem;
        }

        /// <summary>
        /// The absolute simplest way to add an item to an inventory lol. so basic rn.
        /// </summary>
        public void PickupItem(Item item, GameObject obj, bool disableObject)
        {
            //the first time picking up an item allows permanent access to the inventory.
            if (unlockedAbilities.HasFlag(AbilityUnlocks.INVENTORY) is false)
                this.SetAbilityUnlocked(AbilityUnlocks.INVENTORY);
            inventory.Add(item);
            screenshotCamera.Render();
            //System.IntPtr texPtr = screenshotTexture.GetNativeTexturePtr();
            Texture2D texture = new Texture2D(256, 256, TextureFormat.ARGB32, false, false);
            Graphics.CopyTexture(screenshotTexture, texture);
            //Texture2D.CreateExternalTexture(256, 256, TextureFormat.ARGB32, false, false, texPtr);
            item.Sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width >> 1, texture.height >> 1));
            inventoryUIContainer.AddItemAndShow(item);
            OpenInventory();
            if(disableObject)
                obj.SetActive(false);
        }

        /// <summary>
        /// Open the inventory.
        /// </summary>
        private void OpenInventory()
        {
            ActiveControlMode = ControlMode.Inventory;
            inventoryUIContainer.Show();
            voiceOverHandler.Pause();
        }

        private void CloseInventory()
        {
            //close inventory and resume voiceover playing.
            inventoryUIContainer.Hide();
            voiceOverHandler.Resume();
            ActiveControlMode = ControlMode.Player;
        }

        /// <summary>
        /// Causes a VoiceOver to playm or queues it up.
        /// </summary>
        public void PlayVoiceOver(VoiceOver vo)
        {
            voiceOverHandler.QueueVoiceOver(vo);
        }

        /// <summary>
        /// Checks if the player has a specific item.
        /// </summary>
        public bool HasItem(Item item) => inventory.Contains(item);

        /// <summary>
        /// Grants an ability to the player.
        /// </summary>
        public void SetAbilityUnlocked(AbilityUnlocks ability)
        {
            unlockedAbilities |= ability;
            if (unlockedAbilities.HasFlag(AbilityUnlocks.INVENTORY))
                inventoryIcon.SetActive(true);
        }

        public AbilityUnlocks GetAbilityUnlocks() => unlockedAbilities;
        private enum ControlMode
        {
            None = 0,
            Player = 1,
            PauseMenu = 2,
            InspectItem = 3,
            Inventory = 4
        }
    }

    /// <summary>
    /// The "abilities" that the player has unlocked.
    /// </summary>
    [System.Flags, Serializable]
    public enum AbilityUnlocks
    {
        NONE = 0,
        INVENTORY = 1 << 0,
        JUMP = 1 << 1,
        SPRINT = 1 << 2,

        ALL = int.MaxValue //should be all 1s, every flag set
    }
}