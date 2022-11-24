using System;
using Resource;

namespace Map.Tile
{
    [Serializable]
    public class PrefabForResourceType
    {
        public ResourceType type;
        public MapTileResource prefab;
    }
}
