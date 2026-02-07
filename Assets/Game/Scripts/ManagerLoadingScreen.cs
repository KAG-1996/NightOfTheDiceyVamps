using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class ManagerLoadingScreen : MonoBehaviour
{
    public static ManagerLoadingScreen Instance;
    public static bool IsLoading;

    public SceneToLoad _currentScene = SceneToLoad.NONE;
    public RectTransform _rtFader;
    public TextMeshProUGUI _txtLoad, _txtExit;
    public LocalizeStringEvent _locEventQuit;
    public LocalizedString _locMenu, _locExit;
    public GameObject _pnlLoading, _pnlExit, _goSelected;
    public Animator _animLoad, _animSave;
    public float _speed = 2f;
    public float _scale = 15f;

    MainMenu MainMenu => MainMenu.Instance;
    MapGenerator MapGenerator => MapGenerator.Instance;
    ManagerGame ManagerGame => ManagerGame.Instance;
    ManagerInputCalls ManagerInputCall => ManagerInputCalls.Instance;
    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            QualitySettings.vSyncCount = 0; // Disable VSync
#if UNITY_ANDROID
            Application.targetFrameRate = 60;
#else
            Application.targetFrameRate = 60;
#endif
        }
        else Destroy(gameObject);
    }

    public float pollingTime = 1f; // How often to update the FPS display
    private float time;
    private int frameCount;
    public TextMeshProUGUI fpsText; // Assign this in the Inspector
    /*void Update()
    {
        fpsCounter();
    }*/
    void fpsCounter()
    {
        // Calculate the time passed since the last frame
        time += Time.deltaTime;
        frameCount++;

        // Check if the polling time has been reached
        if (time >= pollingTime)
        {
            // Calculate the FPS
            int frameRate = Mathf.RoundToInt(frameCount / time);

            // Update the UI Text element
            if (fpsText != null)
            {
                fpsText.text = frameRate.ToString() + " FPS";
            }

            // Reset for the next polling interval
            time -= pollingTime; // Subtract pollingTime to maintain accuracy over time
            frameCount = 0;
        }
    }
    public void Exit()
    {
        if (IsLoading) return;
        if (ManagerScenes.Instance._phase == PhaseLoading.LOADING) return;
        if (!loadedScene()) return;
        ManagerInputCall.UpdateSelected(_goSelected);
        switch (_currentScene)
        {
            //case SceneToLoad.MENU: _txtExit.SetText("¿Salir del juego?"); break;
            case SceneToLoad.MENU: _locEventQuit.StringReference = _locExit; break;
            case SceneToLoad.MAP:
                if (MapGenerator._boardState != BoardState.WAITING) return;
                if (MapGenerator._pnlInfo.activeInHierarchy) MapGenerator._pnlInfo.SetActive(false);
                _txtExit.SetText("¿Volver al menu principal?"); 
                _locEventQuit.StringReference = _locMenu;
                break;
            case SceneToLoad.COMBAT:
            case SceneToLoad.TUTORIAL:
                if (ManagerGame._phase != Phases.PLANNING) return;
                if (ManagerGame._pnlSkillUse.activeInHierarchy) ManagerGame._pnlSkillUse.SetActive(false);
                if (ManagerGame._pnlInfo.activeInHierarchy) ManagerGame._pnlInfo.SetActive(false);
                //_txtExit.SetText("¿Volver al menu principal?");
                _locEventQuit.StringReference = _locMenu;
                break;
        }
        if (!_pnlExit.activeInHierarchy) _pnlExit.SetActive(true);
    }
    public void LoadMessage(string message) => _txtLoad.SetText(message);
    public void LoadMessage(bool msg) => _txtLoad.gameObject.SetActive(msg);
    bool loadedScene()
    {
        return 
            _currentScene == SceneToLoad.MENU || 
            _currentScene == SceneToLoad.MAP || 
            _currentScene == SceneToLoad.TUTORIAL || 
            _currentScene == SceneToLoad.COMBAT;
    }
    #region InputSysCalls
    public void Z_ExitGame(bool exit)
    {
        _pnlExit.SetActive(false);
        if (exit)
        {
            if (_currentScene == SceneToLoad.MENU) Application.Quit();
            else
            {
                ManagerData.Instance.Save();
                ManagerScenes.Instance.UnLoadScene(SceneToLoad.MENU);
            }
        }
        else
        {
            switch (_currentScene)
            {
                case SceneToLoad.MENU: MainMenu?.SetFirstSelected(); break;
                case SceneToLoad.MAP: ManagerInputCall?.UpdateSelected(MapGenerator._currentGoTile); break;
                case SceneToLoad.COMBAT: 
                case SceneToLoad.TUTORIAL:
                    ManagerGame?.SetDices(); break;
            }
        }
    }
    #endregion
    public IEnumerator IScaleFader(bool load, string msg)
    {
        float scale = load ? 0 : _scale;
        if (load)
        {
            _pnlLoading.SetActive(true);
            IsLoading = true;
            while (_rtFader.localScale.x < _scale)
            {
                yield return null;
                scale += Time.deltaTime * _speed;
                _rtFader.localScale = new Vector3(scale, scale, scale);
            }
            //LoadMessage(true);
            _animLoad.SetBool("load", true);
        }
        else
        {
            //LoadMessage(false);
            _animLoad.SetBool("load", false);
            while (_rtFader.localScale.x > 0)
            {
                yield return null;
                scale -= Time.deltaTime * _speed;
                _rtFader.localScale = new Vector3(scale, scale, scale);
            }
            _rtFader.localScale = Vector3.zero;
            IsLoading = false;
            _pnlLoading.SetActive(false);
        }
    }
}
