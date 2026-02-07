using System;
using System.Collections.Generic;
using UnityEngine;

public class ArenaGenerator : MonoBehaviour
{
    public AG_GameObjects _AGGO;
    public ArenaLayout[] _arenaLayouts;
    public GameObject[] _goProps;

    public List<GameObject> _goIntances;

    public ArenaLayout _currentLayout = null;

    [ContextMenu("START")]
    void Start()
    {
        ClearArena();
        GenerateArena();
    }
    void ClearArena()
    {
        foreach(var a in _goIntances) Destroy(a);
        _goIntances.Clear();
    }

    void GenerateArena()
    {
        _currentLayout = SetArenaLayout();
        foreach (var a in _currentLayout._tPositions)
        {
            int coin = UnityEngine.Random.Range(0, 2);
            _goIntances.Add( Instantiate(_goProps[coin], a.position, Quaternion.identity));
            //a.gameObject.SetActive(false);
        }
    }
    ArenaLayout SetArenaLayout()
    {
        int rnd = UnityEngine.Random.Range(0, _arenaLayouts.Length);
        ArenaLayout layout;
        if (_currentLayout != _arenaLayouts[rnd]) layout = _arenaLayouts[rnd];
        else
        {
            rnd++;
            if (rnd >= _arenaLayouts.Length) layout = _arenaLayouts[0];
            else layout = _arenaLayouts[rnd];
        }
        return layout;
    }
    [Serializable]
    public struct AG_GameObjects
    {
        public GameObject _goTree, _goBush;
    }
}

public enum RoomType { NULL, START, EMPTY, FOES, TREASURE, BOSS }