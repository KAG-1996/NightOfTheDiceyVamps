using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileValues_", menuName = "ScriptableObjects/TileValues")]
public class TileValues : ScriptableObject
{
    [ContextMenuItem("SET ID", "SetId")]
    public TileValue[] _tileValues;
    void SetId()
    {
        for (int i = 0; i < _tileValues.Length; i++) _tileValues[i]._id = _tileValues[i]._type.ToString();
    }
}
