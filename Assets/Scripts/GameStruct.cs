using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Position
{
    public int X;
    public int Y;
}

public struct Neighbour
{
    public Position Position;
    public WallState SharedWall;
}


