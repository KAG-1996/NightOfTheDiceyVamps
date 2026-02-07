using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileSelect : HoverSelect
{
    public Color colorFinished;
    public int _level;
    public TypeTile _typeTile;
    public LineDrawer _lineDrawer;
    public SelectTile _selectTile;

    public const int MaxRedo = 3, MaxSkills = 3;

    SaveData Data => ManagerData.Instance._saveData;
    MapGenerator MapGenerator => MapGenerator.Instance;
    ManagerScenes ManagerScene => ManagerScenes.Instance;
    ManagerInputCalls ManagerInputCall => ManagerInputCalls.Instance;

    /*IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        if (!CanSetDrawer()) yield break;
        EventSystem.current.SetSelectedGameObject(gameObject);
    }*/
    int RandomValue(int min, int max) => UnityEngine.Random.Range(min, max);
    int GetRndEnumLength(Array a, int min = 0) => RandomValue(min, a.Length);
    int CurrentHealthEnegyDices() => Data._playerData._dicesHealth + Data._playerData._dicesEnergy;
    void CheckTile()
    {
        int Tile = -1;
        foreach (var a in MapGenerator._mapTiles)
        {
            foreach (var b in a.mapTiles)
            {
                if (b._tileSelect == this)
                {
                    Tile = Array.IndexOf(a.mapTiles, b);
                    break;
                }
            }
        }
        if (Tile == -1) ZDebug.Log("Tile Value is Wrong", HUE.RED);
        switch (_typeTile)
        {
            default:
                MapGenerator._currentLevel++;
                Data._playerData._currentLevel = MapGenerator._currentLevel;
                Data._playerData._currentTile = Tile;
                break;
            case TypeTile.BLOODSHED:
            case TypeTile.BATTLE:
            case TypeTile.BOSS:
                MapGenerator._currentTile = Tile;
                break;
        }
    }
    public void SetTileValues()
    {
        switch (_typeTile)
        {
            case TypeTile.BATTLE:
            case TypeTile.BLOODSHED:
            case TypeTile.BOSS:
                StartCoroutine(LoadScene());
                break;
            case TypeTile.HEALTH:
                if (CurrentHealthEnegyDices() < MapGenerator._diceHE.Length) Data._playerData._dicesHealth++;
                else ZDebug.Log($"Cannot carry more Health Dices", HUE.ORANGE);
                ZDebug.Log($"current dicesHealth = {Data._playerData._dicesHealth}", HUE.ORANGE);
                break;
            case TypeTile.ENERGY:
                if (CurrentHealthEnegyDices() < MapGenerator._diceHE.Length) Data._playerData._dicesEnergy++;
                else ZDebug.Log($"Cannot carry more Energy Dices", HUE.LIME);
                ZDebug.Log($"current dicesEnergy = {Data._playerData._dicesEnergy}", HUE.LIME);
                break;
            case TypeTile.DECK:
                if (Data._playerData._dicesDeck.Count < MapGenerator._diceDeck.Length)
                    Data._playerData._dicesDeck.Add(GetRndEnumLength(Enum.GetValues(typeof(DamageType))));
                else ZDebug.Log($"Cannot carry more Dices on Deck", HUE.LIME);
                break;
            case TypeTile.SKILL:
                if (Data._playerData._diceSkills < MaxSkills) Data._playerData._diceSkills++;
                else
                    ZDebug.Log($"Cannot carry more Skill Dices{Data._playerData._diceSkills}", HUE.MAGENTA);
                break;
            case TypeTile.PROTECTION:
                int M;
                do M = GetRndEnumLength(Enum.GetValues(typeof(TypeDefense))); 
                while (Data._playerData._defense == M);
                Data._playerData._defense = M;
                ZDebug.Log($"current defense = {(TypeDefense)Data._playerData._defense}", HUE.TEAL);
                break;
            case TypeTile.ROLL:
                if (Data._playerData._rerolls < MaxRedo) Data._playerData._rerolls++;
                ZDebug.Log($"Rerolls amount = {Data._playerData._rerolls}", HUE.WHITE);
                break;
            case TypeTile.SHUFFLE:
                if (Data._playerData._reshuffle < MaxRedo) Data._playerData._reshuffle++;
                ZDebug.Log($"Reshuffle amount = {Data._playerData._reshuffle}", HUE.WHITE);
                break;
        }
        MapGenerator._currentTypeTile = _typeTile;
        StartCoroutine(IUpdatePlayerInventory());
    }
    public bool CanSetDrawer()
    {
        if (_level != MapGenerator._currentLevel + 1) return false;
        if (MapGenerator._boardState != BoardState.WAITING) return false;
        if (MapGenerator._currentDrawer)
        {
            if (!_lineDrawer._t1.Contains(MapGenerator._currentDrawer.transform) &&
                !_lineDrawer._t2.Contains(MapGenerator._currentDrawer.transform)) return false;
        }
        return true;
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (ManagerScene._phase != PhaseLoading.NONE) return;
        if (!CanSetDrawer()) return;
        SetValues();
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!CanSetDrawer()) return;
        base.OnPointerEnter(eventData);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!CanSetDrawer()) return;
        base.OnPointerExit(eventData);
    }
    public void SetValues()
    {
        MapGenerator._currentDrawer = _lineDrawer;
        MapGenerator._tTarget = transform;
        MapGenerator.MoveSelector(_typeTile);
        MapGenerator._boardState = BoardState.MOVING;
        CheckTile();
        SetTileValues();
        _renderer.material.color = colorFinished;
    }
    IEnumerator IUpdatePlayerInventory()
    {
        yield return new WaitUntil(() => MapGenerator._boardState == BoardState.WAITING);
        MapGenerator.UpdatePlayerInventory();
    }
    IEnumerator LoadScene()
    {
        yield return new WaitUntil(() => MapGenerator._boardState == BoardState.WAITING);
        yield return new WaitForSeconds(0.1f);
        yield return ManagerScenes.Instance.IUnLoadScene(SceneToLoad.COMBAT);
        foreach (var a in MapGenerator._mapTiles) a.gameObject.SetActive(false);
        ManagerData.Instance.Save();
    }
}
