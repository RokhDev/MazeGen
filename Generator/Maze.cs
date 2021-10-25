using System.Collections.Generic;

namespace MazeGen
{
    class Maze : ICellFinder
    {
        public int width { get; private set; }
        public int height { get; private set; }
        public int startX { get; private set; }
        public int startY { get; private set; }
        public int goalX { get; private set; }
        public int goalY { get; private set; }

        private Cell[,] cells;

        /// <summary>
        /// Constructor for Maze.
        /// </summary>
        /// <param name="width">Width of the maze</param>
        /// <param name="height">Height of the maze</param>
        public Maze(int width, int height)
        {
            this.width = width;
            this.height = height;
            cells = new Cell[width, height];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    cells[j, i] = new Cell(j, i);
                }
            }
        }

        /// <summary>
        /// Finds cell in given coordinates, returns null if it fails.
        /// </summary>
        /// <param name="x">Target cell's position in x</param>
        /// <param name="y">Target cell's position in y</param>
        /// <returns>The cell in the given coordinates if it's found, null if it wasn't</returns>
        public Cell FindCell(int x, int y)
        {
            if (cells == null ||
                x > cells.GetUpperBound(0) || x < cells.GetLowerBound(0) ||
                y > cells.GetUpperBound(1) || y < cells.GetLowerBound(1))
                return null;

            return cells[x, y];
        }

        /// <summary>
        /// Gets the index of the last element in X dimension.
        /// </summary>
        public int GetMaxXPos()
        {
            return cells.GetUpperBound(0);
        }

        /// <summary>
        /// Gets the index of the last element in Y dimension.
        /// </summary>
        public int GetMaxYPos()
        {
            return cells.GetUpperBound(1);
        }

        /// <summary>
        /// Finds a solution with a minimum amount of steps traveled.
        /// If numSteps is greater than the lenght of the longest solution, 
        /// returns the longest solution.
        /// </summary>
        /// <param name="startX">Starting point in X dimension</param>
        /// <param name="startY">Starting point in Y dimension</param>
        /// <param name="numSteps">Steps required</param>
        /// <param name="rng">Random number generator</param>
        /// <returns></returns>
        public Stack<Cell> FindSolution(int startX, int startY, int numSteps, IRng rng)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    cells[j, i].RemoveVisited();
                }
            }

            this.startX = startX;
            this.startY = startY;
            Stack<Cell> currentPath = new Stack<Cell>();
            List<Cell[]> paths = new List<Cell[]>();
            List<Cell> adjacents = new List<Cell>();
            bool retreating = false;
            currentPath.Push(cells[startX, startY]);
            int steps = 0;
            Cell current;

            while (currentPath.Count > 0)
            {
                current = currentPath.Peek();
                current.Visit();

                FindAdjacents(current, ref adjacents);
                if (adjacents.Count == 0)
                {
                    if (!retreating)
                    {
                        retreating = true;
                        Cell[] newPath = new Cell[currentPath.Count];
                        currentPath.CopyTo(newPath, 0);
                        paths.Add(newPath);
                    }
                    steps--;
                    currentPath.Pop();
                    continue;
                }

                retreating = false;
                steps++;
                if (steps == numSteps)
                {
                    Stack<Cell> fixedPath = new Stack<Cell>();
                    goalX = currentPath.Peek().x;
                    goalY = currentPath.Peek().y;
                    while (currentPath.Count > 0) fixedPath.Push(currentPath.Pop());
                    return fixedPath;
                }
                Cell next = adjacents[rng.GetRandomInt(adjacents.Count)];
                currentPath.Push(next);
            }

            Cell[] chosenPath = paths[0];
            for (int i = 1; i < paths.Count; i++)
            {
                if (paths[i].Length > chosenPath.Length)
                    chosenPath = paths[i];
            }

            Stack<Cell> path = new Stack<Cell>();
            goalX = chosenPath[0].x;
            goalY = chosenPath[0].y;
            for (int i = 0; i < chosenPath.Length; i++)
            {
                path.Push(chosenPath[i]);
            }
            return path;
        }

        /// <summary>
        /// Finds the shortest solution between two points.
        /// </summary>
        /// <param name="startX">Starting point in X dimension</param>
        /// <param name="startY">Starting point in Y dimension</param>
        /// <param name="goalX">Goal point in X dimension</param>
        /// <param name="goalY">Goal point in Y dimension</param>
        /// <param name="heuristicType">Choose between Euclidean or Manhattan</param>
        /// <returns></returns>
        public Stack<Cell> FindSolution(
            int startX,
            int startY,
            int goalX,
            int goalY,
            HeuristicType heuristicType = HeuristicType.Euclidean)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    cells[j, i].RemoveVisited();
                }
            }

            this.startX = startX;
            this.startY = startY;
            List<Cell> adjacents = new List<Cell>();
            Stack<Cell> path = new Stack<Cell>();
            path.Push(cells[startY, startX]);
            Cell current;

            while (path.Count > 0)
            {
                current = path.Peek();
                current.Visit();
                if (current.x == goalX && current.y == goalY) break;
                FindAdjacents(current, ref adjacents);
                if (adjacents.Count == 0)
                {
                    path.Pop();
                    continue;
                }
                Cell next = PickClosestToGoal(adjacents, goalX, goalY, heuristicType);
                path.Push(next);
            }

            Stack<Cell> fixedPath = new Stack<Cell>();
            this.goalX = path.Peek().x;
            this.goalY = path.Peek().y;
            while (path.Count > 0) fixedPath.Push(path.Pop());
            return fixedPath;
        }

        /// <summary>
        /// Gets the maze cells as a 2D array.
        /// </summary>
        /// <returns>A 2D array of cells</returns>
        public Cell[,] GetCells()
        {
            return cells;
        }

        private void FindAdjacents(Cell cell, ref List<Cell> result)
        {
            result.Clear();
            Cell left = cell.leftWall ? null : FindCell(cell.x - 1, cell.y);
            Cell right = cell.rightWall ? null : FindCell(cell.x + 1, cell.y);
            Cell up = cell.upperWall ? null : FindCell(cell.x, cell.y + 1);
            Cell down = cell.lowerWall ? null : FindCell(cell.x, cell.y - 1);

            if (left != null && !left.visited) result.Add(left);
            if (right != null && !right.visited) result.Add(right);
            if (up != null && !up.visited) result.Add(up);
            if (down != null && !down.visited) result.Add(down);
        }

        private Cell PickClosestToGoal(
            List<Cell> adjacents,
            int goalX,
            int goalY,
            HeuristicType heuristicType)
        {
            int distX;
            int distY;
            int chosenNumSteps;
            Cell chosenCell;
            if (heuristicType == HeuristicType.Manhattan)
            {
                distX = System.Math.Abs(goalX - adjacents[0].x);
                distY = System.Math.Abs(goalY - adjacents[0].y);
                chosenNumSteps = distX + distY;
                chosenCell = adjacents[0];

                for (int i = 1; i < adjacents.Count; i++)
                {
                    distX = System.Math.Abs(goalX - adjacents[i].x);
                    distY = System.Math.Abs(goalY - adjacents[i].y);
                    int dist = distX + distY;
                    if (dist < chosenNumSteps)
                    {
                        chosenCell = adjacents[i];
                        chosenNumSteps = dist;
                    }
                }
                return chosenCell;
            }

            distX = System.Math.Abs(goalX - adjacents[0].x);
            distY = System.Math.Abs(goalY - adjacents[0].y);
            float chosenDist = System.MathF.Sqrt(System.MathF.Pow(distX, 2) + System.MathF.Pow(distY, 2));
            chosenCell = adjacents[0];

            for (int i = 1; i < adjacents.Count; i++)
            {
                distX = System.Math.Abs(goalX - adjacents[i].x);
                distY = System.Math.Abs(goalY - adjacents[i].y);
                float dist = System.MathF.Sqrt(System.MathF.Pow(distX, 2) + System.MathF.Pow(distY, 2));
                if (dist < chosenDist)
                {
                    chosenCell = adjacents[i];
                    chosenDist = dist;
                }
            }
            return chosenCell;
        }
    }
}
