using System.Collections.Generic;
using System.Linq;
using Items;
using UnityEngine;

namespace Character
{
    public class CharacterItemsDetector: MonoBehaviour
    {
        public List<GameItem> itemsNearby;

        [Header("Config")]
        public bool highlightClosestItem;

        private GameItem _closestItem;

        void FixedUpdate()
        {
            if (itemsNearby == null || !GameStateManager.Current)
            {
                return;
            }

            Vector3 playerPosition = GameStateManager.Current.player.position;
            GameItem closestItem = itemsNearby.OrderBy(i => Vector3.Distance(i.state.position, playerPosition)).FirstOrDefault();

            if (closestItem != _closestItem)
            {
                if (highlightClosestItem)
                {
                    if (_closestItem)
                    {
                        _closestItem.Unhighlight();
                    }

                    if (closestItem)
                    {
                        closestItem.Highlight();
                    }
                }

                _closestItem = closestItem;
            }
        }
        
        void OnTriggerEnter(Collider other)
        {
            GameItem gameItem = other.gameObject.GetComponentInParent<GameItem>();
            if (!gameItem)
            {
                return;
            }

            itemsNearby ??= new List<GameItem>();

            itemsNearby.Add(gameItem);
        }
        
        void OnTriggerExit(Collider other)
        {
            GameItem gameItem = other.gameObject.GetComponentInParent<GameItem>();
            if (!gameItem)
            {
                return;
            }

            itemsNearby ??= new List<GameItem>();

            itemsNearby.Remove(gameItem);
        }
    }
}
