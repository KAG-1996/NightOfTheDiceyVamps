using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine.Windows;

public class ManagerInputCalls : MonoBehaviour
{
    public static ManagerInputCalls Instance;

    public PlayerInput _input;
    public GameObject _goLast;

    public RumbleValues[] Rumbles;
    SaveData Data => ManagerData.Instance._saveData;
    ManagerGame ManagerGame => ManagerGame.Instance;
    MapGenerator MapGenerator => MapGenerator.Instance;
    ManagerLoadingScreen LoadingScreen => ManagerLoadingScreen.Instance;

    private void Awake() => Instance = this;

    [ContextMenu("SET ID")]
    void SetId()
    {
        for (int i = 0; i < Rumbles.Length; i++) Rumbles[i]._ID = Rumbles[i]._typeRumble.ToString();
    }
    private void LateUpdate()
    {
        CheckGamepad();
        /*foreach (var gamepad in Gamepad.all)
        {
            if (gamepad is DualShockGamepad)
            {
                Debug.Log("PlayStation 4 (DualShock) controller detected: " + gamepad.displayName);
            }
            else if (gamepad is DualSenseGamepadHID)
            {
                Debug.Log("PlayStation 5 (DualSense) controller detected: " + gamepad.displayName);
            }
            else if (gamepad is XInputController)
            {
                Debug.Log("Xbox controller detected: " + gamepad.displayName);
            }
            else
            {
                Debug.Log("Other gamepad detected: " + gamepad.displayName);
            }
        }*/
    }
    void CheckGamepad()
    {
        Cursor.visible = _input.currentControlScheme == "Keyboard&Mouse";
        Cursor.lockState = _input.currentControlScheme == "Keyboard&Mouse" ? CursorLockMode.None : CursorLockMode.Locked;
        if (_input.currentControlScheme != "Gamepad") return;
        if (!EventSystem.current.currentSelectedGameObject)
        {
            EventSystem.current.SetSelectedGameObject(_goLast);
        }
    }

    public void UpdateSelected(GameObject go) 
    {
        _goLast = go;
        switch (_input.currentControlScheme)
        {
            case "Gamepad": EventSystem.current.SetSelectedGameObject(_goLast); break;
            case "Keyboard&Mouse": EventSystem.current.SetSelectedGameObject(null); break;
        }
    }
    public void TriggerRumbleIndef(bool stop, TypeRumble rumble = TypeRumble.NONE)
    {
        if (!Data._settingsData._canRumble) return;
        if (stop) Gamepad.current?.SetMotorSpeeds(0f, 0f); // Stop all vibration
        else
        {
            foreach (var a in Rumbles)
            {
                if (a._typeRumble == rumble) Gamepad.current?.SetMotorSpeeds(a._lowFreq, a._highFreq); 
            }
        }
    }
    public void TriggerRumble(TypeRumble rumble) => StartCoroutine(ITriggerRumble(rumble));

    #region Input Calls
    private void OnInfo(InputValue value)
    {
        switch (LoadingScreen._currentScene)
        {
            case SceneToLoad.COMBAT:
            case SceneToLoad.TUTORIAL:
                ManagerGame?.ShowInfo();
                break;
            case SceneToLoad.MAP: MapGenerator?.ShowInfo(); break;
        }
    }
    private void OnNext(InputValue value)
    {
        switch (LoadingScreen._currentScene)
        {
            case SceneToLoad.COMBAT:
            case SceneToLoad.TUTORIAL:
                ManagerGame?._infoScroll.Z_ChangePage(true);
                break;
            case SceneToLoad.MAP: MapGenerator?._infoScroll.Z_ChangePage(true); break;
        }
    }
    private void OnPrev(InputValue value)
    {
        switch (LoadingScreen._currentScene)
        {
            case SceneToLoad.COMBAT:
            case SceneToLoad.TUTORIAL:
                ManagerGame?._infoScroll.Z_ChangePage(false);
                break;
            case SceneToLoad.MAP: MapGenerator?._infoScroll.Z_ChangePage(false); break;
        }
    }

    private void OnRoll(InputValue value) => ManagerGame?.Z_Roll();
    private void OnShuffle(InputValue value) => ManagerGame?.Z_ReShuffle();
    private void OnPlay(InputValue value) => ManagerGame?.Z_AttackPhase();
    private void OnExit(InputValue value) => LoadingScreen?.Exit();
    #endregion
    #region Coroutines
    IEnumerator ITriggerRumble(TypeRumble rumble)
    {
        if (!Data._settingsData._canRumble) yield break;
        foreach (var a in Rumbles)
        {
            if (a._typeRumble == rumble)
            {
                Gamepad.current?.SetMotorSpeeds(a._lowFreq, a._highFreq);
                yield return new WaitForSeconds(a._rumble);
                Gamepad.current?.SetMotorSpeeds(0f, 0f); // Stop all vibration
            }
        }
    }
    #endregion
}
