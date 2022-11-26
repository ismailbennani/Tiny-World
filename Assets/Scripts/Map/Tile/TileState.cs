using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Map.Tile
{
    [Serializable]
    public class TileState
    {
        public UnityEvent<int> onConsume = new();
        public UnityEvent onDepleted = new();
        
        public TileConfig config;
        public GenerationConfig generationConfig;

        public Vector2Int position;

        [Header("Resource")]
        public int resourceQuantity;

        public bool HasResource => config.tileResource != TileResourceType.None && resourceQuantity > 0;

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
                resourceQuantity = 10;
            }
        }

        public int ConsumeResource(int quantity)
        {
            if (resourceQuantity <= 0)
            {
                throw new InvalidOperationException("Not enough resources");
            }

            int actualQuantity = Mathf.Min(resourceQuantity, quantity);
            
            resourceQuantity -= actualQuantity;
            
            onConsume.Invoke(actualQuantity);

            if (resourceQuantity <= 0)
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
