using UnityEngine;

namespace Map.Generation.RandomGenerator
{
    public class PerlinRandomGenerator : IRandomGenerator
    {
        private readonly int _seed;
        private readonly Vector2 _scale;
        private readonly Vector2 _offset;

        public PerlinRandomGenerator(int seed) : this(seed, Vector2.one, Vector2.zero)
        {
        }

        public PerlinRandomGenerator(int seed, Vector2 scale, Vector2 offset)
        {
            _seed = seed % 1000;
            _scale = scale;
            _offset = offset;
        }

        public float Get(int x, int y)
        {
            float scaleX = _scale.x == 0 ? 1 : _scale.x;
            float scaleY = _scale.y == 0 ? 1 : _scale.y;

            float actualX = ((long)x + _seed + _offset.x) / scaleX;
            float actualY = ((long)y + _seed + _offset.y) / scaleY;

            float result = Mathf.Clamp(Mathf.PerlinNoise(actualX, actualY), 0, 1);
            return result;
        }
    }
}
