using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(menuName = "Custom/Items config")]
    public class ItemsRuntimeConfig : ScriptableObject
    {
        public GameItem baseItem;

        [Header("Config")]
        public float gatherRadius = 1;

        [Header("Prefabs")]
        public List<PrefabsForItem> prefabsByItem;
        public GameObject defaultPrefab;

        public GameObject GetPrefab(ItemState state)
        {
            GameObject[] prefabs = prefabsByItem.FirstOrDefault(i => i.item == state.item)?.prefabs;
            if (prefabs != null && prefabs.Length > 0)
            {
                GameObject prefab = prefabs[state.variant % prefabs.Length];
                if (prefab)
                {
                    return prefab;
                }
            }

            return defaultPrefab;
        }

        public GameObject GetPrefab(Item item)
        {
            return GetPrefab(new ItemState(item));
        }
    }

    [Serializable]
    public class PrefabsForItem
    {
        public Item item;
        public GameObject[] prefabs;
    }
}
