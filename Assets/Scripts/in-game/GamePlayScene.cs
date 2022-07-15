using System.Collections;
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

    private WallState[,] _mazeData;
    private Position _targetPosition;
    private List<Position> _path;
    private Bug _bug;

    void Awake()
    {
        Clear();
    }

    void Start()
    {
        SetUp();
    }

    public void Init(WallState[,] mazeData, int width, int height, Position bugPosition, Position targetPosition)
    {
        _mazeData = mazeData;
        _width = width;
        _height = height;
        _bugPosition = bugPosition;
        _targetPosition = targetPosition;
        _maze.Init(_mazeData, _width, _height);
        MazeGenerator.ResetUnvistedMaze(_maze.mazeData, _width, _height);
        _path = _pathRenderer.Generate(_maze.mazeData, _width, _height, _bugPosition, _targetPosition);
    }

    void SetUp()
    {
        _maze.Generate(_width, _height, _bugPosition);
        SpawnTarget(true);
        SpawnBug();
    }

    public void SpawnBug()
    {
        Vector3 position = GameUtils.GetCenterCellPosition(_bugPosition, _width, _height);
        GameObject objectBug = Instantiate(_objectBug, position, Quaternion.identity);
        objectBug.name = "bug";
        _bug = objectBug.GetComponent<Bug>();
    }
    public void SpawnTarget(bool random)  // random target
    {
        if (random)
        {
            int x = Random.Range(0, _width);
            int y = Random.Range(0, _height);
            _targetPosition = new Position {X = x, Y = y};
        }
        Instantiate(_objectTarget, GameUtils.GetCenterCellPosition(_targetPosition, _width, _height), Quaternion.identity).name = "target";
    }

    public void Draw()
    {
        _maze.Draw(_mazeData);
        SpawnTarget(false);
        SpawnBug();
        _pathRenderer.Draw(_path, _width, _height);
    }
    public void Clear()
    {
        _maze.Clear();
        DestroyImmediate(GameObject.Find("bug"));
        DestroyImmediate(GameObject.Find("target"));
        _pathRenderer.Clear();
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

