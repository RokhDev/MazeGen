
namespace MazeGen
{
    interface ICellFinder
    {
        /// <summary>
        /// Finds cell in given coordinates, returns null if it fails.
        /// </summary>
        /// <param name="x">Target cell's position in x</param>
        /// <param name="y">Target cell's position in y</param>
        /// <returns>The cell in the given coordinates if it's found, null if it wasn't</returns>
        public Cell FindCell(int x, int y);

        /// <summary>
        /// Returns the maximum position in X dimension.
        /// </summary>
        public int GetMaxXPos();

        /// <summary>
        /// Returns the maximum position in Y dimension.
        /// </summary>
        public int GetMaxYPos();
    }
}
