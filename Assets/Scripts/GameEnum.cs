using System;

[Flags]
public enum WallState
{
    // 0000 -> No walls
    LEFT = 1,  // 0001
    RIGHT = 2, // 0010
    UP = 4,    // 0100
    DOWN = 8,  // 1000
    
    VISITED = 128 // 1000 0000
}