using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Resource
{
    [Serializable]
    public class ResourceState
    {
        public UnityEvent<int> onProduce = new();
        public UnityEvent<int> onConsume = new();
        
        [SerializeField]
        private List<int> resourceQuantities = new();

        public int GetQuantity(ResourceType resource)
        {
            int index = GetResourceIndex(resource);
            return index < resourceQuantities.Count ? resourceQuantities[index] : 0;
        }

        public void Produce(ResourceType resource, int quantity)
        {
            int index = GetResourceIndex(resource);
            CreateResourceIfNecessary(resource);

            resourceQuantities[index] += quantity;

            onProduce.Invoke(quantity);
        }

        public void Consume(ResourceType resource, int quantity)
        {
            int index = GetResourceIndex(resource);
            CreateResourceIfNecessary(resource);

            if (resourceQuantities[index] < quantity)
            {
                throw new InvalidOperationException("Not enough resources");
            }

            resourceQuantities[index] -= quantity;
            
            onConsume.Invoke(quantity);
        }
        
        private int GetResourceIndex(ResourceType resource)
        {
            return (int)resource;
        }

        private void CreateResourceIfNecessary(ResourceType resource)
        {
            int index = GetResourceIndex(resource);
            while (index >= resourceQuantities.Count)
            {
                resourceQuantities.Add(0);
            }
        }
    }
}
