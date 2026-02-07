using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiceClass_", menuName = "ScriptableObjects/DiceClass", order = 4)]
public class DiceClass : ScriptableObject
{
    [ContextMenuItem("SetId", "SetId")]
    public DamageValues[] _values;
    void SetId()
    {
        for (int i = 0; i < _values.Length; i++) _values[i]._ID = _values[i]._type.ToString();
    }
}
