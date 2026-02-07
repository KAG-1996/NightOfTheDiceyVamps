using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance;
    public static int Challenge = 1;

    public TypeTile _currentTypeTile = TypeTile.BATTLE;
    public BoardState _boardState;
    public int _currentLevel = 0;
    public int _currentTile = 0;
    public int _distance = 15;
    [Range(10, 50)] public int _rows = 10;
    [Range(3, 5)] public int _eachBattle = 3;
    public MapTiles _tileOnce, _tileTwice, _tileThrice;

    public MapTiles _prevMapT = null;
    public LineDrawer _prevDrawer = null;
    public LineDrawer _currentDrawer = null;

    public Color _colorHealth, _colorEnergy;
    public TileValues _tileValues;
    public BattleSettings _battleSets;
    public ProtectionValues _protectValues;
    public DiceClass _diceClass;
    public Transform _tTarget;
    public DificultSettings[] _dificultSettings;
    public float _speed = 21f;

    public List<MapTiles> _mapTiles;
    public Dice[] _diceDeck, _diceHE, _diceSkills;
    public GameObject[] _goGamepadIcons;

    public GameObject _goSelect, _goCamera, _goDeActives;
    public GameObject _currentGoTile, _pnlInfo;
    public CanvasGroup _cgInfo;
    public ManagerInfoScroll _infoScroll;
    public ScrollRect _srInfo;
    public Scrollbar _sbInfo;
    public TextMeshProUGUI _txtAwards, _txtRolls, _txtShuffle, _txtInfo;
    public Image _imgProtect;
    public Transform _tCanvasWorld;
    public InfoContent[] _infoConts;

    SaveData Data => ManagerData.Instance._saveData;
    ManagerInputCalls ManagerInputCall => ManagerInputCalls.Instance;
    ManagerLoadingScreen ManagerLoadingScreen => ManagerLoadingScreen.Instance;
    void Start()
    {
        _boardState = BoardState.LOADING;
        SetDescriptionPanels();
        StartCoroutine(IGenerteMap(ManagerScenes._loadGame));
        _infoScroll.CGSets(_cgInfo, false);
        ManagerScenes._aScene += ActivateHidders;
    }
    private void Awake() => Instance = this;
    private void OnDestroy()
    {
        Instance = null;
        ManagerScenes._aScene -= ActivateHidders;
    }
    private void LateUpdate()
    {
        foreach (var a in _goGamepadIcons) a.SetActive(ManagerInputCall?._input.currentControlScheme == "Gamepad");
    }
    void SetDescriptionPanels()
    {
        //_infoScroll._txtInfo.SetText(_infoScroll._infoConts[0]._name);
        _infoScroll.SetDescriptionPanels(_diceClass);
        _infoScroll.SetDescriptionPanels(_protectValues);
        _infoScroll.SetDescriptionPanels(_tileValues);
    }
    void ActivateHidders()
    {
        _goCamera.SetActive(true);
        _goSelect.SetActive(true);
        _goDeActives.SetActive(true);
        foreach (var a in _mapTiles) a.gameObject.SetActive(true);
        UpdatePlayerInventory();
    }
    public void UpdateTileSelection(TypeTile tile)
    {
        foreach (var a in _mapTiles[_currentLevel].mapTiles) a._tileSelect._selectTile.DeActiveBtn(false);
        foreach (var a in _mapTiles[_currentLevel + 1].mapTiles) a._tileSelect._selectTile.SetSelected(!IsTile(tile));
    }
    bool IsTile(TypeTile tile)
    {
        return tile == TypeTile.BLOODSHED || tile == TypeTile.BATTLE || tile == TypeTile.BOSS;
    }
    public void MoveSelector(TypeTile tile) => StartCoroutine(IMoveSelector(tile));
    public IEnumerator IMoveSelector(TypeTile tile)
    {
        while (Vector3.Distance(_goSelect.transform.position, new Vector3(_tTarget.position.x, _tTarget.position.y + 1, _tTarget.position.z)) > 0)
        {
            yield return null;
            _goSelect.transform.position =
                Vector3.MoveTowards(_goSelect.transform.position,
                new Vector3(_tTarget.position.x, _tTarget.position.y + 1, _tTarget.position.z), _speed * Time.deltaTime);
        }
        yield return IMoveCamera();
        _boardState = BoardState.WAITING;
        UpdateTileSelection(tile);
        _tTarget = null;
    }
    [ContextMenu("SET DICE DECK")]
    public void UpdatePlayerInventory()
    {
        foreach (var a in _diceDeck) a.gameObject.SetActive(false);
        for (int i = 0; i < Data._playerData._dicesDeck.Count; i++)
        {
            _diceDeck[i].SetDiceType((DamageType)Data._playerData._dicesDeck[i]);
            _diceDeck[i].DiceValue(6);
            _diceDeck[i].gameObject.SetActive(true);
        }
        _imgProtect.sprite = _protectValues._defValues[Data._playerData._defense]._sprites;
        if (Data._playerData._dicesHealth + Data._playerData._dicesEnergy <= _diceHE.Length)
        {
            foreach (var a in _diceHE) a.gameObject.SetActive(false);
            for (int i = 0; i < Data._playerData._dicesHealth; i++)
            {
                _diceHE[i]._imgType.color = _colorHealth;
                _diceHE[i].DiceValue(6);
                _diceHE[i].gameObject.SetActive(true);
            }
            for (int i = Data._playerData._dicesHealth; i < (Data._playerData._dicesHealth + Data._playerData._dicesEnergy); i++)
            {
                _diceHE[i]._imgType.color = _colorEnergy;
                _diceHE[i].DiceValue(6);
                _diceHE[i].gameObject.SetActive(true);
            }
        }
        foreach (var a in _diceSkills) a.gameObject.SetActive(false);
        for (int i = 0; i < Data._playerData._diceSkills; i++)
        {
            //_diceSkills[i].DiceValue(6);
            _diceSkills[i].gameObject.SetActive(true);
        }
        _txtRolls.SetText($"3 / {Data._playerData._rerolls}");
        _txtShuffle.SetText($"3 / {Data._playerData._reshuffle}");
    }

    [ContextMenu("CreateMap")]
    void CreateMap()
    {
        StopCoroutine("IGenerteMap");
        StartCoroutine(IGenerteMap(false));
    }
    [ContextMenu("LoadMap")]
    void LoadMap()
    {
        StopCoroutine("IGenerteMap");
        StartCoroutine(IGenerteMap(true));
    }
    bool Mustfork(TilePosition tp1, TilePosition tp2, bool path)
    {
        /*if (path) return tp1 != tp2;
        else return tp1 == tp2;*/
        return path ? tp1 != tp2 : tp1 == tp2;
    }
    Transform LoadLastPosition()
    {
        for (int i = 0; i <= _currentLevel; i++)
        {
            if (i == _currentLevel)
            {
                ZDebug.Log($"Checking lv {i}");
                foreach (var a in _mapTiles[i].mapTiles)
                {
                    if (Array.IndexOf(_mapTiles[i].mapTiles, a) == Data._playerData._currentTile)
                    {
                        _currentDrawer = a._line;
                        return a._tileSelect.transform;
                    }
                }
            }
        }
        ZDebug.Log("Last Position Null", HUE.RED);
        return null;
    }
    public void ShowInfo()
    {
        if (_boardState != BoardState.WAITING) return;
        if (ManagerLoadingScreen._pnlExit.activeInHierarchy) return;
        //_pnlInfo.SetActive(!_pnlInfo.activeInHierarchy);
        _infoScroll.CGSets(_cgInfo, _cgInfo.alpha == 0);
        //ManagerInputCall.UpdateSelected(_pnlInfo.activeInHierarchy ? _sbInfo.gameObject : _currentGoTile);
        ManagerInputCall.UpdateSelected(_cgInfo.alpha != 0 ? _sbInfo.gameObject : _currentGoTile);
    }
    #region ButtonCalls
    public void Z_ExitToMenu()
    {
        ManagerData.Instance.Save();
        ManagerScenes.Instance.UnLoadScene(SceneToLoad.MENU);
    }
    public int currentDescriptor = 0;
    public void Z_ChangePage(bool forward)
    {
    }
    public void Z_PanelOpen(GameObject pnl) => pnl.SetActive(true);
    public void Z_PanelClose(GameObject pnl) => pnl.SetActive(false);
    public void Z_PanelOpen(CanvasGroup pnl) => _infoScroll.CGSets(pnl, true);
    public void Z_PanelClose(CanvasGroup pnl) => _infoScroll.CGSets(pnl, false);
    #endregion
    #region Coroutines
    IEnumerator IGenerteMap(bool load)
    {
        _currentLevel = load ? Data._playerData._currentLevel : 0;
        _rows = load ? Data._playerData._runRows : _dificultSettings[Challenge]._rows;
        _eachBattle = load ? Data._playerData._runEachBattle : _dificultSettings[Challenge]._perBattle;
        _goSelect.transform.localPosition = Vector3.zero;
        _goCamera.transform.localPosition = new Vector3(_goCamera.transform.position.x, _goCamera.transform.position.y, _goSelect.transform.position.z - 100f);
        foreach (var mapTile in _mapTiles) Destroy(mapTile.gameObject);
        _mapTiles.Clear();
        if (!load) ManagerData.Instance._saveData._mapData = new();
        for (int i = 0; i <= _rows; i++)
        {
            yield return new WaitForEndOfFrame();
            if (i == 0 || i == _rows) yield return IGenerateTile(_tileOnce, i, load);
            else if (i == 1 || i == _rows - 1) yield return IGenerateTile(_tileTwice, i, load);
            else
            {
                if (load) yield return IGenerateTile(ManagerData.Instance._saveData._mapData[i]._tiles == 2 ? _tileTwice : _tileThrice, i, true);
                else yield return IGenerateTile(UnityEngine.Random.Range(0, 2) == 0 ? _tileTwice : _tileThrice, i, false);
            }
        }
        if (load)
        {
            _tTarget = LoadLastPosition();
            _goSelect.transform.position = new Vector3(_tTarget.position.x, _tTarget.position.y + 1, _tTarget.position.z);
            _goCamera.transform.localPosition = new Vector3(_goCamera.transform.position.x, _goCamera.transform.position.y, _goSelect.transform.position.z - 100f);
        }
        else
        {
            ManagerData.Instance.NewPlayerData();
            Data._playerData._runRows = _rows;
            Data._playerData._runEachBattle = _eachBattle;
        }
        UpdatePlayerInventory();
        _boardState = BoardState.WAITING;
        UpdateTileSelection(TypeTile.START);
    }
    IEnumerator IMoveCamera()
    {
        while (_goCamera.transform.position.z < _goSelect.transform.position.z - 100f)
        {
            yield return null;
            _goCamera.transform.position += Vector3.forward * _speed * Time.deltaTime;
        }
    }
    IEnumerator IGenerateTile(MapTiles mt, int value, bool load)
    {
        yield return new WaitForEndOfFrame();
        var currMapT = Instantiate(mt);
        _mapTiles.Add(currMapT);
        currMapT._level = value;
        currMapT.transform.SetParent(transform, false);
        currMapT.transform.localPosition = new Vector3(0, 0, currMapT.transform.position.z + (_distance * value));
        currMapT.name = $"{currMapT.name} {value}";
        currMapT.gameObject.SetActive(true);
        for (int i = 0; i < currMapT.mapTiles.Length; i++) currMapT.mapTiles[i]._line.name = $"{currMapT.mapTiles[i]._line.name} {value} {i}";
        List<int> types = new();
        if (_prevMapT)
        {
            List<int> existTiles = new();
            int LastFight = 0;
            int path = UnityEngine.Random.Range(0, 2);
            for (int i = 0; i < currMapT.mapTiles.Length; i++)
            {
                currMapT.mapTiles[i]._line._t1.Clear();
                currMapT.mapTiles[i]._tileSelect._level = value;
                currMapT.mapTiles[i]._goBtn.transform.SetParent(_tCanvasWorld);
                currMapT.mapTiles[i]._goBtn.name = $"{currMapT.name} {value} {i}";
                for (int j = 0; j < _prevMapT.mapTiles.Length; j++)
                {
                    switch (currMapT.mapTiles[i]._tilePos)
                    {
                        case TilePosition.CENTRAL:
                            if (_prevMapT.mapTiles[j]._tilePos != TilePosition.CENTRAL)
                            {
                                currMapT.mapTiles[i]._line.CreatePath(_prevMapT.mapTiles[j]._line);
                            }
                            break;
                        case TilePosition.LEFT:
                            if (_prevMapT.mapTiles[j]._tilePos != TilePosition.LEFT)
                            {
                                currMapT.mapTiles[i]._line.CreatePath(_prevMapT.mapTiles[j]._line);
                            }
                            break; 
                        case TilePosition.RIGHT:
                            if (_prevMapT.mapTiles[j]._tilePos != TilePosition.RIGHT)
                            {
                                currMapT.mapTiles[i]._line.CreatePath(_prevMapT.mapTiles[j]._line);
                            }
                            break;
                    }
                }
                int type;// = UnityEngine.Random.Range(1, _tileValues._tileValues.Length - 1);//don´t choose start point and final boss
                //if (value % _eachBattle == _eachBattle) type = UnityEngine.Random.Range(1, 3); //ONLY FOR TEST
                if (value % _eachBattle == 0) //SET THIS FOR FINAL SUBMIT
                {
                    LastFight = LastFight == 0 ? UnityEngine.Random.Range(1, 3) : LastFight == 1 ? 2 : 1;
                    type = LastFight;
                }
                else
                {
                    type = UnityEngine.Random.Range(3, _tileValues._tileValues.Length - 1);//don´t choose start point and final boss
                    if (!existTiles.Contains(type)) existTiles.Add(type);
                    else
                    {
                        while (existTiles.Contains(type))
                        {
                            yield return null;
                            type = UnityEngine.Random.Range(3, _tileValues._tileValues.Length - 1);
                        }
                        existTiles.Add(type);
                    }
                }
                //else type = UnityEngine.Random.Range(1, _tileValues._tileValues.Length - 1);//don´t choose start point and final boss
                if (currMapT._level == _rows)
                {
                    currMapT.mapTiles[i]._spRend.sprite = _tileValues._tileValues[_tileValues._tileValues.Length - 1]._sprites;
                    currMapT.mapTiles[i]._tileSelect._typeTile = _tileValues._tileValues[_tileValues._tileValues.Length - 1]._type;
                    types.Add(_tileValues._tileValues.Length - 1);
                }
                else
                {
                    currMapT.mapTiles[i]._spRend.sprite = _tileValues._tileValues[load ? ManagerData.Instance._saveData._mapData[value]._type[i] : type]._sprites;
                    currMapT.mapTiles[i]._tileSelect._typeTile = _tileValues._tileValues[load ? ManagerData.Instance._saveData._mapData[value]._type[i] : type]._type;
                    types.Add(load ? ManagerData.Instance._saveData._mapData[value]._type[i] : type);
                }
            }
            _prevMapT = currMapT;
        }
        else
        {
            _prevMapT = currMapT;
            _currentDrawer = currMapT.mapTiles[0]._line;
            currMapT.mapTiles[0]._spRend.sprite = _tileValues._tileValues[0]._sprites;
            currMapT.mapTiles[0]._goBtn.transform.SetParent(_tCanvasWorld);
        }
        if (!load) ManagerData.Instance._saveData._mapData.Add(new(value, currMapT.mapTiles.Length, types));
    }
    #endregion
}
