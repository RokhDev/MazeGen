
namespace MazeGen
{
    class Rng : IRng
    {
        private System.Random rng;

        public Rng()
        {
            rng = new System.Random();
        }

        public int GetRandomInt(int min, int max)
        {
            return rng.Next(min, max);
        }

        public int GetRandomInt(int max)
        {
            return rng.Next(max);
        }

        public int GetRandomInt()
        {
            return rng.Next();
        }
    }
}
