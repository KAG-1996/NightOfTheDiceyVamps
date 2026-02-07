using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;
    public float _speedTransition, _timeReplayIntro;
    public GameObject _goAny, _goNew, _goSettings, _goDificult;
    public Button _btnLoad, _btnAny;
    public Toggle _toggleScreen, _tggRumble;
    public Speech _speechChallenges;
    //public TextMeshProUGUI _txtAnyKey;
    public LocalizeStringEvent _locEventPressAny;
    public LocalizedString _locKey, _locClick;
    public CanvasGroup _currentCG, _cgSettings;
    public CanvasGroup _cgMain;
    public TMP_Dropdown _ddIdiom;
    public LocalizeStringEvent _locEvent;
    public CanvasGroup[] _canvasGroups;
    public TransitionPanels[] _tp;
    public SliderContent[] _sliderCtns;

    private IEnumerator _iReplayIntro;

    SaveData Data => ManagerData.Instance._saveData;
    ManagerInputCalls InputCall => ManagerInputCalls.Instance;
    private void Awake()
    {
        Instance = this;
        foreach (var a in _canvasGroups) a.gameObject.SetActive(false);
        _currentCG.gameObject.SetActive(true);
        _iReplayIntro = IReplayIntro();
        foreach (var a in _sliderCtns)
        {
            switch (a._data)
            {
                case DataProccess.VOLUME_MASTER: a._slider.value = Data._settingsData._audioMaster; break;
                case DataProccess.VOLUME_MUSIC: a._slider.value = Data._settingsData._auidioMusic; break;
                case DataProccess.VOLUME_SFX: a._slider.value = Data._settingsData._audioSFX; break;
            }
            switch (a._data)
            {
                default: a._txtValue.SetText(a._slider.value.ToString()); break;
                case DataProccess.VOLUME_MASTER:
                case DataProccess.VOLUME_MUSIC:
                case DataProccess.VOLUME_SFX:
                    a._txtValue.SetText($"{(int)(a._slider.value * 100)}");
                    break;
                case DataProccess.NONE:
                    a._slider.value = 1;
                    //a._txtValue.SetText(_speechChallenges._speechLines[(int)a._slider.value]._desc);
                    break;
            }
        }
        _ddIdiom.value = Data._settingsData._idiom;
        SetFirstSelected();
    }
    private void OnDestroy()
    {
        Instance = null;
    }
    void Start()
    {
        _btnLoad.interactable = Data._mapData.Count > 0;
        StartCoroutine(_iReplayIntro);
    }
    private void LateUpdate()
    {
        _locEventPressAny.StringReference = InputCall._input.currentControlScheme == "Gamepad" ? _locKey : _locClick;
    }
    public void SetFirstSelected()
    {
        if (_currentCG == _canvasGroups[0]) InputCall.UpdateSelected(_goAny);
        if (_currentCG == _canvasGroups[1]) InputCall.UpdateSelected(Data._mapData.Count > 0 ? _btnLoad.gameObject : _goNew);
        if (_currentCG == _canvasGroups[2]) InputCall.UpdateSelected(_goDificult);
        if (_currentCG == _canvasGroups[3]) InputCall.UpdateSelected(_goSettings);
    }
    public void Z_Value(Slider slider) 
    {
        foreach(var a in _sliderCtns)
        {
            if (a._slider == slider)
            {
                switch (a._data)
                {
                    case DataProccess.VOLUME_MASTER: Data._settingsData._audioMaster = slider.value; break;
                    case DataProccess.VOLUME_MUSIC: Data._settingsData._auidioMusic = slider.value; break;
                    case DataProccess.VOLUME_SFX: Data._settingsData._audioSFX = slider.value; break;
                }
                switch (a._data)
                {
                    default: a._txtValue.SetText($"{slider.value}"); break;
                    case DataProccess.VOLUME_MASTER: 
                    case DataProccess.VOLUME_MUSIC: 
                    case DataProccess.VOLUME_SFX: 
                        a._txtValue.SetText($"{(int)(slider.value * 100)}"); break;
                }
                switch (a._data)
                {
                    case DataProccess.VOLUME_MASTER:
                    case DataProccess.VOLUME_MUSIC:
                    case DataProccess.VOLUME_SFX:
                        if (ManagerAudio.Instance) ManagerAudio.Instance.UpdateAudioVolume();
                        break;
                }

            }
        }
    }
    public void Z_StartGame(bool load)
    {
        if (load)
        {
            if (Data._mapData == null || Data._mapData.Count.Equals(0))
            {
                ZDebug.Log("Not Data to load current game");
                return;
            }
            else ManagerScenes._loadGame = true; 
        }
        else ManagerScenes._loadGame = false;
        ManagerScenes.Instance.UnLoadScene(SceneToLoad.MAP);
    }
    public void Z_SetChallenge(int challenge)
    {
        MapGenerator.Challenge = challenge;
        Z_StartGame(false);
    }
    public void Z_SetChallenge(Slider slider) 
    {
        MapGenerator.Challenge = (int)slider.value;
        foreach (var a in _sliderCtns) 
            if (a._data == DataProccess.NONE)
                _locEvent.StringReference = _speechChallenges._speechLines[(int)slider.value]._locString;
    }
    public void Z_FullScreen()
    {
        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.FullScreenWindow:
            case FullScreenMode.ExclusiveFullScreen:
            case FullScreenMode.MaximizedWindow:
                Screen.SetResolution(1080, 720, FullScreenMode.Windowed);
                break;
            case FullScreenMode.Windowed:
                Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.FullScreenWindow);
                break;
        }
        //Data._settingsData._fullScreen = Screen.fullScreenMode != FullScreenMode.Windowed;
    }
    public void Z_Localize()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_ddIdiom.value];
        Data._settingsData._idiom = _ddIdiom.value;
    }
    public void Z_Rumble()
    {
        Data._settingsData._canRumble = _tggRumble.isOn;
        InputCall.TriggerRumble(TypeRumble.ATTACK);
    }
    public void Z_SaveSettings() => ManagerData.Instance.Save();
    public void Z_UpdateSelected(GameObject go) => InputCall.UpdateSelected(go);
    public void Z_BackToIdioms() => InputCall.UpdateSelected(_ddIdiom.gameObject);
    public void Z_StartTutorial() => ManagerScenes.Instance.UnLoadScene(SceneToLoad.TUTORIAL);
    public void Z_LoadScene() => ManagerScenes.Instance.UnLoadScene(SceneToLoad.MAP);
    public void Z_Credits() => ManagerScenes.Instance.UnLoadScene(SceneToLoad.CREDITS);
    public void Z_CanvasTransition(CanvasGroup cg) => StartCoroutine(ICanvasTransition(_currentCG, cg));
    public void Z_SettingsTransition(CanvasGroup cg) => StartCoroutine(ISettingsTransition(_cgSettings, cg));
    public void Z_StopReplay() => StopCoroutine(_iReplayIntro);
    public void Z_QuitGame() => Application.Quit();
    IEnumerator IReplayIntro()
    {
        yield return new WaitForSeconds(_timeReplayIntro);
        _btnAny.interactable = false;
        ManagerScenes.Instance.UnLoadScene(SceneToLoad.STORY);
    }
    IEnumerator ICanvasTransition(CanvasGroup cgOut, CanvasGroup cgIn, float t = 1f)
    {
        while (cgOut.alpha > 0)
        {
            yield return null;
            t -= _speedTransition * Time.deltaTime;
            cgOut.alpha = t;
        }
        cgOut.gameObject.SetActive(false);
        cgIn.gameObject.SetActive(true);
        while (cgIn.alpha < 1)
        {
            yield return null;
            t += _speedTransition * Time.deltaTime;
            cgIn.alpha = t;
        }
        _currentCG = cgIn;
        yield return new WaitForSeconds(0.25f);
        SetFirstSelected();
    }
    IEnumerator ISettingsTransition(CanvasGroup cgOut, CanvasGroup cgIn, float t = 1f)
    {
        if (cgIn == _cgSettings) yield break;
        while (cgOut.alpha > 0)
        {
            yield return null;
            t -= _speedTransition * Time.deltaTime;
            cgOut.alpha = t;
        }
        cgOut.gameObject.SetActive(false);
        cgIn.gameObject.SetActive(true);
        while (cgIn.alpha < 1)
        {
            yield return null;
            t += _speedTransition * Time.deltaTime;
            cgIn.alpha = t;
        }
        _cgSettings = cgIn;
        /*yield return new WaitForSeconds(0.25f);
        SetFirstSelected();*/
    }
}
