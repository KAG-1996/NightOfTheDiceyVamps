using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatsEnemy_", menuName = "ScriptableObjects/StatsEnemy", order = 2)]
public class StatsEnemy : ScriptableObject
{
    public TypeEnemy _typeEnemy;
    public TypeDefense[] _typeDefenses;
    public GameObject _goPrefab;
    public Sprite _sprite;
    [Range(1, 5)] public int _healthDices = 1;
    [Range(1, 10)] public int _suckOdds;
}