using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textLevel;
    [SerializeField] private List<GameObject> _objectStars;
    [SerializeField] private GameObject _objectLock;

    private int _level;
    private bool _isUnlocked;
    private int _nStars;

    public void Init(StageItemConfig config)
    {
        _level = config.id + 1;
        _isUnlocked = config.isUnlocked;
        _nStars = config.nStars;
    }
    public void Draw()
    {
        if (_level == 1)
        {
            _textLevel.text = "";
            var tutorialPrefab = Resources.Load<GameObject>("tutorial");
            Instantiate(tutorialPrefab, transform).name = "tutorial";
        }
        else
        {
            _textLevel.text = _level.ToString();
            var tutorial = transform.Find("tutorial");
            if (tutorial != null)
                Destroy(tutorial.gameObject);
        }
            
        _objectLock.SetActive(!_isUnlocked);
        GetComponent<Button>().enabled = _isUnlocked;
        int i = 0;
        for (; i < _nStars; i++)
            _objectStars[i].SetActive(true);
        for (; i < GameConstant.MAX_STARS; i++)
            _objectStars[i].SetActive(false);
            
    }
}
