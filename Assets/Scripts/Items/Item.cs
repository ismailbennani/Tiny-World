using UnityEngine;

namespace Items
{
    [CreateAssetMenu(menuName = "Custom/Item")]
    public class Item: ScriptableObject
    {
        public string itemName;
        public string itemDescription;

        public bool stackable;

        public override string ToString()
        {
            return $"{itemName}";
        }
    }
}
