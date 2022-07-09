using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRenderer : MonoBehaviour
{
    [SerializeField] private GameObject _pathPrefab;
    [SerializeField] private float _pathWidth = 0.75f;

    private List<Position> _path;

    public List<Position> Generate(WallState[,] maze, int width, int height, Position startPosition, Position targetPosition)
    {
        _path = PathFinder.ApplyBFS(maze, width, height, startPosition, targetPosition);
        return _path;
    }
    public void Draw(List<Position> path, int width, int height)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            GameObject objectPath = Instantiate(_pathPrefab, transform);
            var line = objectPath.GetComponent<LineRenderer>();
            line.startWidth = _pathWidth;
            line.endWidth = _pathWidth;
            Vector3 offset = Vector3.zero;
            if (path[i].X < path[i + 1].X)
                offset = new Vector3(_pathWidth/2, 0, 0);
            else if (path[i].X > path[i + 1].X)
                offset = new Vector3(-_pathWidth/2, 0, 0);
            else if (path[i].Y < path[i + 1].Y)
                offset = new Vector3(0, -_pathWidth/2, 0);
            else if (path[i].Y > path[i + 1].Y)
                offset = new Vector3(0, _pathWidth/2, 0);
            line.SetPosition(0, GameUtils.GetCenterCellPosition(path[i], width, height) - offset);
            line.SetPosition(1, GameUtils.GetCenterCellPosition(path[i + 1], width, height) + offset);
        }
    }
}
