using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class ManagerAudio : MonoBehaviour
{
    public static ManagerAudio Instance;
    public AudioSource _audioMusic, _audioSFX;
    public AudioMixerGroup _gMixerMaster, _gMixerMusic, _gMixerSFX;
    public AudioValues _music;
    public float _speed = 0.01f;

    bool _isplay = true;
    SaveData Data => ManagerData.Instance._saveData;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            _isplay = true;
        }
        else Destroy(gameObject);
    }
    private void Start()
    {
        UpdateAudioVolume(); //load audio settings
    }
    public void PlaySFX(TypeSFX sfx, bool overplay = true)
    {
        foreach (var a in _music._sfx)
        {
            if (a._type == sfx) _audioSFX.clip = a._audioClip; 
        }
        if (!_audioSFX.isPlaying || overplay) _audioSFX.Play();
    }
    public void PlaySFX(AudioSource source, TypeSFX sfx, bool overplay = true)
    {
        foreach (var a in _music._sfx)
        {
            if (a._type == sfx) source.clip = a._audioClip; 
        }
        if (!source.isPlaying || overplay) source.Play();
    }
    public void UpdateAudioVolume()
    {
        try { if (Data == null) return; }
        catch { return; }
        _gMixerMaster.audioMixer.SetFloat("Master", Mathf.Log10(Data._settingsData._audioMaster) * 20);
        _gMixerMusic.audioMixer.SetFloat("Music", Mathf.Log10(Data._settingsData._auidioMusic)* 20);
        _gMixerSFX.audioMixer.SetFloat("SFX", Mathf.Log10(Data._settingsData._audioSFX) * 20);
    }
    [ContextMenu("DeIncreaseVolume")]
    void DeIncreaseVolume() => StartCoroutine(IDeIncreaseVolume(_isplay = !_isplay));
    public void TransitionAudio(SceneToLoad scene) => StartCoroutine(ITransitionAudio(scene));
    public IEnumerator ITransitionAudio(SceneToLoad scene)
    {
        if (_audioMusic.clip) yield return IDeIncreaseVolume(false);
        foreach(var a in _music._music)
        {
            if (a._scene == scene) _audioMusic.clip = a._audioClip;
        }
        yield return new WaitUntil(() => ManagerScenes.Instance._phase != PhaseLoading.LOADING);
        _audioMusic.Play();
        yield return IDeIncreaseVolume(true);
    }
    public IEnumerator IDeIncreaseVolume(bool increase)
    {
        if (increase)
        {
            while (_audioMusic.volume < 1)
            {
                yield return null;
                _audioMusic.volume += Time.timeScale * _speed;
                _audioSFX.volume += Time.timeScale * _speed;
            }
        }
        else
        {
            while (_audioMusic.volume > 0)
            {
                yield return null;
                _audioMusic.volume -= Time.timeScale * _speed;
                _audioSFX.volume -= Time.timeScale * _speed;
            }
        }
    }
}
