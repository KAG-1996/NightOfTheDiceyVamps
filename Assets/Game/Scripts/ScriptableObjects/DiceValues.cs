using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "DiceValue_", menuName = "ScriptableObjects/DiceValues", order = 1)]
public class DiceValues : ScriptableObject
{
    [ContextMenuItem("SetId", "SetId")]
    public DamageValues[] _values;
    void SetId()
    {
        for (int i = 0; i < _values.Length; i++) _values[i]._ID = _values[i]._type.ToString();
    }
    [Serializable]
    public struct DamageValues
    {
        [HideInInspector] public string _ID;
        public TypeSkill _type;
        public Sprite _sprite;
        [Range(0, 12)] public int _energyReq;
        [TextArea(1,2)] public string _name, _desc;
        public LocalizedString _locStringName, _locStringDesc;
    }
}
