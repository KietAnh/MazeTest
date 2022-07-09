using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameUtils
{
    public static Vector3 GetCenterCellPosition(Position coordinate, int width, int height)
    {
        return new Vector3(-width/2 + coordinate.X + 0.5f, height/2 - coordinate.Y - 0.5f, 0) * 0.5f;
    }
}
