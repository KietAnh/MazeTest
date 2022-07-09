using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController
{
    public static Action onRowBecameInvisible;

    public static void OnRowBecameInvisible()
    {
        onRowBecameInvisible?.Invoke();
    }
}
