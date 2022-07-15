using System.Linq;
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
    public static void CountPaths(WallState[,] maze, int width, int height, Position start, Position end, ref int nSuccessPaths, ref int nFailPaths)
    {
        if (start.X == end.X && start.Y == end.Y)
        {
            nSuccessPaths += 1;
            return;
        }
        PathNode current = new PathNode(start, null);     
        maze[start.Y, start.X] |= WallState.VISITED;
        List<PathNode> neighbours = GetUnvisitedNeighbours(current, maze, width, height);
        if (neighbours.Count == 0)
        {
            nFailPaths += 1;
        }
        foreach (var neighbour in neighbours)
        {
            CountPaths(maze, width, height, neighbour.position, end, ref nSuccessPaths, ref nFailPaths);
        }
        maze[start.Y, start.X] &= ~WallState.VISITED;
    }
    public static void CountPaths(WallState[,] maze, int width, int height, Position start, Position end, ref int nSuccessPaths, ref int nFailPaths, ref List<Position> successPath, ref List<Position> localPathList)
    {
        if (start.X == end.X && start.Y == end.Y)
        {
            foreach (var p in localPathList)
            {
                if (!successPath.Contains(p))
                {
                    successPath.Add(p);
                }
            }
            nSuccessPaths += 1;
            return;
        }
        PathNode current = new PathNode(start, null);     
        maze[start.Y, start.X] |= WallState.VISITED;
        List<PathNode> neighbours = GetUnvisitedNeighbours(current, maze, width, height);
        if (neighbours.Count == 0)
        {
            nFailPaths += 1;
        }
        foreach (var neighbour in neighbours)
        {
            localPathList.Add(neighbour.position);
            CountPaths(maze, width, height, neighbour.position, end, ref nSuccessPaths, ref nFailPaths, ref successPath, ref localPathList);
            localPathList.Remove(neighbour.position);
        }
        maze[start.Y, start.X] &= ~WallState.VISITED;
    }
    
    // public static (int, int) CountPaths(WallState[,] maze, int width, int height, Position start, Position end)
    // {
    //     int nSuccessPaths = 0;
    //     int nFailPaths = 0;
    //     Stack<PathNode> stack = new Stack<PathNode>();
    //     List<PathNode> pathList = new List<PathNode>();
    //     stack.Push(new PathNode(start, null));

    //     while (stack.Count > 0)
    //     {
    //         // string s = "";
    //         // foreach (var ele in stack)
    //         // {
    //         //     s += "(" + ele.position.X + "," + ele.position.Y + "),"; 
    //         // }
    //         // Debug.Log(s);

    //         PathNode current = stack.Pop();
    //         int posX = current.position.X;
    //         int posY = current.position.Y;
    //         if (posX == end.X && posY == end.Y)
    //         {
    //             nSuccessPaths += 1;
    //             break;
    //         }
    //         if (!maze[posY, posX].HasFlag(WallState.VISITED))
    //         {
    //             maze[posY, posX] |= WallState.VISITED;
    //             List<PathNode> neighbours = GetUnvisitedNeighbours(current, maze, width, height);
    //             int countNeighbours = 0;
    //             foreach (var neighbour in neighbours)
    //             {
    //                 if (current.parent != null && neighbour.position.X == current.parent.position.X && neighbour.position.Y == current.parent.position.Y)
    //                 {
    //                     continue;
    //                 }
    //                 stack.Push(neighbour);
    //                 countNeighbours += 1;
    //             }
    //             if (countNeighbours == 0)
    //                 nFailPaths += 1;
    //             maze[posY, posX] &= ~WallState.VISITED;
    //         }
    //     }
    //     return (nSuccessPaths, nFailPaths);
    // }

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
