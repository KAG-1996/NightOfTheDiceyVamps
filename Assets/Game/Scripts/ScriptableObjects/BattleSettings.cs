using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleSets_", menuName = "ScriptableObjects/BattleSettings")]
public class BattleSettings : ScriptableObject
{
    [ContextMenuItem("SET ID", "SetID")]
    public BattleSets[] _battleSets;

    void SetID() 
    {
        for (int i = 0; i < _battleSets.Length; i++) _battleSets[i]._ID = $"SET FROM {_battleSets[i]._fromLv} TO {_battleSets[i]._toLv}";
    }
}
