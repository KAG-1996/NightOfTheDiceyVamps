using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class SaveData
{
    public PlayerData _playerData;
    public List<MapData> _mapData;
    public SettingsData _settingsData;

    const string _pathSave = "/NightoftheDiceyVamps.json";
    private SaveData Save()
    {
        SaveData _data = new SaveData();

        _data._playerData = new();
        _data._playerData = _playerData;
        _data._mapData = new();
        _data._mapData = _mapData;

        _data._settingsData = new(true);
        _data._settingsData = _settingsData;
        return _data;
    }
    private void Load(SaveData _data)
    {
        _playerData = _data._playerData;
        _mapData = _data._mapData;
        _settingsData = _data._settingsData;
    }
    private string GetPath()
    {
#if UNITY_EDITOR
        string preferencePath = Application.dataPath + _pathSave;
#else
        string preferencePath = Application.persistentDataPath + _pathSave;
#endif
        return preferencePath;
    }
    public void SaveToJson()
    {
        ZDebug.Log("Saving...");
        SaveData _loadSC = Save();
        string _json = JsonUtility.ToJson(_loadSC);
        ZDebug.Log(_json);
        using StreamWriter writer = new(GetPath());
        writer.Write(_json);
    }
    public void SaveToBinary()
    {
        ZDebug.Log("Saving...");
        SaveData _loadSC = Save();
        BinaryFormatter bf = new BinaryFormatter();
        using var file = File.Open(GetPath(), FileMode.OpenOrCreate);
        bf.Serialize(file, _loadSC);
    }
    public void LoadFromJson()
    {
        using StreamReader reader = new(GetPath());
        string _json = reader.ReadToEnd();
        SaveData _loadSC = JsonUtility.FromJson<SaveData>(_json);
        Load(_loadSC);
    }
    public void LoadFromBinary()
    {
        BinaryFormatter bf = new BinaryFormatter();
        SaveData _loadSC = Save();
        using var file = File.Open(GetPath(), FileMode.Open, FileAccess.Read);
        _loadSC = (SaveData)bf.Deserialize(file);
        Load(_loadSC);
    }
}

[Serializable]
public struct MapData
{
    public int _level, _tiles;
    public List<int> _type;
    public MapData(int level, int tiles, List<int> Type)
    {
        this._level = level;
        this._tiles = tiles;
        this._type = Type;
    }
}
[Serializable]
public struct PlayerData
{
    public int _currentLevel;
    public int _currentTile;
    public int _runRows, _runEachBattle;
    public int _dicesHealth, _dicesEnergy, _dicesSum, _diceSkills, _rerolls, _reshuffle, _defense;
    public List<int> _dicesDeck;
    //public PlayerData(int lv, int tile, int rows, int battles, int health, int energy, int sum, int skill, int reDo, int def, List<int> deck)
    public PlayerData(int byDefault, int health, int energy, List<int> deck)
    {
        this._currentLevel = byDefault;
        this._currentTile = byDefault;
        this._runRows = byDefault;
        this._runEachBattle = byDefault;
        this._dicesHealth = health;
        this._dicesEnergy = energy;
        this._dicesSum = byDefault;
        this._diceSkills = byDefault;
        this._rerolls = byDefault;
        this._reshuffle = byDefault;
        this._defense = byDefault;
        this._dicesDeck = deck;
    }
}
[Serializable]
public struct SettingsData
{
    [Range(0.0001f, 1f)] public float _audioMaster, _auidioMusic, _audioSFX;
    public int _idiom;
    public bool _canRumble;
    public SettingsData(bool rumble)
    {
        this._audioMaster = 1f;
        this._auidioMusic = 0.7f;
        this._audioSFX = 1f;
        this._idiom = 0;
        this._canRumble = rumble;
    }
}
