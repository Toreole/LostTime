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
        private new Transform camera;
        [SerializeField]
        private float interactionRange;
        [SerializeField]
        private LayerMask interactionMask;

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
        }

        // Update is called once per frame
        void Update()
        {
            EscapeFunctionality();
            if(ActiveControlMode is ControlMode.Player)
                CheckInteraction();
        }

        private void EscapeFunctionality()
        {
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
                    case ControlMode.None:
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
        public void PickupItem(Item item)
        {
            inventory.Add(item);
        }

        private enum ControlMode
        {
            None = 0,
            Player = 1,
            PauseMenu = 2,
            InspectItem = 3,

        }
    }
}