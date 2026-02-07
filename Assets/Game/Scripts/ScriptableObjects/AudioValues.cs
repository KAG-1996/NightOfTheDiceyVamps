using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioValues", menuName = "ScriptableObjects/AudioValues")]
public class AudioValues : ScriptableObject
{
    [ContextMenuItem("SET ID", "SetId")]
    public EnvironmentMusic[] _music;
    public ClipSfx[] _sfx;
    void SetId()
    {
        for (int i = 0; i < _music.Length; i++) _music[i]._ID = _music[i]._type.ToString();
        for (int i = 0; i < _sfx.Length; i++) _sfx[i]._ID = _sfx[i]._type.ToString();
    }

    [Serializable]
    public struct EnvironmentMusic
    {
        [HideInInspector] public string _ID;
        public TypeMusic _type;
        public SceneToLoad _scene;
        public bool _isLoop;
        public AudioClip _audioClip;
    }

    [Serializable]
    public struct ClipSfx
    {
        [HideInInspector] public string _ID;
        public TypeSFX _type;
        public AudioClip _audioClip;
    }
}
