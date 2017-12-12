using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace d3
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = Convert.ToInt32(args[0]);
            var size = 1;
            while(input > (size * size))
            {
                size = size + 2;
            }

            var grid = new Grid(size);
            var index = 1;
            Position? from = null;
            using(var enumerator = new SpiralOutwardGridPositionEnumerator(grid))
            {
                while(enumerator.MoveNext())
                {
                    grid.TakePosition(enumerator.Current, index);
                    Console.WriteLine("[{0},{1}] = {2}", enumerator.Current.X, enumerator.Current.Y, index);
                    //Console.ReadLine();
                    if(index == input)
                    {
                        from = enumerator.Current;
                    }
                    index++;
                }
            }

            var center = grid.GetCenterPosition();
            if(from.HasValue){
                Console.WriteLine(Math.Abs(center.X - from.Value.X) + Math.Abs(center.Y - from.Value.Y));
            } else {
                Console.WriteLine("{0} not found on grid.", input);
            }
            
        }

        private enum Direction { Left, Right, Up, Down }

        private class Grid
        {
            private readonly int[,] grid;
            private readonly int size;

            public Grid(int size)
            {
                this.grid = new int[size, size];
                this.size = size;
            }

            public int Size => this.size;

            public bool CanPositionBeTaken(Position position)
            {
                return this.grid[position.X, position.Y] == 0;
            }

            public void TakePosition(Position position, int value)
            {
                this.grid[position.X, position.Y] = value;
            }

            public Position GetCenterPosition()
            {
                return new Position((size - 1) / 2,(size - 1) / 2);
            }

            public Position? FindPositionOf(int value)
            {
                using(var enumerator = new SpiralOutwardGridPositionEnumerator(this))
                {
                    while(enumerator.MoveNext())
                    {
                        if(grid[enumerator.Current.X, enumerator.Current.Y] == value)
                            return enumerator.Current;
                    }
                }
                return null;
            }
        }

        private class CompassCycle
        {
            private readonly Direction[] cycle;
            private readonly int position;

            public CompassCycle(params Direction[] cycle)
            {
                this.cycle = cycle;
                this.position = 0;
            }

            private CompassCycle(Direction[] cycle, int position)
            {
                this.cycle = cycle;
                this.position = position;
            }

            public Direction Direction => this.cycle[this.position];

            public CompassCycle Next()
            {
                return new CompassCycle(this.cycle, (this.position + 1) % this.cycle.Length);
            }

            public CompassCycle Reset()
            {
                return new CompassCycle(this.cycle, 0);
            }
        }

        private class SpiralOutwardGridPositionEnumerator : IEnumerator<Position>
        {
            private readonly Grid grid;
            private readonly Position end;
            private Position position;
            private CompassCycle compassCycle;
            private bool initialized;

            public SpiralOutwardGridPositionEnumerator(Grid grid)
            {
                this.grid = grid ?? throw new ArgumentNullException(nameof(grid));
                this.compassCycle = new CompassCycle(Direction.Right, Direction.Up, Direction.Left, Direction.Down);
                this.end = new Position(grid.Size - 1, grid.Size - 1);
                this.initialized = false;
            }

            public Position Current {
                get {
                    if(!this.initialized) 
                        throw new InvalidOperationException("Please call MoveNext() before calling Current.");

                    return this.position;
                }
            }

            object IEnumerator.Current => this.Current;

            public bool MoveNext()
            {
                if(!this.initialized)
                {
                    this.position = this.grid.GetCenterPosition();
                    this.compassCycle = this.compassCycle.Reset();
                    this.initialized = true;
                    return true;
                }

                if(this.position == this.end) 
                    return false;

                var compassCycle = this.compassCycle.Next();
                var position = this.position.MoveTo(compassCycle.Direction);
                if(this.grid.CanPositionBeTaken(position))
                {
                    this.compassCycle = compassCycle;
                    this.position = position;
                } else {
                    this.position = this.position.MoveTo(this.compassCycle.Direction);
                }
                return true;
            }

            public void Reset()
            {
                this.initialized = false;
            }

            public void Dispose()
            {
            }
        }

        private struct Position : IEquatable<Position>
        {
            public readonly int X;
            public readonly int Y;

            public Position(int x, int y)
            {
                X = x;
                Y = y;
            }

            public bool Equals(Position other)
            {
                return this.X.Equals(other.X) && this.Y.Equals(other.Y);
            }

            public override bool Equals(object obj)
            {
                return (obj is Position) && Equals((Position)obj);
            }

            public override int GetHashCode()
            {
                return this.X ^ this.Y;
            }

            public static bool operator ==(Position left, Position right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Position left, Position right)
            {
                return !left.Equals(right);
            }

            public Position MoveTo(Direction direction)
            {
                var result = this;
                switch(direction)
                {
                    case Direction.Down:
                        result = new Position(this.X, this.Y - 1);
                        break;
                    case Direction.Up:
                        result = new Position(this.X, this.Y + 1);
                        break;
                    case Direction.Left:
                        result  = new Position(this.X - 1, this.Y);
                        break;
                    case Direction.Right:
                        result = new Position(this.X + 1, this.Y);
                        break;
                }
                return result;
            }
        }

    }
}
