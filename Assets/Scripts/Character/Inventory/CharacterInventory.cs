using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using Map;
using UnityEngine;

namespace Character.Inventory
{
    [Serializable]
    public class CharacterInventory
    {
        public List<InventoryLine> lines;

        public void TakeItem(ItemState item)
        {
            InventoryLine matchingLine = lines.FirstOrDefault(l => l.item == item.item);
            if (matchingLine == null || !matchingLine.item.stackable)
            {
                matchingLine = new InventoryLine { item = item.item, count = 0 };
                lines.Add(matchingLine);
            }

            matchingLine.count++;
        }

        /// <param name="item"></param>
        /// <param name="position"></param>
        /// <param name="indexHint">If positive, and if item at that index is the same as item, will drop that one instead of the first one</param>
        public void DropItem(Item item, Vector3 position, int indexHint = -1)
        {
            InventoryLine matchingLine;
            if (indexHint > 0 && lines[indexHint].item == item)
            {
                matchingLine = lines[indexHint];
            }
            else
            {
                matchingLine = lines.FirstOrDefault(l => l.item == item);
            }

            if (matchingLine == null)
            {
                throw new InvalidOperationException($"Cannot find item {item}");
            }

            matchingLine.count--;
            if (matchingLine.count <= 0)
            {
                lines.Remove(matchingLine);
            }

            GameMap map = GameMap.Instance;
            if (!map)
            {
                return;
            }

            map.SpawnItem(item, position);
        }
    }

    [Serializable]
    public class InventoryLine
    {
        public Item item;
        public int count;
    }
}
