using UnityEngine;

namespace Items
{
    [CreateAssetMenu(menuName = "Custom/Item")]
    public class Item: ScriptableObject
    {
        public string itemName;
        public string itemDescription;

        public override string ToString()
        {
            return $"{itemName}";
        }
    }
}
