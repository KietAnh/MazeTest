using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageRow : MonoBehaviour
{
    [SerializeField] private List<StageItem> _items;
    [SerializeField] private GameObject _lineRight;
    [SerializeField] private GameObject _lineLeft;

    private int _index;
    private int _nItems;
    private bool _isFinal;
    private int _nStagesUnlocked;

    public void Init(int index, bool isFinal, int nStagesUnlocked, List<StageItemConfig> itemsOnRow)
    {
        _index = index;
        _nItems = itemsOnRow.Count;
        _isFinal = isFinal;
        _nStagesUnlocked = nStagesUnlocked;

        if (_index % 2 == 1)
            _items.Reverse();

        int i = 0;
        for (; i < _nItems; i++)
        {
            _items[i].Init(itemsOnRow[i]);
        }
        for (; i < GameConstant.NUMBER_MAP_COLUMNS; i++)
        {
            _items[i].GetComponent<Image>().enabled = false;
            _items[i].transform.Find("content").gameObject.SetActive(false);
        }

        OnDraw();
    }
    void OnDraw()
    {
        if (!_isFinal)
        {
            if (_index % 2 == 0)
            {
                _lineRight.SetActive(true);
                _lineLeft.SetActive(false);
            }
            else
            {
                _lineLeft.SetActive(true);
                _lineRight.SetActive(false);
            }
        }
        
        for (int i = 0; i < _nItems; i++)
        {
            _items[i].Draw();
        }
    }
    public void Reset()
    {
        if (_index % 2 == 1)
            _items.Reverse();
    }
}
