using System;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(menuName = "Custom/Item")]
    public class Item: ScriptableObject, IEquatable<Item>
    {
        public string itemName;
        public string itemDescription;
        public Sprite sprite;

        public bool stackable;

        public override string ToString()
        {
            return $"{itemName}";
        }

        public bool Equals(Item other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return base.Equals(other) && itemName == other.itemName;
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
            return Equals((Item)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), itemName);
        }

        public static bool operator ==(Item left, Item right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Item left, Item right)
        {
            return !Equals(left, right);
        }
    }
}
