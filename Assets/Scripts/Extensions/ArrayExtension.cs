using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtensions
{
    public static T[,] Resize2DArray<T>(this T[,] original, int rows, int cols)
    {
        var newArray = new T[rows,cols];
        int minRows = Math.Min(rows, original.GetLength(0));
        int minCols = Math.Min(cols, original.GetLength(1));
        for(int i = 0; i < minRows; i++)
            for(int j = 0; j < minCols; j++)
                newArray[i, j] = original[i, j];
        return newArray;
    }
}
