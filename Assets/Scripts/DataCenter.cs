using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
