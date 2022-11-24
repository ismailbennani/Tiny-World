using System;
using Resource;
using UnityEngine;
using UnityEngine.Events;

namespace Map.Tile
{
    [Serializable]
    public class TileState
    {
        public UnityEvent<int> onConsume = new();
        public UnityEvent onDepleted = new();
        
        public TileConfig config;

        public Vector2Int position;
        
        [Range(0, 3)]
        public int rotation;

        [Header("Resource")]
        public int resourceQuantity;

        public bool HasResource => config.resource != ResourceType.None && resourceQuantity > 0;

        public TileState(TileConfig config, Vector2Int position, int rotation)
        {
            this.config = config;
            this.position = position;
            this.rotation = Mathf.Clamp(rotation, 0, 3);

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
}
