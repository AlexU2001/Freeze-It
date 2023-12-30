using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    private static InputManager _instance;

    public static InputManager instance
    {
        get { return _instance; }
    }
    private bool _jumpHeld;
    private PlayerControls _playerControls;
    public static Action GrabDrop;
    public static Action FreezeUnfreeze;
    public static Action PauseUnpause;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else
            _instance = this;
        _playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        _playerControls.Enable();
        _playerControls.Player.Jump.performed += ctx => _jumpHeld = true;
        _playerControls.Player.Jump.canceled += ctx => _jumpHeld = false;
        _playerControls.Player.GrabDrop.performed += ctx => OnGrabDrop();
        _playerControls.Player.FreezeUnfreeze.performed += ctx => OnFreezeUnfreeze();
        _playerControls.Player.Restart.performed += ctx => ResetLevel();
        _playerControls.Player.Pause.performed += ctx => OnPauseUnpause();
    }

    private void OnDisable()
    {
        _playerControls.Player.Jump.performed -= ctx => _jumpHeld = true;
        _playerControls.Player.Jump.canceled -= ctx => _jumpHeld = false;
        _playerControls.Player.GrabDrop.performed -= ctx => OnGrabDrop();
        _playerControls.Player.FreezeUnfreeze.performed -= ctx => OnFreezeUnfreeze();
        _playerControls.Player.Restart.performed -= ctx => ResetLevel();
        _playerControls.Player.Pause.performed -= ctx => OnPauseUnpause();
        _playerControls.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return _playerControls.Player.Movement.ReadValue<Vector2>();
    }
    public Vector2 GetMouseDelta()
    {
        return _playerControls.Player.Look.ReadValue<Vector2>();
    }

    // This frame
    public bool PlayerJumped()
    {
        return _jumpHeld;
    }

    private void OnGrabDrop()
    {
        GrabDrop?.Invoke();
    }

    private void OnFreezeUnfreeze()
    {
        FreezeUnfreeze?.Invoke();
    }
    public PlayerControls GetPlayerControls()
    {
        return _playerControls;
    }

    public void ResetLevel()
    {
        SceneController.instance.RestartScene();
    }

    private void OnPauseUnpause()
    {
        Debug.Log("Pausing");
        PauseUnpause?.Invoke();
    }
}
