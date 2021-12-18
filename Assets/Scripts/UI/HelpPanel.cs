using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostTime.UI
{
    public class HelpPanel : UIPanel
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI textElement;

        //Refresh the contents of the panel 
        private void OnEnable()
        {
            //fetch abilities
            var abilities = LostTime.Core.Player.Instance.GetAbilityUnlocks();
            //init stringbuilder
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            //add all the keybinds to the help menu
            stringBuilder.AppendLine("Move: W/A/S/D");
            stringBuilder.AppendLine("Look Around: Mouse");
            stringBuilder.AppendLine("Interact: E");
            stringBuilder.AppendLine("Pause: Escape");
            if(abilities.HasFlag(Core.AbilityUnlocks.JUMP)) stringBuilder.AppendLine("Jump: Spacebar");
            if (abilities.HasFlag(Core.AbilityUnlocks.INVENTORY)) stringBuilder.AppendLine("Inventory: I");
            if (abilities.HasFlag(Core.AbilityUnlocks.SPRINT)) stringBuilder.AppendLine("Sprint: Left Shift");

            textElement.text = stringBuilder.ToString();
        }
    }
}