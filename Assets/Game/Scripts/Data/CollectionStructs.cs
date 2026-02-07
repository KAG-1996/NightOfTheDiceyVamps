using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public struct MapTile
{
    public TilePosition _tilePos;
    public SpriteRenderer _spRend;
    public LineDrawer _line;
    public TileSelect _tileSelect;
    public GameObject _goBtn;
}
[Serializable]
public struct TileValue
{
    [HideInInspector] public string _id;
    public TypeTile _type;
    public Sprite _sprites;
    public LocalizedString _locString;
    [TextArea (2, 5)] public string _desc;
}
[Serializable]
public struct DefenseValue
{
    [HideInInspector] public string _id;
    public TypeDefense _type;
    public Sprite _sprites;
    public LocalizedString _locString;
    [TextArea(2, 5)] public string _desc;
}
[Serializable]
public struct DamageValues
{
    [HideInInspector] public string _ID;
    public DamageType _type;
    public Sprite _sprite;
    [Range(0, 12)] public int _energyReq;
    public LocalizedString _locString;
    [TextArea(2, 5)] public string _desc;
}
[Serializable]
public struct BattleSets
{
    [HideInInspector] public string _ID;
    [Range(0, 45)] public int _fromLv;
    [Range(0, 50)] public int _toLv;
    [Range(1, 3)] public int _totalEnemies;
    [Range(2, 3)] public int _totalWaves;
    public StatsEnemy[] _enemies;
}
[Serializable]
public struct DificultSettings
{
    [HideInInspector] public string _ID;
    [Range(10, 50)] public int _rows;
    [Range(3, 5)] public int _perBattle;
}
[Serializable]
public struct ResolutionSets
{
    [HideInInspector] public string _id;
    public int _width, _height;
}
[Serializable]
public struct ReordeEnemies
{
    public string _ID;
    public Transform[] _tPositions;
}
[Serializable]
public struct SliderContent
{
    [HideInInspector] public string _ID;
    public DataProccess _data;
    public Slider _slider;
    public TextMeshProUGUI _txtValue;
}
[Serializable]
public struct InfoContent
{
    public string _name;
    public LocalizedString _locString;
    public GameObject _pnlDescriptor;
    public CanvasGroup _cgDesc;
    public DescriptionContent[] _dc;
    public List<DescriptionContent> _contentDesc;
}
[Serializable]
public struct TransitionPanels
{
    [HideInInspector] public string _ID;
    public CanvasGroup _cg;
    public Slider _slider;
    public TextMeshProUGUI _txtValue;
}
[Serializable]
public struct SpeechLines
{
    [HideInInspector] public string _ID;
    public string _tittle;
    [TextArea(2, 10)]public string _desc;
    public LocalizedString _locString;
}
[Serializable]
public struct RumbleValues
{
    [HideInInspector] public string _ID;
    public TypeRumble _typeRumble;
    [Range(0f, 1f)] public float _lowFreq, _highFreq, _rumble;
}
[Serializable]
public struct SceneContent
{
    [HideInInspector] public string _ID;
    public SceneToLoad _scene;
    public LoadSceneMode _mode;
    public string _name;
#if UNITY_EDITOR
    public SceneAsset _asset;
#endif
}