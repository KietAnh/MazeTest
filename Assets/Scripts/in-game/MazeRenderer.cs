using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    public WallState[,] mazeData {get; private set;}

    [SerializeField] private int _sparseness = 0;
    [SerializeField] private float _wallThickness = 0.1f;
    [SerializeField] private Transform _wallPrefab = null;

    private int _width;
    private int _height;

    public void Generate(int width, int height, Position startPosition)
    {
        _width = width;
        _height = height;
        mazeData = MazeGenerator.Generate(width, height, startPosition, _sparseness);
        
        Draw(mazeData);
    }

    public void Init(WallState[,] mazeData, int width, int height)
    {
        this.mazeData = mazeData;
        _width = width;
        _height = height;
    }

    public void RemoveWalls()
    {
        MazeGenerator.RandomRemoveWalls(mazeData, _width, _height, _sparseness);
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        Draw(mazeData);
    }
    

    public void Draw(WallState[,] maze)                       //
    {
        Vector3[] linePositions = new Vector3[2];
        for (int i = 0; i < _height; ++i)
        {
            for (int j = 0; j < _width; ++j)
            {
                var cell = maze[i, j];
                var position = new Vector3(-_width / 2 + j, _height / 2 - i, 0);        //

                if (cell.HasFlag(WallState.UP))
                {
                    linePositions[0] = new Vector3(position.x - _wallThickness, position.y, 0) * 0.5f;
                    linePositions[1] = new Vector3(position.x + 1f + _wallThickness, position.y, 0) * 0.5f;
                    DrawWall(linePositions);
                }

                if (cell.HasFlag(WallState.LEFT))
                {
                    linePositions[0] = new Vector3(position.x, position.y + _wallThickness, 0) * 0.5f;
                    linePositions[1] = new Vector3(position.x, position.y - 1f - _wallThickness, 0) * 0.5f;
                    DrawWall(linePositions);
                }

                if (j == _width - 1)
                {
                    if (cell.HasFlag(WallState.RIGHT))
                    {
                        linePositions[0] = new Vector3(position.x + 1f, position.y + _wallThickness, 0) * 0.5f;
                        linePositions[1] = new Vector3(position.x + 1f, position.y - 1f - _wallThickness, 0) * 0.5f;
                        DrawWall(linePositions);
                    }
                }
                if (i == _height - 1)
                {
                    if (cell.HasFlag(WallState.DOWN))
                    {
                        linePositions[0] = new Vector3(position.x - _wallThickness, position.y - 1f, 0) * 0.5f;
                        linePositions[1] = new Vector3(position.x + 1f + _wallThickness, position.y - 1f, 0) * 0.5f;
                        DrawWall(linePositions);
                    }
                }
            }

        }
    }

    private void DrawWall(Vector3[] positions)
    {
        Transform wall = Instantiate(_wallPrefab, transform) as Transform;
        var line = wall.GetComponent<LineRenderer>();
        line.startWidth = _wallThickness;
        line.endWidth = _wallThickness;
        line.SetPositions(positions);
    }
    public void Clear()      // for editor
    {
        var tempList = transform.Cast<Transform>().ToList();
        foreach(var child in tempList)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}

