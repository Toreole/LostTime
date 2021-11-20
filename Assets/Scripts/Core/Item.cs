using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostTime.Core
{
    [CreateAssetMenu(menuName = "CustomAsset/Item")]
    public class Item : ScriptableObject
    {
        public string itemName;

        public Sprite Sprite { get; set; }
    }
}