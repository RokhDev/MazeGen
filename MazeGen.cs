using System;
using System.Windows.Forms;
using System.Drawing;

namespace MazeGen
{
    public partial class MazeGen : Form
    {
        private Generator generator;
        private Maze maze;
        private Rng rng;
        private Graphics g;
        private Pen p;
        private SolidBrush b;
        private int mazeWidth;
        private int mazeHeight;
        private int startX;
        private int startY;
        private int goalX;
        private int goalY;
        private int extraOpenings;
        private int minSteps;
        private System.Collections.Generic.Stack<Cell> solution;
        private int cellSize;

        public MazeGen()
        {
            InitializeComponent();

            generator = new Generator();
            rng = new Rng();
            g = panel1.CreateGraphics();
            p = new Pen(Color.DarkBlue, 2.0f);
            b = new SolidBrush(Color.White);
            g.Clear(Color.White);
            mazeWidth = 25;
            textBox1.Text = mazeWidth.ToString();
            mazeHeight = 25;
            textBox2.Text = mazeHeight.ToString();
            startX = 0;
            textBox3.Text = (startX + 1).ToString();
            startY = 0;
            textBox4.Text = (startY + 1).ToString();
            extraOpenings = 0;
            textBox5.Text = extraOpenings.ToString();
            goalX = 0;
            textBox8.Text = (goalX + 1).ToString();
            goalY = 0;
            textBox9.Text = (goalY + 1).ToString();
            minSteps = mazeWidth * mazeHeight;
            textBox10.Text = minSteps.ToString();
        }

        private void Generate_Click(object sender, EventArgs e)
        {
            maze = generator.Generate(mazeWidth,
                mazeHeight,
                startX,
                startY,
                extraOpenings);
            Cell[,] cells = maze.GetCells();

            DrawMaze(g, b, p, cells);
        }

        private void FindPath_Click(object sender, EventArgs e)
        {
            if (maze == null) return;
            Cell[,] cells = maze.GetCells();
            solution = maze.FindSolution(startX, startY, goalX, goalY);
            if (solution != null) DrawSolution(g, b, p, cells, cellSize);
        }

        private void GeneratePathBySteps_Click(object sender, EventArgs e)
        {
            if (maze == null) return;
            Cell[,] cells = maze.GetCells();
            solution = maze.FindSolution(startX, startY, minSteps, rng);
            if (solution != null) DrawSolution(g, b, p, cells, cellSize);
        }

        private void RandomizeStartpos_Click(object sender, EventArgs e)
        {
            startX = rng.GetRandomInt(0, mazeWidth);
            startY = rng.GetRandomInt(0, mazeHeight);
            textBox3.Text = (startX + 1).ToString();
            textBox4.Text = (startY + 1).ToString();
        }

        private void SetStandardExtraOpenings_Click(object sender, EventArgs e)
        {
            extraOpenings = mazeWidth + mazeHeight;
            textBox5.Text = extraOpenings.ToString();
        }

        private void RandomizeGoal_Click(object sender, EventArgs e)
        {
            goalX = rng.GetRandomInt(0, mazeWidth);
            goalY = rng.GetRandomInt(0, mazeHeight);
            textBox8.Text = (goalX + 1).ToString();
            textBox9.Text = (goalY + 1).ToString();
        }

        private void DrawMaze(Graphics g, SolidBrush b, Pen p, Cell[,] cells)
        {
            panel1.Width = 512;
            panel1.Height = 512;
            g.Clear(Color.White);
            p.Color = Color.DarkBlue;
            b.Color = Color.White;
            p.Width = 2.0f;

            if (mazeWidth > mazeHeight)
                cellSize = panel1.Width / mazeWidth;
            else
                cellSize = panel1.Width / mazeHeight;
            panel1.Width = cellSize * mazeWidth + 2;
            panel1.Height = cellSize * mazeHeight + 2;
            g.FillRectangle(b, 0, 0, panel1.Width, panel1.Height);

            int x = 1;
            int y = panel1.Height - cellSize - 1;

            for (int i = 0; i < mazeHeight; i++)
            {
                for (int j = 0; j < mazeWidth; j++)
                {
                    DrawCell(g, p, cells[j, i], cellSize, x, y, Color.DarkBlue);
                    x += cellSize;
                }
                x = 1;
                y -= cellSize;
            }
        }

        private void DrawSolution(Graphics g, SolidBrush b, Pen p, Cell[,] cells, int cellSize)
        {
            DrawMaze(g, b, p, cells);

            Cell cell = null;
            int x = 0;
            int y = 0;
            int solutionOriginalCount = solution.Count;
            int startX = 0;
            int startY = 0;
            int endX = 0;
            int endY = 0;
            Cell start = null;
            Cell end = null;
            while (solution.Count > 0)
            {
                Cell last = cell;
                cell = solution.Pop();
                int lastx = x;
                int lasty = y;
                x = cell.x * cellSize;
                y = panel1.Height - (cellSize * cell.y + cellSize);
                if (last != null)
                {
                    DrawLine(g,
                        p,
                        lastx + (cellSize / 2),
                        lasty + (cellSize / 2),
                        x + (cellSize / 2),
                        y + (cellSize / 2),
                        Color.LightBlue,
                        cellSize / 3);
                }
                if (solution.Count == solutionOriginalCount - 1)
                {
                    start = cell;
                    startX = x;
                    startY = y;
                }
                else if (solution.Count == 0)
                {
                    end = cell;
                    endX = x;
                    endY = y;
                }
            }
            if (start != null) DrawFilledCircle(g, b, cellSize, startX, startY, Color.Green, cellSize / 6);
            if (end != null) DrawFilledCircle(g, b, cellSize, endX, endY, Color.Red, cellSize / 6);
        }

