using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerScenes : MonoBehaviour
{
    public static ManagerScenes Instance;
    public PhaseLoading _phase;
    public static Action _aScene;
    public static bool _loadGame;
    public bool _isLoading;
    public SceneContent[] _sceneCnts;
    MapGenerator MapGenerator => MapGenerator.Instance;
    private void Awake()
    {
        if (!Instance) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(IPresentation());
        }
        else Destroy(gameObject);
    }
    public void UnLoadScene(SceneToLoad scene, bool load = true) => StartCoroutine(IUnLoadScene(scene, load));
    IEnumerator IPresentation()
    {
        yield return new WaitForSeconds(0.25f);
        UnLoadScene(SceneToLoad.STORY);
    }
    IEnumerator ILoadScreen(bool load, string msg)
    {
        if (!ManagerLoadingScreen.Instance) yield break;
        yield return ManagerLoadingScreen.Instance.IScaleFader(load, msg);
    }
    IEnumerator IWaitFor(SceneToLoad scene)
    {
        switch (scene)
        {
            default: yield return null; 
                break;
            case SceneToLoad.MAP:
                yield return new WaitUntil(() => MapGenerator != null);
                yield return new WaitUntil(() => MapGenerator._boardState != BoardState.LOADING);
                break;
            case SceneToLoad.COMBAT:
                yield return new WaitUntil(() => ManagerGame.Instance != null);
                MapGenerator._goCamera.SetActive(false);
                MapGenerator._goDeActives.SetActive(false);
                MapGenerator._goSelect.SetActive(false);
                break;
        }
    }
    public IEnumerator IUnLoadScene(SceneToLoad scene, bool load = true)
    {
        if (_phase != PhaseLoading.NONE) yield break;
        string name = string.Empty;
        LoadSceneMode mode = LoadSceneMode.Single;
        foreach (var a in _sceneCnts)
        {
            if (scene == a._scene)
            {
                name = a._name;
                mode = a._mode;
                break;
            }
        }
        if (string.IsNullOrEmpty(name)) yield break;
        else _phase = PhaseLoading.ENTER;
        yield return ILoadScreen(true, "Cargando...");
        if (ManagerLoadingScreen.Instance) ManagerLoadingScreen.Instance._currentScene = scene;
        _phase = PhaseLoading.LOADING;
        if (ManagerAudio.Instance) ManagerAudio.Instance.TransitionAudio(load ? scene : SceneToLoad.MAP);
        yield return load ? 
            SceneManager.LoadSceneAsync(name, mode) :
            SceneManager.UnloadSceneAsync(name);
        if (load) yield return IWaitFor(scene);
        else
        {
            _aScene?.Invoke();
            MapGenerator.UpdateTileSelection(TypeTile.START);
            ManagerLoadingScreen.Instance._currentScene = SceneToLoad.MAP;
        }
        //if (mode == LoadSceneMode.Additive) SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
        yield return ILoadScreen(false, string.Empty);
        _phase = PhaseLoading.NONE;
        if (scene == SceneToLoad.CREDITS) StartCoroutine(IExitFromCredits());
    }
    IEnumerator IExitFromCredits()
    {
        yield return new WaitForSeconds(25f);
        UnLoadScene(SceneToLoad.MENU);
    }

#if UNITY_EDITOR
    [ContextMenu("GET SCENE NAMES")]
    void GetSceneName()
    {
        for (int i = 0; i < _sceneCnts.Length; i++)
        {
            _sceneCnts[i]._ID = $"{_sceneCnts[i]._scene}";
            _sceneCnts[i]._name = _sceneCnts[i]._asset.name;
        }
    }
#endif
}