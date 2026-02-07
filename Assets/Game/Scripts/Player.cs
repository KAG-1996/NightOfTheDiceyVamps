using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player instance;
    public Vector3 _targetPos;
    public GameObject _body;
    [SerializeField] private float _posY;
    [SerializeField] private float _speed;

    public Rigidbody _rig3D;
    public float _movSpeed;
    public Vector2 _movDir;
    public InputActionReference _move, _fire;

    public void Start()
    {
        instance = this;
        //StartCoroutine(MoveTo());
    }
    private void Update()
     {
        //Controls();
        /* return;
        if (GameManager.instance.GamePhase == GameManager.State.PLAYING)
        {
            if (GameManager.instance._distance > 0 && GameManager.instance._timer > 0)
            {
                Controls();
            }
        }
        else if (GameManager.instance.GamePhase == GameManager.State.END)
        {
            if (GameManager._isWin)
            {
                _targetPos = new Vector2(0, -1);
            }
            else
            {
                _targetPos = new Vector2(0, 3);
            }
        }
        else if (GameManager.instance.GamePhase == GameManager.State.MENU)
        {
            _targetPos = new Vector2(-12, -1);
            gameObject.transform.position = new Vector2(-12, -1);
        }*/
        //UpdatePosition();
        //transform.localRotation = Quaternion.Euler(0f, Mathf.Lerp(110, 70, RRR()), 0f);
    }
    public bool _isMove = false;
    private void UpdatePosition()
    {
        _isMove = transform.position.z == _targetPos.z;
        if (!_isMove) 
        {
            if (transform.position.z < _targetPos.z) transform.localRotation = Quaternion.Euler(0f, Mathf.Lerp(90, 70, RRR()), 0f);
            else if (transform.position.z > _targetPos.z) transform.localRotation = Quaternion.Euler(0f, Mathf.Lerp(90, 110, RRR()), 0f);
        }else transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, _speed * Time.deltaTime);
    }
    float RRR()
    {
        if (_movDir.y > 0f) return 1f;
        else if (_movDir.y < 0f) return 0f;
        else return 0.5f;
    }
    IEnumerator ISteering()
    {
        while(transform.position.y != _targetPos.y)
        {
            yield return null; 
            transform.localRotation = Quaternion.Euler(0f, Mathf.Lerp(110, 70, RRR()), 0f);
        }
    }
    IEnumerator MoveTo()
    {
        gameObject.transform.position = new Vector2(-12, -1);
        yield return new WaitForSecondsRealtime(0.01f);
        _targetPos = new Vector2(-12, -1);
    }
    #region CalledBySchemes
    //Name´s methods always must be On+{ActionName} to be called by InputSys and work 

    private void FixedUpdate()
    {
        Vector2 result = _movDir * _movSpeed * Time.fixedDeltaTime;
        _rig3D.velocity = new(result.x, _rig3D.velocity.y, result.y);
    }
    private void OnMove(InputValue value)
    {
        _movDir = value.Get<Vector2>();
        //_rig2D.velocity = result;
        ZDebug.Log($"vel {_rig3D.velocity}");
        if (_movDir.y == 1 || _movDir.y == -1)
        {
            /*if (Mathf.RoundToInt(_movDir.y) == 1 && index < GameManager.instance._tRails.Length - 1) index++;
            if (Mathf.RoundToInt(_movDir.y) == -1 && index > 0) index--;
            GameManager.instance._tRailCurrent = GameManager.instance._tRails[index];
            _targetPos = new(_targetPos.x, _targetPos.y, GameManager.instance._tRailCurrent.position.z);*/
            /*StopCoroutine("ISteering");
            StartCoroutine(ISteering());*/
        }
        //ZDebug.Log($"{_movDir}");
    }
    /*void OnEnable()
    {
        _move.action.started += OnMoveStarted;
        _move.action.performed += OnMovePerformed;
        _move.action.canceled += OnMoveCanceled;
    }

    void OnDisable()
    {
        _move.action.started -= OnMoveStarted;
        _move.action.performed -= OnMovePerformed;
        _move.action.canceled -= OnMoveCanceled;
    }*/
    private void OnMoveStarted(InputAction.CallbackContext context) => ZDebug.Log("Move started (charging)");  
    private void OnMovePerformed(InputAction.CallbackContext context) => ZDebug.Log("Move performed (charged shot fired!)");  
    private void OnMoveCanceled(InputAction.CallbackContext context) => ZDebug.Log("Move canceled (input released)");

    private void OnFire(InputValue value) => ZDebug.Log("Regular");
    private void OnHold(InputValue value) => ZDebug.Log("Hold");
    #endregion
    void MoveLogic()
    {
        Vector2 result = _movDir * _movSpeed * Time.fixedDeltaTime;
        //_rig2D.velocity = result;
        _rig3D.velocity = new(0, 0, result.y);
    }
    private void OnTriggerEnter(Collider col)
    {
        switch(col.tag)
        {
            case "Finish":
                SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
                break;
        }
    }
}
