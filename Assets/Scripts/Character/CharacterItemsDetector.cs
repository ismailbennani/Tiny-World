using System.Collections.Generic;
using System.Linq;
using Items;
using UnityEngine;
using UnityEngine.Events;

namespace Character
{
    public class CharacterItemsDetector: MonoBehaviour
    {
        public UnityEvent<GameItem> onClosestItemChange = new();
        
        public GameItem closestItem;
        public List<GameItem> itemsNearby;

        [Header("Config")]
        public bool highlightClosestItem;


        void FixedUpdate()
        {
            if (itemsNearby == null || !GameStateManager.Current)
            {
                return;
            }

            itemsNearby.RemoveAll(i => !i);

            Vector3 playerPosition = GameStateManager.Current.player.position;
            GameItem newClosestItem = itemsNearby.OrderBy(i => Vector3.Distance(i.transform.position, playerPosition)).FirstOrDefault();

            if (newClosestItem != closestItem)
            {
                if (highlightClosestItem)
                {
                    if (closestItem)
                    {
                        closestItem.Unhighlight();
                    }

                    if (newClosestItem)
                    {
                        newClosestItem.Highlight();
                    }
                }

                onClosestItemChange?.Invoke(newClosestItem);
                closestItem = newClosestItem;
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
