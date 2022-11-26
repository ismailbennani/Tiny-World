using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public static class EnumerableExtensions
    {
        public static T ChooseRandomly<T>(this IEnumerable<T> enumerable)
        {
            T[] array = enumerable as T[] ?? enumerable.ToArray();
            int count = array.Length;
            
            if (count <= 0)
            {
                return default;
            }

            int index = Random.Range(0, count);
            return array.ElementAt(index);
        }
    }
}
