using System;
using MackySoft.Choice;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(menuName = "Custom/Loot table")]
    public class LootTable : ScriptableObject
    {
        public Loot[] loots;

        public Item Loot()
        {
            if (loots == null || loots.Length == 0)
            {
                return null;
            }

            return loots.ToWeightedSelector(l => l.weight).SelectItemWithUnityRandom().item;
        }

        public static LootTable Empty => new();
    }

    [Serializable]
    public class Loot
    {
        public Item item;

        [Tooltip("Chance of looting this item relative to the others in the same loot table. The higher the more likely this will drop.")]
        public float weight;
    }
}
