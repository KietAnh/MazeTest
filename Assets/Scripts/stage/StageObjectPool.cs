using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObjectPool : MonoBehaviour
{
    public static StageObjectPool Instance;
    public List<GameObject> pooledObjects;
    private int amountToPool => pooledObjects.Count;

    [SerializeField] private Transform _pool;

    void Awake()
    {
        Instance = this;
    }

    // public void Init()
    // {
    //     pooledObjects = new List<GameObject>();
    //     GameObject tmp;
    //     for(int i = 0; i < amountToPool; i++)
    //     {
    //         tmp = Instantiate(objectToPool);
    //         tmp.SetActive(false);
    //         pooledObjects.Add(tmp);
    //     }
    // }

    public GameObject GetPooledObject()
    {
        for(int i = 0; i < amountToPool; i++)
        {
            if(!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }
    public void ReturnToPool(GameObject go)
    {
        go.SetActive(false);
        if (_pool != null)
            go.transform.SetParent(_pool, true);
        go.GetComponent<StageRow>().Reset();
    }
}