        private void DrawCell(Graphics g, Pen p, Cell cell, int cellSize, int x, int y, Color color)
        {
            p.Color = color;
            if (cell.upperWall)
                g.DrawLine(p, x, y, x + cellSize, y);
            if (cell.lowerWall)
                g.DrawLine(p, x, y + cellSize, x + cellSize, y + cellSize);
            if (cell.rightWall)
                g.DrawLine(p, x + cellSize, y, x + cellSize, y + cellSize);
            if (cell.leftWall)
                g.DrawLine(p, x, y, x, y + cellSize);
        }

        private void DrawLine(Graphics g, Pen p, int x1, int y1, int x2, int y2, Color color, float width = 2.0f)
        {
            p.Color = color;
            p.Width = width;
            g.DrawLine(p, x1, y1, x2, y2);
        }

        private void DrawFilledCircle(Graphics g, Brush p, int cellSize, int x, int y, Color color, int offset = 0)
        {
            b.Color = color;
            Rectangle rect = new Rectangle(
                x + offset,
                y + offset,
                cellSize - offset * 2,
                cellSize - offset * 2);
            g.FillEllipse(b, rect);
        }

        private void MazeSizeXTextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (int.TryParse(textBox.Text, out int num))
            {
                if (num < 5)
                {
                    textBox.Text = "5";
                    mazeWidth = 5;
                }
                else if (num > 100)
                {
                    textBox.Text = "100";
                    mazeWidth = 100;
                }
                else
                {
                    mazeWidth = num;
                }
            }
            else
            {
                textBox.Text = mazeWidth.ToString();
            }
        }

        private void MazeSizeYTextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (int.TryParse(textBox.Text, out int num))
            {
                if (num < 5)
                {
                    textBox.Text = "5";
                    mazeHeight = 5;
                }
                else if (num > 100)
                {
                    textBox.Text = "100";
                    mazeHeight = 100;
                }
                else
                {
                    mazeHeight = num;
                }
            }
            else
            {
                textBox.Text = mazeWidth.ToString();
            }
        }

        private void StartPosXTextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (int.TryParse(textBox.Text, out int num))
            {
                if (num < 1)
                {
                    textBox.Text = "1";
                    startX = 0;
                }
                else if (num > mazeWidth)
                {
                    textBox.Text = mazeWidth.ToString();
                    startX = mazeWidth - 1;
                }
                else
                {
                    startX = num - 1;
                }
            }
            else
            {
                textBox.Text = startX.ToString();
            }
        }

        private void StartPosYTextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (int.TryParse(textBox.Text, out int num))
            {
                if (num < 1)
                {
                    textBox.Text = "1";
                    startY = 0;
                }
                else if (num > mazeWidth)
                {
                    textBox.Text = mazeWidth.ToString();
                    startY = mazeWidth - 1;
                }
                else
                {
                    startY = num - 1;
                }
            }
            else
            {
                textBox.Text = startY.ToString();
            }
        }

        private void ExtraOpeningsTextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int maxExtraOpenings = mazeWidth * mazeHeight;
            if (int.TryParse(textBox.Text, out int num))
            {
                if (num < 0)
                {
                    textBox.Text = "0";
                    extraOpenings = 0;
                }
                else if (num > maxExtraOpenings)
                {
                    textBox.Text = maxExtraOpenings.ToString();
                    extraOpenings = maxExtraOpenings;
                }
                else
                {
                    extraOpenings = num;
                }
            }
            else
            {
                textBox.Text = extraOpenings.ToString();
            }
        }

        private void GoalXTextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (int.TryParse(textBox.Text, out int num))
            {
                if (num < 1)
                {
                    textBox.Text = "1";
                    goalX = 0;
                }
                else if (num > mazeWidth)
                {
                    textBox.Text = mazeWidth.ToString();
                    goalX = mazeWidth - 1;
                }
                else
                {
                    goalX = num - 1;
                }
            }
            else
            {
                textBox.Text = goalX.ToString();
            }
        }

        private void GoalYTextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (int.TryParse(textBox.Text, out int num))
            {
                if (num < 1)
                {
                    textBox.Text = "1";
                    goalY = 0;
                }
                else if (num > mazeWidth)
                {
                    textBox.Text = mazeWidth.ToString();
                    goalY = mazeWidth - 1;
                }
                else
                {
                    goalY = num - 1;
                }
            }
            else
            {
                textBox.Text = goalY.ToString();
            }
        }

        private void MinStepsTextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int maxSteps = mazeWidth * mazeHeight;
            if (int.TryParse(textBox.Text, out int num))
            {
                if (num < 0)
                {
                    textBox.Text = "0";
                    minSteps = 0;
                }
                else if (num > maxSteps)
                {
                    textBox.Text = maxSteps.ToString();
                    minSteps = maxSteps;
                }
                else
                {
                    minSteps = num;
                }
            }
            else
            {
                textBox.Text = minSteps.ToString();
            }
        }
    }
}
