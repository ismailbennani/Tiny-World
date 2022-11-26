using System;
using Resource;
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

        public bool HasResource => config.resource != ResourceType.None && resourceQuantity > 0;

        public TileState(TileConfig config, Vector2Int position)
        {
            this.config = config;
            this.position = position;

            generationConfig = new GenerationConfig
            {
                rotation = Random.Range(0, 4),
                platformVariant = Random.Range(0, config.nPlatformVariants + 1),
                resourceVariant = Random.Range(0, config.nResourceVariants + 1),
            };

            if (this.config.resource != ResourceType.None)
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
        [Range(0, 3)]
        public int rotation;
        public int platformVariant;
        public int resourceVariant;
    }
}
