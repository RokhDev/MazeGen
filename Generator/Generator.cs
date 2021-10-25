using System.Collections.Generic;

namespace MazeGen
{
    class Generator
    {
        private IRng rng;

        /// <summary>
        /// Constructor for Generator.
        /// </summary>
        public Generator()
        {
            rng = new Rng();
        }

        /// <summary>
        /// Generates a maze randomly within given parameters.
        /// </summary>
        /// <param name="width">Width of the maze</param>
        /// <param name="height">Height of the maze</param>
        /// <param name="startX">Starting cell X</param>
        /// <param name="startY">Starting cell Y</param>
        /// <param name="extraOpenings">Extra openings, if desired</param>
        /// <returns>The generated maze</returns>
        public Maze Generate(int width,
            int height,
            int startX,
            int startY,
            int extraOpenings = 0)
        {
            if (startX < 0) startX = 0;
            else if (startX >= width) startX = width - 1;
            if (startY < 0) startY = 0;
            else if (startY >= height) startY = height - 1;

            Maze maze = new Maze(width, height);
            List<Cell> adjacents = new List<Cell>();
            Stack<Cell> openCells = new Stack<Cell>();
            openCells.Push(maze.FindCell(startY, startX));

            Cell current;
            while (openCells.Count > 0)
            {
                current = openCells.Peek();
                FindAdjacents(current, maze, ref adjacents);
                if (adjacents.Count == 0)
                {
                    openCells.Pop();
                    continue;
                }
                Cell next = adjacents[rng.GetRandomInt(adjacents.Count)];
                current.KnockDownWall(next);
                next.KnockDownWall(current);
                openCells.Push(next);
            }

            if (extraOpenings > 0)
            {
                Stack<int> randomXCoords = new Stack<int>();
                Stack<int> randomYCoords = new Stack<int>();
                Cell[,] mazeCells = maze.GetCells();
                for (int i = 0; i < extraOpenings; i++)
                {
                    randomXCoords.Push(rng.GetRandomInt(width));
                    randomYCoords.Push(rng.GetRandomInt(height));
                }
                for (int i = 0; i < extraOpenings; i++)
                {
                    int randomOpeningX = randomXCoords.Pop();
                    int randomOpeningY = randomYCoords.Pop();
                    mazeCells[randomOpeningX, randomOpeningY].KnockDownRandomWall(rng, maze);
                }
            }
            return maze;
        }

        /// <summary>
        /// Finds the valid adjacent cells of a cell, if any.
        /// </summary>
        /// <param name="cell">Cell of which adjacents will be searched for</param>
        /// <param name="maze">Maze in which the cell belongs</param>
        /// <param name="result">List of cells where the adjacents will be written, if any</param>
        private void FindAdjacents(Cell cell, ICellFinder maze, ref List<Cell> result)
        {
            result.Clear();
            Cell left = maze.FindCell(cell.x - 1, cell.y);
            Cell right = maze.FindCell(cell.x + 1, cell.y);
            Cell up = maze.FindCell(cell.x, cell.y + 1);
            Cell down = maze.FindCell(cell.x, cell.y - 1);

            if (left != null && !left.visited) result.Add(left);
            if (right != null && !right.visited) result.Add(right);
            if (up != null && !up.visited) result.Add(up);
            if (down != null && !down.visited) result.Add(down);
        }
    }
}
