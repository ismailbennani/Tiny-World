using System;
using UnityEngine;

namespace Character
{
    [Serializable]
    public class CharacterState
    {
        public CharacterConfig config;
        public Vector3 position;

        [Header("Computed state")]
        public Vector2Int playerChunk;
        public Vector2Int playerTile;
    }
}
