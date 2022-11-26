using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(menuName = "Custom/Items config")]
    public class ItemsRuntimeConfig: ScriptableObject
    {
        public GameItem baseItem;
        
        [Header("Config")]
        public float gatherRadius = 1;
        
        [Header("Prefabs")]
        public List<PrefabForItem> prefabs;
        public GameObject defaultPrefab;

        public GameObject GetPrefab(Item item)
        {
            GameObject prefab = prefabs.FirstOrDefault(i => i.item.itemName == item.itemName)?.prefab;
            if (prefab)
            {
                return prefab;
            }
            
            return defaultPrefab;
        }
    }

    [Serializable]
    public class PrefabForItem
    {
        public Item item;
        public GameObject prefab;
    }
}
