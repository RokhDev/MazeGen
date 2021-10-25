
namespace MazeGen
{
    /// <summary>
    /// Interface to implement whatever random number generator you like.
    /// </summary>
    interface IRng
    {
        /// <summary>
        /// Returns a random int between min and max.
        /// </summary>
        /// <param name="min">Min number (inclusive)</param>
        /// <param name="max">Max number (exclusive)</param>
        /// <returns></returns>
        public int GetRandomInt(int min, int max);

        /// <summary>
        /// Returns a random int between 0 and max.
        /// </summary>
        /// <param name="max">Max number (exclusive)</param>
        /// <returns></returns>
        public int GetRandomInt(int max);

        /// <summary>
        /// Returns a random int between 0 and int.MaxValue (exclusive).
        /// </summary>
        /// <returns></returns>
        public int GetRandomInt();
    }
}
