using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageRowRenderer : MonoBehaviour
{
    void OnBecameInvisible()
    {
        StageController.OnRowBecameInvisible();
    }
}
