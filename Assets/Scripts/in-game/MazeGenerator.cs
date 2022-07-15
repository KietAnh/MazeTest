using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator
{
    public static WallState[,] Generate(int width, int height, Position startPosition, int sparseness)
    {
        WallState[,] maze = new WallState[height, width];
        WallState initial = WallState.RIGHT | WallState.LEFT | WallState.UP | WallState.DOWN;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                maze[i, j] = initial;     // 1111
            }
        } 
        ApplyRecursiveBacktracker(maze, width, height, startPosition);
        if (sparseness > 0)
            RandomRemoveWalls(maze, width, height, sparseness);
        return maze;
    }

    private static WallState[,] ApplyRecursiveBacktracker(WallState[,] maze, int width, int height, Position startPosition)
    {
        var rng = new System.Random(Guid.NewGuid().GetHashCode());
        var positionStack = new Stack<Position>();
        var position = startPosition;

        maze[position.Y, position.X] |= WallState.VISITED;  // 1000 1111
        positionStack.Push(position);

        while (positionStack.Count > 0)
        {
            var current = positionStack.Pop();
            var neighbours = GetUnvisitedNeighbours(current, maze, width, height);

            if (neighbours.Count > 0)
            {
                positionStack.Push(current);

                var randIndex = rng.Next(0, neighbours.Count);
                var randomNeighbour = neighbours[randIndex];

                var nPosition = randomNeighbour.Position;
                maze[current.Y, current.X] &= ~randomNeighbour.SharedWall;
                maze[nPosition.Y, nPosition.X] &= ~GetOppositeWall(randomNeighbour.SharedWall);
                maze[nPosition.Y, nPosition.X] |= WallState.VISITED;

                positionStack.Push(nPosition);
            }
        }

        return maze;
    }
    public static void RandomRemoveWalls(WallState[,] maze, int width, int height, int sparseness)
    {
        List<int> availableIndexes = new List<int>();
        List<Position> randomPositions = new List<Position>();
        for (int i = 0; i < width * height; i++)
            availableIndexes.Add(i);
        for (int i = 0; i < sparseness; i++)
        {
            int randId = UnityEngine.Random.Range(0, availableIndexes.Count);
            var randPosition = new Position{X = availableIndexes[randId] % width, Y = availableIndexes[randId] / width};
            randomPositions.Add(randPosition);
            availableIndexes.RemoveAt(randId);
        }

        foreach (var position in randomPositions)
        {
            List<WallState> walls = new List<WallState>();
            
            if (position.X > 0 && maze[position.Y, position.X].HasFlag(WallState.LEFT))
                walls.Add(WallState.LEFT);
            if (position.Y > 0 && maze[position.Y, position.X].HasFlag(WallState.UP))
                walls.Add(WallState.UP);
            if (position.X < width - 1 && maze[position.Y, position.X].HasFlag(WallState.RIGHT))
                walls.Add(WallState.RIGHT);
            if (position.Y < height - 1 && maze[position.Y, position.X].HasFlag(WallState.DOWN))
                walls.Add(WallState.DOWN);
            
            if (walls.Count > 0)
            {
                int randId = UnityEngine.Random.Range(0, walls.Count);
                maze[position.Y, position.X] &= ~walls[randId];
                Position neighbourPos = GetNeighbourPosition(position, walls[randId]);
                maze[neighbourPos.Y, neighbourPos.X] &= ~GetOppositeWall(walls[randId]);
            }
        }
    }

    private static WallState GetOppositeWall(WallState wall)
    {
        switch (wall)
        {
            case WallState.RIGHT: return WallState.LEFT;
            case WallState.LEFT: return WallState.RIGHT;
            case WallState.UP: return WallState.DOWN;
            case WallState.DOWN: return WallState.UP;
            default: return WallState.LEFT;
        }
    }
    private static Position GetNeighbourPosition(Position pos, WallState shareWall)
    {
        switch (shareWall)
        {
            case WallState.LEFT:
                return new Position{X = pos.X - 1, Y = pos.Y};
            case WallState.RIGHT:
                return new Position{X = pos.X + 1, Y = pos.Y};
            case WallState.UP:
                return new Position{X = pos.X, Y = pos.Y - 1};
            case WallState.DOWN:
                return new Position{X = pos.X, Y = pos.Y + 1};
            default:
                return new Position {X = 0, Y = 0};
        }
    }

    private static List<Neighbour> GetUnvisitedNeighbours(Position p, WallState[,] maze, int width, int height)
    {
        var list = new List<Neighbour>();

        if (p.X > 0) // left
        {
            if (!maze[p.Y, p.X - 1].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X - 1,
                        Y = p.Y
                    },
                    SharedWall = WallState.LEFT
                });
            }
        }

        if (p.Y > 0) // UP
        {
            if (!maze[p.Y - 1, p.X].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X,
                        Y = p.Y - 1
                    },
                    SharedWall = WallState.UP
                });
            }
        }

        if (p.Y < height - 1) // DOWN
        {
            if (!maze[p.Y + 1, p.X].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X,
                        Y = p.Y + 1
                    },
                    SharedWall = WallState.DOWN
                });
            }
        }

        if (p.X < width - 1) // RIGHT
        {
            if (!maze[p.Y, p.X + 1].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X + 1,
                        Y = p.Y
                    },
                    SharedWall = WallState.RIGHT
                });
            }
        }

        return list;
    }

    public static void ResetUnvistedMaze(WallState[,] maze, int width, int height)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                maze[i, j] &= ~WallState.VISITED;
            }
        }
    }
    public static void AdjustShareWalls(WallState[,] maze, int width, int height, int i, int j)
    {
        if (i > 0)
        {
            if (maze[i, j].HasFlag(WallState.UP))
                maze[i - 1, j] |= WallState.DOWN;
            else
                maze[i - 1, j] &= ~WallState.DOWN;
        }
        if (i < height - 1)
        {
            if (maze[i, j].HasFlag(WallState.DOWN))
                maze[i + 1, j] |= WallState.UP;
            else
                maze[i + 1, j] &= ~WallState.UP;
        }  
        if (j > 0)
        {
            if (maze[i, j].HasFlag(WallState.LEFT))
                maze[i, j - 1] |= WallState.RIGHT;
            else
                maze[i, j - 1] &= ~WallState.RIGHT;
        }
        if (j < width - 1)
        {
            if (maze[i, j].HasFlag(WallState.RIGHT))
                maze[i, j + 1] |= WallState.LEFT;
            else
                maze[i, j + 1] &= ~WallState.LEFT;
        }
    }
}
