using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInput : MonoBehaviour
{
    public static TestInput Instance;
    public bool _op;
    private PlayerInput _player;
    InputAction inputAction;

    private void Awake()
    {
        Instance = this;

        _player = GetComponent<PlayerInput>();
        inputAction = _player.actions["Cancel"];
    }
    private void Update()
    {
        _op = inputAction.IsPressed();
    }
}
