using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageItemConfig
{
    public int id;
    public bool isUnlocked = false;
    public int nStars = 0;

    public StageItemConfig(int id, bool isUnlocked, int nStars)
    {
        this.id = id;
        this.isUnlocked = isUnlocked;
        this.nStars = nStars;
    }
}
