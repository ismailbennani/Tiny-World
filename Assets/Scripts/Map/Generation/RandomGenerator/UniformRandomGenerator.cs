using System;

namespace Map.Generation.RandomGenerator
{
    public class UniformRandomGenerator: IRandomGenerator
    {
        private Random _random;
        
        public UniformRandomGenerator(int seed)
        {
            _random = new Random(seed);
        }

        public float Get(int x, int y)
        {
            return (float)_random.NextDouble();
        }
    }
}
