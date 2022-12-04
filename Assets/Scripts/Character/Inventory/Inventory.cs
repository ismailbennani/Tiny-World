using Character.Player;
using Items;
using Map;

namespace Character.Inventory
{
    public static class Inventory
    {
        public static bool Take(InventoryState inventoryState, ItemState item)
        {
            GameState gameState = GameStateManager.Current;
            if (!gameState)
            {
                return false;
            }

            MapState map = gameState.map;
            if (map == null)
            {
                return false;
            }

            if (map.RemoveItem(item))
            {
                inventoryState.TakeItem(item);
            }

            return true;
        }
        
        public static int Drop(InventoryState inventoryState, Item item, int count = 1, int indexHint = -1)
        {
            GameState gameState = GameStateManager.Current;
            if (!gameState)
            {
                return -1;
            }

            PlayerState player = gameState.player;
            if (player == null)
            {
                return -1;
            }
            
            GameMap map = GameMap.Instance;
            if (!map)
            {
                return -1;
            }

            int actualCount = inventoryState.DropItem(item, count, indexHint);

            for (int i = 0; i < actualCount; i++)
            {
                map.SpawnItem(item, player.position);
            }

            return count;
        }
    }
}
