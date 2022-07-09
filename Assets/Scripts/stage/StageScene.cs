using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageScene : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _content;
    [SerializeField] private GameObject _stageRowPrefab;
    [Header("Attributes")]
    
    [SerializeField] private float _rowSpace = 250f;
    [SerializeField] private float _bottomPadding = 175f;

    private RectTransform _scrollMapRect;
    private int _nRows;
    private int _maxId;
    private int _minId;
    private int _nStagesUnlocked;
    private int _nStages;

    void Awake()
    {
        DataCenter.LoadData();

        StageController.onRowBecameInvisible += DestroyAndSpawnRow;
    }

    void OnDestroy()
    {
        StageController.onRowBecameInvisible -= DestroyAndSpawnRow;

        DataCenter.SaveData();
    }

    void Start()
    {
        Application.targetFrameRate = 60;

        SetUpMap();
    }

    void SetUpMap()
    {
        if (DataCenter.stageData == null)
        {
            DataCenter.stageData = new StageData();
            DataCenter.stageData.Generate();
        }
        _nStages = DataCenter.stageData.nStages;
        _nStagesUnlocked = DataCenter.stageData.nStagesUnlocked;
        _nRows = _nStages / GameConstant.NUMBER_MAP_COLUMNS;
        _content.sizeDelta += new Vector2(0, _bottomPadding);

        for (int i = 0; i < _nRows && i < GameConstant.MAX_ROW_SHOW_UP; i++)
        {
            StageRow row = SpawnStageRow(i);
            List<StageItemConfig> itemsOnRow = new List<StageItemConfig>();
            for (int j = i * GameConstant.NUMBER_MAP_COLUMNS; j < (i + 1) * GameConstant.NUMBER_MAP_COLUMNS && j < _nStages; j++)
                itemsOnRow.Add(DataCenter.stageData.items[j]);
            InitRow(row, i, itemsOnRow);
        }
        _minId = 0;
        _maxId = GameConstant.MAX_ROW_SHOW_UP - 1;
    }
    StageRow SpawnStageRow(int id, bool isUp = true)       
    {
        GameObject objectStageRow = StageObjectPool.Instance.GetPooledObject();
        objectStageRow.transform.SetParent(_content.transform);
        // GameObject objectStageRow = Instantiate(_stageRowPrefab, _content.transform);
        objectStageRow.SetActive(true);
        RectTransform rectRow = objectStageRow.GetComponent<RectTransform>();
        objectStageRow.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -_bottomPadding - id*(rectRow.sizeDelta.y + _rowSpace));

        if (isUp)
        {
            _content.sizeDelta += new Vector2(0, objectStageRow.GetComponent<RectTransform>().sizeDelta.y + _rowSpace);
        }
        else
        {
            _content.sizeDelta -= new Vector2(0, objectStageRow.GetComponent<RectTransform>().sizeDelta.y + _rowSpace);
            objectStageRow.transform.SetSiblingIndex(0);
        } 

        return objectStageRow.GetComponent<StageRow>();
    }

    void DestroyAndSpawnRow()
    {
        if (_scrollRect.velocity.y > 0)    // up
        {
            if (_maxId >= _nRows)
                return;
            var botRow = _content.GetChild(0).gameObject;
            StageObjectPool.Instance.ReturnToPool(botRow);
            _maxId += 1;
            _minId += 1;
            var row = SpawnStageRow(_maxId);
            List<StageItemConfig> itemsOnRow = new List<StageItemConfig>();
            for (int j = _maxId * GameConstant.NUMBER_MAP_COLUMNS; j < (_maxId + 1) * GameConstant.NUMBER_MAP_COLUMNS && j < _nStages; j++)
                itemsOnRow.Add(DataCenter.stageData.items[j]);
            InitRow(row, _maxId, itemsOnRow);
        }
        else if (_scrollRect.velocity.y < 0)  // down
        {
            if (_minId - 1 < 0)
                return;
            var topRow = _content.GetChild(GameConstant.MAX_ROW_SHOW_UP - 1).gameObject;
            StageObjectPool.Instance.ReturnToPool(topRow);
            _maxId -= 1;
            _minId -= 1;
            var row = SpawnStageRow(_minId, false);
            List<StageItemConfig> itemsOnRow = new List<StageItemConfig>();
            for (int j = _minId * GameConstant.NUMBER_MAP_COLUMNS; j < (_minId + 1) * GameConstant.NUMBER_MAP_COLUMNS && j < _nStages; j++)
                itemsOnRow.Add(DataCenter.stageData.items[j]);
            InitRow(row, _minId, itemsOnRow);
        }
    }
    void InitRow(StageRow row, int i, List<StageItemConfig> itemsOnRow)
    {
        bool isFinal;
        int nStagesUnlockedOnRow;
        if (i < _nRows)
        {
            isFinal = false;
        }
        else
        {
            isFinal = true;
        }
        if (i < _nStagesUnlocked / GameConstant.NUMBER_MAP_COLUMNS)
            nStagesUnlockedOnRow = GameConstant.NUMBER_MAP_COLUMNS;
        else if (i == _nStagesUnlocked / GameConstant.NUMBER_MAP_COLUMNS)
            nStagesUnlockedOnRow = _nStagesUnlocked % GameConstant.NUMBER_MAP_COLUMNS;
        else
            nStagesUnlockedOnRow = 0;
                
        row.Init(i, isFinal, nStagesUnlockedOnRow, itemsOnRow);
    }

    // ====== Buttons on click==========
    public void ItemOnClick(StageItem item)
    {
        SceneManager.LoadScene(GameConstant.SCENE_INGAME);
    }
    public void ResetOnClick()
    {
        DataCenter.ClearData();
        SceneManager.LoadScene(GameConstant.SCENE_STAGE);
    }
}
