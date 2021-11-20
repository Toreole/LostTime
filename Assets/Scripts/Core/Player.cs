﻿using System.Collections.Generic;
using UnityEngine;
using LostTime.UI;

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
        ItemInspector itemInspector;
        [SerializeField]
        GameObject inventoryUI;
        [SerializeField]
        ComplexItemContainer inventoryUIContainer;

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

        private ControlMode ActiveControlMode
        {
            get => currentControlMode;
            set
            {
                currentControlMode = value;
                bool playerActive = currentControlMode == ControlMode.Player;
                if(controller)
                    controller.enabled = playerActive;
                ingameOverlay.SetActive(playerActive);
            }
        }

        // Use this for initialization
        void Start()
        {
            PauseMenu.OnMenuClosed += () => ActiveControlMode = ControlMode.Player;
            screenshotTexture = new RenderTexture(256, 256, 1, RenderTextureFormat.ARGB32);
            screenshotTexture.useMipMap = false;
            screenshotCamera.targetTexture = screenshotTexture;
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
                        inventoryUI.SetActive(false);
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
                        inventoryUI.SetActive(true);
                        ActiveControlMode = ControlMode.Inventory;
                        break;
                    case ControlMode.Inventory:
                        inventoryUI.SetActive(false);
                        ActiveControlMode = ControlMode.Player;
                        break;
                }
            }
        }

        private void CheckInteraction()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Physics.SphereCast(camera.position, 0.2f, camera.forward, out RaycastHit hit, interactionRange, interactionMask))
                {
                    var interactable = hit.collider.GetComponent<IInteractable>();
                    if (interactable != null)
                    {
                        interactable.Interact(this);
                    }
                }
            }
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