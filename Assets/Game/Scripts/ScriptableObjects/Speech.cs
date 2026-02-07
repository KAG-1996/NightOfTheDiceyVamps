using UnityEngine;

[CreateAssetMenu(fileName = "Speech_", menuName = "ScriptableObjects/Speech")]
public class Speech : ScriptableObject
{
    [ContextMenuItem("SET ID", "SetId")]
    public SpeechLines[] _speechLines;
    void SetId()
    {
        for (int i = 0; i < _speechLines.Length; i++) _speechLines[i]._ID = _speechLines[i]._tittle;
    }
}
