using System;
using System.Collections.Generic;

namespace TetrisFilling
{
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
        public override bool Equals(object obj)
        {
            if (obj is Point c)
            {
                return c.X == X && c.Y == Y;
            }
            else
                return false;
        }
        public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
        public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);
        public static bool operator ==(Point c1, Point c2) => c1.Equals(c2);
        public static bool operator !=(Point c1, Point c2) => !c1.Equals(c2);
        public override string ToString() => string.Format("({0}; {1})", X, Y);
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static bool TryParse(string line, out Point output)
        {
            List<int> coords = new();
            foreach (string subString in line.Split(' '))
                if (int.TryParse(subString, out int number)) coords.Add(number);
            if (coords.Count == 2)
            {
                output = new Point(coords[0], coords[1]);
                return true;
            }
            output = new Point(0, 0);
            return false;
        }
    }
    class Figure
    {
        public readonly Point[] relativeCoordinates = Array.Empty<Point>();
        public int PointCount
        {
            get { return relativeCoordinates.Length; }
        }
        public readonly byte code;

        public Figure(byte code, Point[] relativeCoordinates)
        {
            this.code = code;
            this.relativeCoordinates = relativeCoordinates;
        }
        public static Point GetRotatedCoords(Point point, Point rotationPoint)
        {
            return new(rotationPoint.Y - point.Y + rotationPoint.X, // y0 - y + x0
                       point.X - rotationPoint.X + rotationPoint.Y); // x - x0 + y0
        }
        public Point[] GetRelativeCoords(int startingPoint, int rotations)
        {
            if (startingPoint < 0 || startingPoint > PointCount) return null;

            Point[] newCoordinates = new Point[PointCount];
            Point rotationPoint = relativeCoordinates[startingPoint];

            for (int i = 0; i < PointCount; i++)
            {
                Point tempPoint = relativeCoordinates[i];
                for (int j = 0; j < rotations; j++)
                {
                    tempPoint = GetRotatedCoords(tempPoint, rotationPoint);
                }
                newCoordinates[i] = tempPoint - rotationPoint;
            }

            return newCoordinates;
        }
    }
    class TetrisField
    {
        int height = 0;
        int width = 0;
        byte[,] field = new byte[0, 0];
        int[] pieces = new int[7];

        // I J L O S T Z
        public readonly Figure[] figures = new Figure[] {
            new(1, new Point[]{ new(0, 0), new(0, 1), new(0, 2), new(0, 3)}), // I
            new(2, new Point[]{ new(0, 0), new(0, 1), new(0, 2), new(1, 2)}), // J
            new(3, new Point[]{ new(0, 0), new(1, 0), new(2, 0), new(2, 1)}), // L
            new(4, new Point[]{ new(0, 0), new(0, 1), new(1, 1), new(1, 0)}), // O
            new(5, new Point[]{ new(0, 0), new(1, 0), new(1, 1), new(2, 1)}), // S
            new(6, new Point[]{ new(0, 0), new(1, 1), new(0, 1), new(0, 2)}), // T
            new(7, new Point[]{ new(0, 0), new(0, 1), new(1, 1), new(1, 2)})  // Z
        };
        public readonly char[] names = ".IJLOSTZ".ToCharArray();
        public readonly ConsoleColor[] colors = { ConsoleColor.White,
                                                  ConsoleColor.Cyan,
                                                  ConsoleColor.Blue,
                                                  ConsoleColor.Magenta,
                                                  ConsoleColor.Yellow,
                                                  ConsoleColor.Green,
                                                  ConsoleColor.DarkMagenta,
                                                  ConsoleColor.Red};

        public void ResetField()
        {
            field = new byte[height, width];
        }

        void SetSize(int height, int width)
        {
            this.height = height;
            this.width = width;
            ResetField();
        }

        public TetrisField(int height, int width)
        {
            if ((height * width) % 4 != 0)
            {
                height = 0;
                width = 0;
            }
            SetSize(height, width);
        }

        public bool IsFittig(Point pointCoordinates, Point[] relativeCoordinates)
        {
            for (int i = 0; i < relativeCoordinates.Length; i++)
            {
                int x = pointCoordinates.X + relativeCoordinates[i].X;
                int y = pointCoordinates.Y + relativeCoordinates[i].Y;
                if (x < 0 || y < 0 || x >= height || y >= width) return false;
                if (field[x, y] != 0) return false;
            }
            return true;
        }

        public bool IsFull()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (field[i, j] == 0) return false;
                }
            }
            return true;
        }

        public bool FigurePlacerRemover(Point pointCoordinates, int startingPoint, int rotation, Figure figure, bool remove = false)
        {
            Point[] figureRelativePoints = figure.GetRelativeCoords(startingPoint, rotation);

            if (remove || (figureRelativePoints != null && IsFittig(pointCoordinates, figureRelativePoints)))
            {
                for (int i = 0; i < figure.PointCount; i++)
                {
                    field[pointCoordinates.X + figureRelativePoints[i].X,
                          pointCoordinates.Y + figureRelativePoints[i].Y] = (byte)(remove ? 0 : figure.code); // i + 1;
                }
                return true;
            }

            return false;
        }

        public bool PlaceFigure(Point pointCoordinates, int startingPoint, int rotation, Figure figure)
        {
            return FigurePlacerRemover(pointCoordinates, startingPoint, rotation, figure);
        }

        public bool RemoveFigure(Point pointCoordinates, int startingPoint, int rotation, Figure figure)
        {
            return FigurePlacerRemover(pointCoordinates, startingPoint, rotation, figure, true);
        }

        public bool Fill(int x, int y)
        {
            if (y >= width)
            {
                x++;
                y = 0;
            }
            if (x >= height)
            {
                return IsFull();
            }

            if (field[x, y] != 0)
            {
                if (Fill(x, y + 1)) return true;
            }
            else
            {
                for (int i = 0; i < pieces.Length; i++)
                {
                    if (pieces[i] > 0)
                    {
                        for (int rotation = 0; rotation < 4; rotation++)
                        {
                            if (PlaceFigure(new(x, y), 0, rotation, figures[i]))
                            {
                                pieces[i]--;
                                if (Fill(x, y + 1)) return true;
                                RemoveFigure(new(x, y), 0, rotation, figures[i]);
                                pieces[i]++;
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                if (Fill(x, y + 1)) return true;
            }

            return false;
        }

        public bool GetAnswer(int[] pieces, out string message)
        {
            if (height == 0 || width == 0)
            {
                message = String.Format("Incorrect size of field ({0} {1})", height, width);
                return false;
            }
            this.pieces = pieces;
            int sum = 0;
            foreach (int count in pieces) sum += count;
            if ((sum * 4) < (width * height))
            {
                message = "Not enough figures";
                return false;
            }
            ResetField();
            if (Fill(0, 0))
            {
                message = "Filled";
                return true;
            }
            message = "Not Filled";
            return false;
        }

        public void Print()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    ConsoleColor temp = Console.ForegroundColor;
                    Console.ForegroundColor = colors[field[i, j]];
                    Console.Write("{0}", names[field[i, j]]);
                    Console.ForegroundColor = temp;
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            TetrisField tf = new(6, 6);
            if (tf.GetAnswer(new int[] { 8, 1, 1, 0, 0, 0, 0 }, out string message))
                tf.Print();
            Console.WriteLine(message);
            Console.WriteLine("I'm finished!");
            Console.ReadLine();
        }
    }
}
