using System;
using System.Collections.Generic;
using Items;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Map.Tile
{
    [Serializable]
    public class TileState
    {
        public UnityEvent<Item[]> onLoot = new();
        public UnityEvent onDepleted = new();

        public TileConfig config;
        public GenerationConfig generationConfig;

        public Vector2Int position;

        [Header("Resource")]
        public int remainingLootAttempts;

        public bool IsLootable => config.lootTable != null && remainingLootAttempts > 0;

        public TileState(TileConfig config, Vector2Int position)
        {
            this.config = config;
            this.position = position;

            generationConfig = new GenerationConfig
            {
                platformRotation = (uint)Random.Range(int.MinValue, int.MaxValue),
                platformVariant = (uint)Random.Range(int.MinValue, int.MaxValue),
                resourceRotation = (uint)Random.Range(int.MinValue, int.MaxValue),
                resourceVariant = (uint)Random.Range(int.MinValue, int.MaxValue),
            };

            if (this.config.tileResource != TileResourceType.None)
            {
                remainingLootAttempts = Random.Range(config.nLoots.x, config.nLoots.y + 1);
            }
        }

        public int Loot(int quantity)
        {
            if (remainingLootAttempts <= 0)
            {
                throw new InvalidOperationException($"Cannot loot tile {position}");
            }

            int actualQuantity = quantity <= 0 ? remainingLootAttempts : Mathf.Min(remainingLootAttempts, quantity);

            remainingLootAttempts -= actualQuantity;

            List<Item> loots = new();
            for (int i = 0; i < actualQuantity; i++)
            {
                Item loot = config.lootTable.Loot();
                if (loot)
                {
                    loots.Add(loot);
                }
            }

            onLoot.Invoke(loots.ToArray());

            if (remainingLootAttempts <= 0)
            {
                onDepleted.Invoke();
            }

            return actualQuantity;
        }
    }

    [Serializable]
    public class GenerationConfig
    {
        public uint platformRotation;
        public uint platformVariant;
        public uint resourceRotation;
        public uint resourceVariant;
    }
}
