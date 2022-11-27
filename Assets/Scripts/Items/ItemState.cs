using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Items
{
    [Serializable]
    public class ItemState: IEquatable<ItemState>
    {
        [SerializeField]
        private string guid;

        public Item item;
        public Vector3 position;
        
        [Tooltip("Chunk where this item is stored. This might be different than the chunk corresponding to `position` because the item might have moved")]
        public Vector2Int chunk;

        [Header("Render")]
        public uint variant;

        public bool newlySpawned;

        public ItemState(Item item)
        {
            guid = Guid.NewGuid().ToString();
            this.item = item;

            variant = (uint)Random.Range(int.MinValue, int.MaxValue);
            newlySpawned = true;
        }

        public bool Equals(ItemState other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return guid == other.guid;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((ItemState)obj);
        }

        public override int GetHashCode()
        {
            return (guid != null ? guid.GetHashCode() : 0);
        }
    }
}
