using System;
using Character.Inventory;
using UnityEngine;

namespace Character
{
    [Serializable]
    public class CharacterState
    {
        public CharacterConfig config;
        public Vector3 position;
        public bool sprint;

        public InventoryState inventoryState;

        [Header("Computed state")]
        public Vector2Int chunk;
        public Vector2Int tile;

        public CharacterState(CharacterConfig config)
        {
            this.config = config;
        }
    }
}
