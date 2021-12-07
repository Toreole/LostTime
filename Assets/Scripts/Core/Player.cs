using System.Collections.Generic;
using UnityEngine;
using LostTime.UI;
using LostTime.Audio;

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
        Crosshair crosshair;
        [SerializeField]
        ItemInspector itemInspector;
        [SerializeField]
        GameObject inventoryUI;
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
            screenshotTexture = new RenderTexture(256, 256, 1, RenderTextureFormat.ARGB32);
            screenshotTexture.useMipMap = false;
            screenshotCamera.targetTexture = screenshotTexture;

            inventoryUIContainer.Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            MenuFunctionality();
            if(ActiveControlMode is ControlMode.Player)
                CheckInteraction();
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
                        inventoryUIContainer.Hide();
                        ActiveControlMode = ControlMode.Player;
                        break;
                    case ControlMode.None:
                        break;
                }
            }
            //I: inventory keybind.
            if(Input.GetKeyDown(KeyCode.I))
            {
                switch(ActiveControlMode)
                {
                    case ControlMode.Player:
                        if (voiceOverHandler.IsPlaying) //block the inventory while playing voiceovers. easy fix
                            break;
                        inventoryUIContainer.Show();
                        ActiveControlMode = ControlMode.Inventory;
                        break;
                    case ControlMode.Inventory:
                        inventoryUIContainer.Hide();
                        ActiveControlMode = ControlMode.Player;
                        break;
                }
            }
        }

        private void CheckInteraction()
        {
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
        public void InspectObject(Mesh mesh, Material[] sharedMaterials, string objectName, string description)
        {
            itemInspector.StartInspecting(mesh, sharedMaterials, objectName, description);
            ActiveControlMode = ControlMode.InspectItem;
        }

        /// <summary>
        /// The absolute simplest way to add an item to an inventory lol. so basic rn.
        /// </summary>
        public void PickupItem(Item item, GameObject obj)
        {
            inventory.Add(item);
            screenshotCamera.Render();
            //System.IntPtr texPtr = screenshotTexture.GetNativeTexturePtr();
            Texture2D texture = new Texture2D(256, 256, TextureFormat.ARGB32, false, false);
            Graphics.CopyTexture(screenshotTexture, texture);
            //Texture2D.CreateExternalTexture(256, 256, TextureFormat.ARGB32, false, false, texPtr);
            item.Sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width >> 1, texture.height >> 1));
            inventoryUIContainer.AddItem(item);
            obj.SetActive(false);
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

        private enum ControlMode
        {
            None = 0,
            Player = 1,
            PauseMenu = 2,
            InspectItem = 3,
            Inventory = 4
        }
    }
}