
namespace MazeGen
{
    /// <summary>
    /// Class for the cells making up the maze.
    /// </summary>
    class Cell
    {
        public int x { get; private set; }
        public int y { get; private set; }
        public bool upperWall { get; private set; }
        public bool lowerWall { get; private set; }
        public bool rightWall { get; private set; }
        public bool leftWall { get; private set; }
        public bool visited { get; private set; }

        private enum WallPosition
        {
            Upper,
            Lower,
            Right,
            Left
        }

        /// <summary>
        /// Constructor for Cell.
        /// </summary>
        /// <param name="x">Position in X dimension</param>
        /// <param name="y">Position in Y dimension</param>
        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
            upperWall = true;
            lowerWall = true;
            rightWall = true;
            leftWall = true;
            visited = false;
        }

        /// <summary>
        /// Knocks down a specific wall.
        /// </summary>
        /// <param name="wallPosition">Position of the wall to knock</param>
        private void KnockDownWall(WallPosition wallPosition)
        {
            if (wallPosition == WallPosition.Left && leftWall)
            {
                leftWall = false;
            }
            else if (wallPosition == WallPosition.Lower)
            {
                lowerWall = false;
            }
            else if (wallPosition == WallPosition.Right)
            {
                rightWall = false;
            }
            else if (wallPosition == WallPosition.Upper)
            {
                upperWall = false;
            }
        }

        /// <summary>
        /// Knocks down a specific wall if able, using a number from 0 to 3.
        /// </summary>
        /// <param name="n">Number that must be from 0 to 3</param>
        /// <param name="cellFinder">Object implementing ICellFinder interface</param>
        private void KnockDownWallByNumber(int n, ICellFinder cellFinder)
        {
            if (n == 0)
            {
                if (leftWall && x > 0)
                {
                    leftWall = false;
                    Cell target = cellFinder.FindCell(x - 1, y);
                    target.KnockDownWall(WallPosition.Right);
                }
                else if (rightWall && x < cellFinder.GetMaxXPos())
                {
                    rightWall = false;
                    Cell target = cellFinder.FindCell(x + 1, y);
                    target.KnockDownWall(WallPosition.Left);
                }
                else if (lowerWall && y > 0)
                {
                    lowerWall = false;
                    Cell target = cellFinder.FindCell(x, y - 1);
                    target.KnockDownWall(WallPosition.Upper);
                }
                else if (upperWall && y < cellFinder.GetMaxYPos())
                {
                    upperWall = false;
                    Cell target = cellFinder.FindCell(x, y + 1);
                    target.KnockDownWall(WallPosition.Lower);
                }
            }
            else if (n == 1)
            {
                if (upperWall && y < cellFinder.GetMaxYPos())
                {
                    upperWall = false;
                    Cell target = cellFinder.FindCell(x, y + 1);
                    target.KnockDownWall(WallPosition.Lower);
                }
                else if (leftWall && x > 0)
                {
                    leftWall = false;
                    Cell target = cellFinder.FindCell(x - 1, y);
                    target.KnockDownWall(WallPosition.Right);
                }
                else if (rightWall && x < cellFinder.GetMaxXPos())
                {
                    rightWall = false;
                    Cell target = cellFinder.FindCell(x + 1, y);
                    target.KnockDownWall(WallPosition.Left);
                }
                else if (lowerWall && y > 0)
                {
                    lowerWall = false;
                    Cell target = cellFinder.FindCell(x, y - 1);
                    target.KnockDownWall(WallPosition.Upper);
                }
            }
            else if (n == 2)
            {
                if (lowerWall && y > 0)
                {
                    lowerWall = false;
                    Cell target = cellFinder.FindCell(x, y - 1);
                    target.KnockDownWall(WallPosition.Upper);
                }
                else if (upperWall && y < cellFinder.GetMaxYPos())
                {
                    upperWall = false;
                    Cell target = cellFinder.FindCell(x, y + 1);
                    target.KnockDownWall(WallPosition.Lower);
                }
                else if (leftWall && x > 0)
                {
                    leftWall = false;
                    Cell target = cellFinder.FindCell(x - 1, y);
                    target.KnockDownWall(WallPosition.Right);
                }
                else if (rightWall && x < cellFinder.GetMaxXPos())
                {
                    rightWall = false;
                    Cell target = cellFinder.FindCell(x + 1, y);
                    target.KnockDownWall(WallPosition.Left);
                }
            }
            else if (n == 3)
            {
                if (rightWall && x < cellFinder.GetMaxXPos())
                {
                    rightWall = false;
                    Cell target = cellFinder.FindCell(x + 1, y);
                    target.KnockDownWall(WallPosition.Left);
                }
                else if (lowerWall && y > 0)
                {
                    lowerWall = false;
                    Cell target = cellFinder.FindCell(x, y - 1);
                    target.KnockDownWall(WallPosition.Upper);
                }
                else if (upperWall && y < cellFinder.GetMaxYPos())
                {
                    upperWall = false;
                    Cell target = cellFinder.FindCell(x, y + 1);
                    target.KnockDownWall(WallPosition.Lower);
                }
                else if (leftWall && x > 0)
                {
                    leftWall = false;
                    Cell target = cellFinder.FindCell(x - 1, y);
                    target.KnockDownWall(WallPosition.Right);
                }
            }
        }

        /// <summary>
        /// Knocks down a wall neighboring target cell.
        /// </summary>
        /// <param name="target">Target neighbor cell</param>
        public void KnockDownWall(Cell target)
        {
            if (x > target.x)
                leftWall = false;
            else if (x < target.x)
                rightWall = false;
            else if (y > target.y)
                lowerWall = false;
            else if (y < target.y)
                upperWall = false;
            visited = true;
        }

        /// <summary>
        /// Knocks down a random wall in the cell.
        /// </summary>
        /// <param name="rng">Object implementing IRng interface</param>
        /// <param name="cellFinder">Object implementing ICellFinder interface</param>
        /// <returns></returns>
        public bool KnockDownRandomWall(IRng rng, ICellFinder cellFinder)
        {
            int numWallsStanding = 0;
            if (leftWall) numWallsStanding++;
            if (rightWall) numWallsStanding++;
            if (lowerWall) numWallsStanding++;
            if (upperWall) numWallsStanding++;
            if (numWallsStanding == 0) return false;

            int rn = rng.GetRandomInt(numWallsStanding);
            KnockDownWallByNumber(rn, cellFinder);
            return true;
        }

        /// <summary>
        /// Sets 'visited' to 'false'.
        /// </summary>
        public void RemoveVisited()
        {
            visited = false;
        }

        /// <summary>
        /// Sets 'visited' to 'true'.
        /// </summary>
        public void Visit()
        {
            visited = true;
        }
    }
}
