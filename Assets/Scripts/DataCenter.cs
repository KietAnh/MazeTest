using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class DataCenter
{
    public static StageData stageData;

    public static void LoadData()
    {
        stageData = JsonUtility.FromJson<StageData>(PlayerPrefs.GetString(GameConstant.PLAYER_PREF_STAGE, ""));
    }
    public static void SaveData()
    {
        PlayerPrefs.SetString(GameConstant.PLAYER_PREF_STAGE, JsonUtility.ToJson(stageData));
    }
    public static void ClearData()
    {
        stageData = null;
    }
}

[System.Serializable]
public class StageData
{
    public List<StageItemConfig> items;
    public int nStagesUnlocked;
    public int nStages = 999;

    public StageData()
    {
        items = new List<StageItemConfig>();
        nStagesUnlocked = Random.Range(1, nStages + 1);
    }

    public void Generate()
    {
        for (int i = 0; i < nStages; i++)
        {
            bool isUnlocked = false;
            int nStars = 0;
            if (i < nStagesUnlocked)
            {
                isUnlocked = true;
                nStars = Random.Range(1, GameConstant.MAX_STARS + 1);
            }
            var item = new StageItemConfig(i, isUnlocked, nStars);
            items.Add(item);
        }
    }
}

[System.Serializable]
public class MazeData
{
    public int width;
    public int height;
    public Position startPosition;
    public Position endPosition;
    public List<string> walls;
    public MazeData(WallState[,] maze, int width, int height, Position startPosition, Position endPosition)
    {
        this.width = width;
        this.height = height;
        this.startPosition = startPosition;
        this.endPosition = endPosition;
        walls = new List<string>();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                walls.Add(maze[i, j].ToString());
            }
        }
    }
    public (int, int, Position, Position, WallState[,]) Parse()
    {
        int w = width;
        int h = height;
        Position sPos = startPosition;
        Position ePos = endPosition;
        WallState[,] ws = new WallState[height, width];
        
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                ws[i, j] = (WallState)Enum.Parse(typeof(WallState), walls[i * width + j]);
            }
        }

        return (w, h, sPos, ePos, ws);
    }
}

[System.Serializable]
public class LevelInfo
{
    public string path;
    public int minPathStep;
    public int nSuccessPaths;
    public int nFailPaths;
    public float difficulty;

    public LevelInfo(string path, int minPathStep, int nSuccessPaths, int nFailPaths, float difficulty)
    {
        this.path = path;
        this.minPathStep = minPathStep;
        this.nSuccessPaths = nSuccessPaths;
        this.nFailPaths = nFailPaths;
        this.difficulty = difficulty;
    }
}
