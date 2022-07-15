using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

public class LevelDesignWindow : EditorWindow
{
    [MenuItem(("Window/Level Design"))]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LevelDesignWindow));
    }
    
    private const float CELL_WIDTH = 20f;
    private WallState[,] _maze;
    private int _width;
    private int _height;
    private Position _startPosition;
    private Position _endPosition;
    private Position _selectedCell;
    private Vector3 _gridOriginPosition = new Vector3(549f, 39f);               //!!!
    private bool _isKeyDown = false;
    private string _saveFileName;
    //private int _panelToggle;
    private int _minPathStep;
    private int _nSuccessPaths;
    private int _nFailPaths;
    private float _difficulty;
    private bool _solveLevel;
    private List<Position> _minPath;
    private List<Position> _sucessPath;
    private bool _showMinPath;
    private bool _showSuccessPath;
    private float _genMazeDifficult;
    private WallState _defaultOption;
    private bool _activeWallOption;

    void OnEnable()
    {
        OnInit();
    }
    void OnInit()
    {
        _width = 0;
        _height = 0;
        _maze = new WallState[0, 0];
        _startPosition = new Position{X = 0, Y = 0};
        _endPosition = new Position{X = 0, Y = 0};
        InitOther();
    }
    void InitOther()
    {
        _selectedCell = new Position{X = -1, Y = -1};
        _saveFileName = "";
        _minPathStep = 0;
        _nSuccessPaths = 0;
        _nFailPaths = 0;
        _difficulty = 0;
        _solveLevel = false;
        _showMinPath = false;
        _showSuccessPath = false;
        _defaultOption = WallState.LEFT | WallState.DOWN | WallState.RIGHT | WallState.UP;
        _activeWallOption = false;
    }
    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Width(1000));
        OnShowListLevel();
        GUILayout.Space(20);           //
        
        OnShowLevelCustomize();
        GUILayout.Space(20);
        _isKeyDown = OnDetectInput();
        OnDrawGrid();
        if (_isKeyDown)
            Repaint();
        EditorGUILayout.EndHorizontal();
    }

    void OnShowListLevel()
    {
        EditorGUILayout.BeginVertical();

        GUILayout.Label("Level List");

        if (GUILayout.Button("New level...", GUILayout.Width(100)))
        {
            OpenNewLevel();
        } 

        GUILayout.Space(10f);

        string[] levelFiles = Directory.GetFiles(Application.persistentDataPath);
        for (int i = 0; i < levelFiles.Length; i++)
        {
            string[] nameSplit = levelFiles[i].Split('\\');
            string fileName = nameSplit[nameSplit.Length - 1];
            if (GUILayout.Button(fileName, GUILayout.Width(100)))
            {
                LoadLevel(fileName);
            }
        }

        EditorGUILayout.EndVertical();
    }

    void OnShowLevelCustomize()
    {
        EditorGUILayout.BeginVertical();

        //EditorGUILayout.BeginHorizontal();
        // if (GUILayout.Toggle(true, "Toggle me !", "Button"))
        // {
        //     Debug.Log("toggle");
        // }

        EditorGUI.BeginChangeCheck();

        GUILayout.Label("Level Customize");
        
        GUILayout.Space(10f);

        _activeWallOption = EditorGUILayout.BeginToggleGroup("Wall Option", _activeWallOption);
        EditorGUILayout.BeginHorizontal();
        if (EditorGUILayout.ToggleLeft("Left", _defaultOption.HasFlag(WallState.LEFT), GUILayout.Width(50)))
            _defaultOption |= WallState.LEFT;
        else
            _defaultOption &= ~WallState.LEFT;
        if (EditorGUILayout.ToggleLeft("Right", _defaultOption.HasFlag(WallState.RIGHT), GUILayout.Width(50)))
            _defaultOption |= WallState.RIGHT;
        else
            _defaultOption &= ~WallState.RIGHT;
        if (EditorGUILayout.ToggleLeft("Up", _defaultOption.HasFlag(WallState.UP), GUILayout.Width(50)))
            _defaultOption |= WallState.UP;
        else
            _defaultOption &= ~WallState.UP;
        if (EditorGUILayout.ToggleLeft("Down", _defaultOption.HasFlag(WallState.DOWN), GUILayout.Width(50)))
            _defaultOption |= WallState.DOWN;
        else
            _defaultOption &= ~WallState.DOWN;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndToggleGroup();


        GUILayout.Space(10f);

        GUILayout.Label("Size");
        _width = EditorGUILayout.IntSlider("X", _width, 0, GameConstant.MAX_WIDTH, GUILayout.Width(400));
        _height = EditorGUILayout.IntSlider("Y", _height, 0, GameConstant.MAX_HEIGHT, GUILayout.Width(400));


        GUILayout.Space(10f);

        GUILayout.Label("Start position: ");
        _startPosition.X = EditorGUILayout.IntSlider("X", _startPosition.X, 0, GameConstant.MAX_WIDTH - 1, GUILayout.Width(400));
        _startPosition.Y = EditorGUILayout.IntSlider("Y", _startPosition.Y, 0, GameConstant.MAX_HEIGHT - 1, GUILayout.Width(400));
        if (GUILayout.Button("Random Start", GUILayout.Width(100)))
        {
            RandomStartPosition();
        }
        
        GUILayout.Label("End position: ");
        _endPosition.X = EditorGUILayout.IntSlider("X", _endPosition.X, 0, GameConstant.MAX_WIDTH - 1, GUILayout.Width(400));
        _endPosition.Y = EditorGUILayout.IntSlider("Y", _endPosition.Y, 0, GameConstant.MAX_HEIGHT - 1, GUILayout.Width(400));
        if (GUILayout.Button("Random End", GUILayout.Width(100)))
        {
            RandomEndPosition();
        }
        GUILayout.Space(20);

        if (EditorGUI.EndChangeCheck())
        {
            _maze = ResizeMaze(_maze, _width, _height);
        }

        // # region randomize maze
        GUILayout.Label("Randomize: ");
        if (GUILayout.Button("Randomize", GUILayout.Width(150)))
        {
            RandomizeMaze();
        }
        GUILayout.Space(20);

        // # region auto-gen maze
        GUILayout.Label("AI generate maze: ");
        _genMazeDifficult = EditorGUILayout.Slider("Difficulty: ", _genMazeDifficult, 0, 1f, GUILayout.Width(400));
        if (GUILayout.Button("Generate", GUILayout.Width(150)))
        {
            AutoGenMaze();
        }
        GUILayout.Space(20);

        // # region save file
        _saveFileName = EditorGUILayout.TextField("Save file name: ", _saveFileName, GUILayout.Width(300f));
        if (GUILayout.Button("Save Level", GUILayout.Width(100)))
        {
            SaveLevel();
        }
        GUILayout.Space(20);

        // # region solve level
        GUILayout.Label("Solving Level:");
        if (GUILayout.Button("Solve level", GUILayout.Width(100)))
        {
            _solveLevel = !_solveLevel;
            if (_solveLevel == true)
                SolveLevel();
        }
        
        if (EditorGUILayout.BeginFadeGroup(_solveLevel ? 1 : 0))
        {
            _minPathStep = Int32.Parse(EditorGUILayout.TextField("Min Path Step: ", _minPathStep.ToString(), GUILayout.Width(300)));
            _nSuccessPaths = Int32.Parse(EditorGUILayout.TextField("Success Paths: ", _nSuccessPaths.ToString(), GUILayout.Width(300)));
            _nFailPaths = Int32.Parse(EditorGUILayout.TextField("Fail Paths: ", _nFailPaths.ToString(), GUILayout.Width(300)));
            _difficulty = float.Parse(EditorGUILayout.TextField("Difficulty: ", _difficulty.ToString(), GUILayout.Width(300)));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show min path", GUILayout.Width(150)))
            {
                ShowMinPath();
            }
            if (GUILayout.Button("Show success paths", GUILayout.Width(150)))
            {
                ShowSuccessPaths();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Origin map", GUILayout.Width(150)))
            {
                ShowOriginMap();
            }
            if (GUILayout.Button("Export Json", GUILayout.Width(150)))
            {
                ExportJson();
            }
            EditorGUILayout.EndHorizontal();
            
        }
        EditorGUILayout.EndFadeGroup();
        GUILayout.Space(20);

        // # region view on scene
        if (GUILayout.Button("View on scene", GUILayout.Width(150)))
        {
            ViewOnScene();
        }
        GUILayout.Space(20);

        EditorGUILayout.EndVertical();
    }

    void OnDrawGrid()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(500));

        GUILayout.Label("Preview Level");
        GUILayout.Space(20);

        EditorGUILayout.BeginVertical();

        for (int i = 0; i < _height; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < _width; j++)
            {
                GUI.color = Color.white;
                if (i == _selectedCell.Y && j == _selectedCell.X)
                    GUI.color = Color.green;
                else if (_showMinPath && _minPath.Any(pos => pos.X == j && pos.Y == i))
                    GUI.color = Color.red;
                else if (_showSuccessPath && _sucessPath.Any(pos => pos.X == j && pos.Y == i))
                    GUI.color = Color.magenta;
                string btnContent = "";
                if (i == _startPosition.Y && j == _startPosition.X)
                    btnContent = "S";
                else if (i == _endPosition.Y && j == _endPosition.X)
                    btnContent = "E";
                if (GUILayout.Button(btnContent, GUILayout.Width(CELL_WIDTH)))
                {
                    CellOnClick(i, j);
                }
                GUILayout.Space(-3f);
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(-1f);
        }

        GUI.color = Color.white;

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();

        Handles.BeginGUI();

        Handles.color = Color.yellow;
        
        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                DrawWall(i, j);   
            }
        }

        Handles.EndGUI();
    }

    void DrawWall(int i, int j)
    {
        Vector3 position = _gridOriginPosition + new Vector3(j * CELL_WIDTH, i * CELL_WIDTH);
        WallState cell = _maze[i, j];
        if (cell.HasFlag(WallState.LEFT))
            Handles.DrawLine(position, position + new Vector3(0, CELL_WIDTH));
        if (cell.HasFlag(WallState.UP))
            Handles.DrawLine(position, position + new Vector3(CELL_WIDTH, 0));
        if (j == _width - 1 && cell.HasFlag(WallState.RIGHT))
            Handles.DrawLine(position + new Vector3(CELL_WIDTH, 0), position + new Vector3(CELL_WIDTH, CELL_WIDTH));
        if (i == _height - 1 && cell.HasFlag(WallState.DOWN))
            Handles.DrawLine(position + new Vector3(0, CELL_WIDTH), position + new Vector3(CELL_WIDTH, CELL_WIDTH));
    }

    WallState[,] ResizeMaze(WallState[,] original, int width, int height)
    {
        var newMaze = original.Resize2DArray<WallState>(height, width);
        WallState initial = WallState.LEFT | WallState.RIGHT | WallState.UP | WallState.DOWN;
        for (int i = original.GetLength(0); i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                newMaze[i, j] = initial;
            }
        }
        for (int i = 0; i < height; i++)
        {
            for (int j = original.GetLength(1); j < width; j++)
            {
                newMaze[i, j] = initial;
            }
        }
        return newMaze;
    }
    bool OnDetectInput()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown && _selectedCell.X != -1)
        {
            switch (e.keyCode)
            {
                case KeyCode.A:
                    if (_selectedCell.X > 0)
                    {
                        if (_maze[_selectedCell.Y, _selectedCell.X].HasFlag(WallState.LEFT))
                        {
                            _maze[_selectedCell.Y, _selectedCell.X] &= ~WallState.LEFT;
                            _maze[_selectedCell.Y, _selectedCell.X - 1] &= ~WallState.RIGHT;
                        }
                        else
                        {
                            _maze[_selectedCell.Y, _selectedCell.X] |= WallState.LEFT;
                            _maze[_selectedCell.Y, _selectedCell.X - 1] |= WallState.RIGHT;
                        }
                    }
                    break;
                case KeyCode.W:
                    if (_selectedCell.Y > 0)
                    {
                        if (_maze[_selectedCell.Y, _selectedCell.X].HasFlag(WallState.UP))
                        {
                            _maze[_selectedCell.Y, _selectedCell.X] &= ~WallState.UP;
                            _maze[_selectedCell.Y - 1, _selectedCell.X] &= ~WallState.DOWN;
                        }
                        else
                        {
                            _maze[_selectedCell.Y, _selectedCell.X] |= WallState.UP;
                            _maze[_selectedCell.Y - 1, _selectedCell.X] |= WallState.DOWN;
                        }
                    }
                    break;
                case KeyCode.D:
                    if (_selectedCell.X < _width - 1)
                    {
                        if (_maze[_selectedCell.Y, _selectedCell.X].HasFlag(WallState.RIGHT))
                        {
                            _maze[_selectedCell.Y, _selectedCell.X] &= ~WallState.RIGHT;
                            _maze[_selectedCell.Y, _selectedCell.X + 1] &= ~WallState.LEFT;
                        }
                        else
                        {
                            _maze[_selectedCell.Y, _selectedCell.X] |= WallState.RIGHT;
                            _maze[_selectedCell.Y, _selectedCell.X + 1] |= WallState.LEFT;
                        }
                    }
                    break;
                case KeyCode.S:
                    if (_selectedCell.Y < _height - 1)
                    {
                        if (_maze[_selectedCell.Y, _selectedCell.X].HasFlag(WallState.DOWN))
                        {
                            _maze[_selectedCell.Y, _selectedCell.X] &= ~WallState.DOWN;
                            _maze[_selectedCell.Y + 1, _selectedCell.X] &= ~WallState.UP;
                        }
                        else
                        {
                            _maze[_selectedCell.Y, _selectedCell.X] |= WallState.DOWN;
                            _maze[_selectedCell.Y + 1, _selectedCell.X] |= WallState.UP;
                        }
                    }
                    break;
                case KeyCode.Q:
                    _startPosition = _selectedCell;
                    break;
                case KeyCode.E:
                    _endPosition = _selectedCell;
                    break;
            }
            return true;
        }
        return false;
    }

    // ==============Buttons on click==============
    void RandomStartPosition()
    {
        _startPosition.X = Random.Range(0, _width);
        _startPosition.Y = Random.Range(0, _height);
    }
    void RandomEndPosition()
    {
        _endPosition.X = Random.Range(0, _width);
        _endPosition.Y = Random.Range(0, _height);
    }
    void CellOnClick(int i, int j)
    {
        if (i == _selectedCell.Y && j == _selectedCell.X)
            _selectedCell = new Position{X = -1, Y = -1};
        else
            _selectedCell = new Position{X = j, Y = i};
        if (_activeWallOption)
        {
            _maze[i, j] = _defaultOption;
            if (i == 0) _maze[i, j] |= WallState.UP;
            if (i == _height - 1) _maze[i, j] |= WallState.DOWN;
            if (j == 0) _maze[i, j] |= WallState.LEFT;
            if (j == _width - 1) _maze[i, j] |= WallState.RIGHT;
            MazeGenerator.AdjustShareWalls(_maze, _width, _height, i, j);
        }
    }
    void SaveLevel()
    {
        string saveText = JsonUtility.ToJson(new MazeData(_maze, _width, _height, _startPosition, _endPosition));
        string path = Application.persistentDataPath + "/" + _saveFileName;
        using (FileStream fs = File.Create(path))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(saveText);
            fs.Write(info, 0, info.Length);
        }
        System.Diagnostics.Process.Start("explorer.exe", Application.persistentDataPath.Replace("/", "\\"));
    }
    void LoadLevel(string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName;
        string loadText = "";
        using (StreamReader sr = File.OpenText(path))
        {
            string s = "";
            while ((s = sr.ReadLine()) != null)
            {
                loadText += s;
            }
        }
        var mazeData = JsonUtility.FromJson<MazeData>(loadText);
        OnInit();
        (_width, _height, _startPosition, _endPosition, _maze) = mazeData.Parse();
        _saveFileName = fileName;
    }
    void OpenNewLevel()
    {
        OnInit();
    }
    void SolveLevel()
    {
        MazeGenerator.ResetUnvistedMaze(_maze, _width, _height);
        _minPath = PathFinder.ApplyBFS(_maze, _width, _height, _startPosition, _endPosition);
        if (_minPath != null)
        {
            _minPathStep = _minPath.Count - 1;
        }
        else
        {
            _minPathStep = 0;
        }
        MazeGenerator.ResetUnvistedMaze(_maze, _width, _height);
        _nSuccessPaths = 0;
        _nFailPaths = 0;
        _sucessPath = new List<Position>();
        List<Position> localPathList = new List<Position>();
        PathFinder.CountPaths(_maze, _width, _height, _startPosition, _endPosition, ref _nSuccessPaths, ref _nFailPaths, ref _sucessPath, ref localPathList);
        //(_nSuccessPaths, _nFailPaths) = PathFinder.CountPaths(_maze, _width, _height, _startPosition, _endPosition);
        //_difficulty = (_nFailPaths * _minPathStep) / (float)((_nFailPaths + _nSuccessPaths)*(_height*_width));
        if (_nSuccessPaths == 0)
            _difficulty = 0;
        else
            _difficulty = _nFailPaths / (float)(2*(_nFailPaths + _nSuccessPaths)) + (_minPathStep + _width + _height) / (float)(GameConstant.MAX_HEIGHT*GameConstant.MAX_WIDTH*2); 
    }
    void ShowMinPath()
    {
        // MazeGenerator.ResetUnvistedMaze(_maze, _width, _height);
        // _minPath = PathFinder.ApplyBFS(_maze, _width, _height, _startPosition, _endPosition);
        if (_minPath != null)
            _showMinPath = true;
        else
            _showMinPath = false;
    }
    void ShowSuccessPaths()
    {
        if (_sucessPath != null)
            _showSuccessPath = true;
        else
            _showSuccessPath = false;
    }
    void ShowOriginMap()
    {
        _showMinPath = false;
        _showSuccessPath = false;
    }
    void ExportJson()
    {
        string dataPath = Application.persistentDataPath + "/" + _saveFileName;
        string saveText = JsonUtility.ToJson(new LevelInfo(dataPath, _minPathStep, _nSuccessPaths, _nFailPaths, _difficulty));
        string folder = Application.persistentDataPath + "/data/";
        string savePath = folder + "data_" + _saveFileName;
        using (FileStream fs = File.Create(savePath))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(saveText);
            fs.Write(info, 0, info.Length);
        }
        System.Diagnostics.Process.Start("explorer.exe", folder.Replace("/", "\\"));
    }
    void RandomizeMaze()
    {
        _width = Random.Range(3, GameConstant.MAX_WIDTH);
        _height = Random.Range(3, GameConstant.MAX_HEIGHT);
        _startPosition = new Position{X = Random.Range(0, _width), Y = Random.Range(0, _height)};
        _endPosition = new Position{X = Random.Range(0, _width), Y = Random.Range(0, _height)};
        _maze = MazeGenerator.Generate(_width, _height, _startPosition, 10);     //
        InitOther();
    }
    void AutoGenMaze()
    {
        InitOther();
        //_width = 
        var GA = new GeneticAlgorithm();
        GA.Run(10, _width, _height, _startPosition, _endPosition, _genMazeDifficult);
        _maze = GA.GetFittestOffspring().maze;
        Repaint();
    }
    void ViewOnScene()
    {
        var gamePlay = GameObject.Find("GameManager").GetComponent<GamePlayScene>();
        gamePlay.Init(_maze, _width, _height, _startPosition, _endPosition);
        gamePlay.Clear();
        gamePlay.Draw();
    }
}
