using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bug : MonoBehaviour
{
    [SerializeField] private float _moveTimePerStep;

    public void AutoMove(List<Position> path, int width, int height)
    {
        var wayPoints = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            wayPoints[i] = GameUtils.GetCenterCellPosition(path[i], width, height);
        }
        transform.DOPath(wayPoints, _moveTimePerStep * (path.Count - 1)).SetEase(Ease.Linear).OnWaypointChange((id) => {
            if (id < wayPoints.Length - 1)
                RotateByDirection(wayPoints[id], wayPoints[id + 1]);
        });
    }
    void RotateByDirection(Vector3 from, Vector3 to)
    {
        Vector3 direction = to - from;
        if (direction.x > 0)
            transform.rotation = Quaternion.Euler(0, 0, -90f);
        else if (direction.x < 0)
            transform.rotation = Quaternion.Euler(0, 0, 90f);
        else if (direction.y > 0)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (direction.y < 0)
            transform.rotation = Quaternion.Euler(0, 0, 180f);
    }
}
