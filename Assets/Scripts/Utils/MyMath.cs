using UnityEngine;

namespace Utils
{
    public static class MyMath
    {
        public static int GetIndex(Vector2Int position, Vector2Int size)
        {
            return GetIndex(position.x, position.y, size);
        }

        public static int GetIndex(int x, int y, Vector2Int size)
        {
            return GetIndex(x, y, size.x, size.y);
        }

        public static int GetIndex(int x, int y, int width, int height)
        {
            return x * height + y;
        }

        public static (int, int) GetCoords(int index, Vector2Int size)
        {
            return GetCoords(index, size.x, size.y);
        }

        public static (int, int) GetCoords(int index, int width, int height)
        {
            return (index / height, index % height);
        }
    }
}
