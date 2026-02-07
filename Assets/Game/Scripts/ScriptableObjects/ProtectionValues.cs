using UnityEngine;

[CreateAssetMenu(fileName = "ProtectionValues_", menuName = "ScriptableObjects/ProtectionValues")]
public class ProtectionValues : ScriptableObject
{
    [ContextMenuItem("SET ID", "SetId")]
    public DefenseValue[] _defValues;
    void SetId()
    {
        for (int i = 0; i < _defValues.Length; i++) _defValues[i]._id = _defValues[i]._type.ToString();
    }
}
