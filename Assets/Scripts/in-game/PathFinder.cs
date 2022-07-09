using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    public static List<Position> ApplyDFS(WallState[,] maze, int width, int height, Position startPosition, Position target)
    {
        Stack<PathNode> stack = new Stack<PathNode>();
        stack.Push(new PathNode(startPosition, null));

        while (stack.Count > 0)
        {
            PathNode current = stack.Pop();
            int posX = current.position.X;
            int posY = current.position.Y;
            if (posX == target.X && posY == target.Y)
                return TrackPath(current);
            if (!maze[posY, posX].HasFlag(WallState.VISITED))
            {
                maze[posY, posX] |= WallState.VISITED;
                List<PathNode> neighbours = GetUnvisitedNeighbours(current, maze, width, height);
                foreach (var neighbour in neighbours)
                {
                    stack.Push(neighbour);
                }
            }
            
        }
        return null;
    }
    public static List<Position> ApplyBFS(WallState[,] maze, int width, int height, Position startPosition, Position target)
    {
        Queue<PathNode> queue = new Queue<PathNode>();
        queue.Enqueue(new PathNode(startPosition, null));

        while (queue.Count > 0)
        {
            foreach (var q in queue)
            {
                Debug.Log("(" + q.position.Y + "," + q.position.X + ")");
            }
            PathNode current = queue.Dequeue();
            int posX = current.position.X;
            int posY = current.position.Y;
            if (posX == target.X && posY == target.Y)
                return TrackPath(current);
            if (!maze[posY, posX].HasFlag(WallState.VISITED))
            {
                maze[posY, posX] |= WallState.VISITED;
                List<PathNode> neighbours = GetUnvisitedNeighbours(current, maze, width, height);
                foreach (var neighbour in neighbours)
                {
                    queue.Enqueue(neighbour);
                }
            }
        }
        return null;
    }

    public static List<PathNode> GetUnvisitedNeighbours(PathNode node, WallState[,] maze, int width, int height)
    {
        List<PathNode> neighbours = new List<PathNode>();

        int posX = node.position.X;
        int posY = node.position.Y;

        if (posX > 0 && !maze[posY, posX].HasFlag(WallState.LEFT) )
        {
            Position p = new Position{X = posX - 1, Y = posY};
            if (!maze[p.Y, p.X].HasFlag(WallState.VISITED))
                neighbours.Add(new PathNode(p, node));
        }
        if (posX < width - 1 && !maze[posY, posX].HasFlag(WallState.RIGHT))
        {
            Debug.Log("right");
            Position p = new Position{X = posX + 1, Y = posY};
            if (!maze[p.Y, p.X].HasFlag(WallState.VISITED))
                neighbours.Add(new PathNode(p, node));
        }
        if (posY > 0 && !maze[posY, posX].HasFlag(WallState.UP))
        {
            Position p = new Position{X = posX, Y = posY - 1};
            if (!maze[p.Y, p.X].HasFlag(WallState.VISITED))
                neighbours.Add(new PathNode(p, node));
        }
        if (posY < height - 1 && !maze[posY, posX].HasFlag(WallState.DOWN))
        {
            Debug.Log("down");
            Position p = new Position{X = posX, Y = posY + 1};
            if (!maze[p.Y, p.X].HasFlag(WallState.VISITED))
                neighbours.Add(new PathNode(p, node));
        }

        return neighbours;
    }

    public static List<Position> TrackPath(PathNode target)
    {
        List<Position> path = new List<Position>();
        PathNode node = target;
        while (node != null)
        {
            path.Add(node.position);
            node = node.parent;
        }
        path.Reverse();
        return path;
    }
}
