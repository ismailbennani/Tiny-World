using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using UnityEngine;
using UnityEngine.Events;

namespace Character.Inventory
{
    [Serializable]
    public class InventoryState
    {
        public UnityEvent<InventoryLine> onChange = new();

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

            matchingLine.onChange?.Invoke();
            onChange.Invoke(matchingLine);
        }

        /// <param name="item"></param>
        /// <param name="position"></param>
        /// <param name="indexHint">If positive, and if item at that index is the same as item, will drop that one instead of the first one</param>
        public int DropItem(Item item, int count = 1, int indexHint = -1)
        {
            InventoryLine matchingLine;
            if (indexHint >= 0 && indexHint < lines.Count && lines[indexHint].item == item)
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

            int actualCount = Mathf.Min(count, matchingLine.count);

            matchingLine.count -= actualCount;
            if (matchingLine.count <= 0)
            {
                lines.Remove(matchingLine);
            }

            matchingLine.onChange?.Invoke();
            onChange.Invoke(matchingLine);

            return actualCount;
        }
    }

    [Serializable]
    public class InventoryLine
    {
        public Item item;
        public int count;
        public UnityEvent onChange = new();
    }
}
