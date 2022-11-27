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
    }
}
