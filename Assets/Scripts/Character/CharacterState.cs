using System;
using UnityEngine;

namespace Character
{
    [Serializable]
    public class CharacterState
    {
        public CharacterConfig config;
        public Vector3 position;
        public bool sprinting;

        public CharacterInventory inventory;

        [Header("Computed state")]
        public Vector2Int chunk;
        public Vector2Int tile;

        public CharacterState(CharacterConfig config)
        {
            this.config = config;
        }
    }
}
