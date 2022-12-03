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
        
        public static bool Drop(InventoryState inventoryState, Item item, int indexHint = -1)
        {
            GameState gameState = GameStateManager.Current;
            if (!gameState)
            {
                return false;
            }

            PlayerState player = gameState.player;
            if (player == null)
            {
                return false;
            }
            
            GameMap map = GameMap.Instance;
            if (!map)
            {
                return false;
            }

            inventoryState.DropItem(item, indexHint);
            map.SpawnItem(item, player.position);

            return true;
        }
    }
}
