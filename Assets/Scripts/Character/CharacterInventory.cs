using System;
using System.Collections.Generic;
using Items;

namespace Character
{
    [Serializable]
    public class CharacterInventory
    {
        public List<InventoryLine> lines;
    }

    [Serializable]
    public class InventoryLine
    {
        public ItemState item;
        public int count;
    }
}
