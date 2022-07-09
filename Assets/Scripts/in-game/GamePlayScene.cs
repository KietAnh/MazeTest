﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GamePlayScene : MonoBehaviour
{
    [SerializeField] private GameObject _objectBug;
    [SerializeField] private GameObject _objectTarget;
    [SerializeField] private MazeRenderer _maze;
    [SerializeField] private PathRenderer _pathRenderer;
    [Header("Attributes")]
    [SerializeField] [Range(1, 50)] private int _width = 10;
    [SerializeField] [Range(1, 50)] private int _height = 13;
    [SerializeField] private Position _bugPosition;
    [Header("UI")]
    [SerializeField] Button _btnHint;
    [SerializeField] Button _btnMove;

    private Position _targetPosition;
    private List<Position> _path;
    private Bug _bug;

    void Start()
    {
        SetUp();
    }

    void SetUp()
    {
        _maze.Generate(_width, _height, _bugPosition);
        SpawnTarget();
        SpawnBug();
    }

    void SpawnBug()
    {
        Vector3 position = GameUtils.GetCenterCellPosition(_bugPosition, _width, _height);
        GameObject objectBug = Instantiate(_objectBug, position, Quaternion.identity);
        _bug = objectBug.GetComponent<Bug>();
    }
    void SpawnTarget()  // random target
    {
        int x = Random.Range(0, _width);
        int y = Random.Range(0, _height);
        _targetPosition = new Position {X = x, Y = y};
        Instantiate(_objectTarget, GameUtils.GetCenterCellPosition(_targetPosition, _width, _height), Quaternion.identity);
    }

    //============Buttons on clicks==============
    public void FindPathOnClick()
    {
        MazeGenerator.ResetUnvistedMaze(_maze.mazeData, _width, _height);
        _path = _pathRenderer.Generate(_maze.mazeData, _width, _height, _bugPosition, _targetPosition);
        _pathRenderer.Draw(_path, _width, _height);

        _btnHint.enabled = false;
    }
    public void SparseOnClick()
    {
        _maze.RemoveWalls();
    }
    public void AutoMoveOnClick()
    {
        if (_path == null)
        {
            MazeGenerator.ResetUnvistedMaze(_maze.mazeData, _width, _height);
            _path = _pathRenderer.Generate(_maze.mazeData, _width, _height, _bugPosition, _targetPosition);
        }
        _bug.AutoMove(_path, _width, _height);

        _btnHint.enabled = false;
        _btnMove.enabled = false;
    }
    public void MenuOnClick()
    {
        SceneManager.LoadScene(GameConstant.SCENE_STAGE);
    }
}

