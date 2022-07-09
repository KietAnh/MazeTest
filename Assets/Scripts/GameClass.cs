using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public Position position;
    public PathNode parent;

    public PathNode(Position position, PathNode parent)
    {
        this.position = position;
        this.parent = parent;
    }
}