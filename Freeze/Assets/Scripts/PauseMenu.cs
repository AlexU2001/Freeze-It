using System;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static Action PausePressed;

    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _optionsPanel;
    [SerializeField] private GameObject _crosshair;
    private bool _paused = false;
    private void OnEnable()
    {
        InputManager.PauseUnpause += Pause;
    }
    private void OnDisable()
    {
        InputManager.PauseUnpause -= Pause;
    }
    public void Pause()
    {
        PausePressed?.Invoke();
        if (_pausePanel == null)
            return;

        _paused = !_paused;
        if (_paused)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
        }
        else
        {
            _optionsPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f;
        }
        Cursor.visible = _paused;
        _pausePanel.SetActive(_paused);
        if (_crosshair != null)
            _crosshair.SetActive(!_paused);
    }
    public void GoToMainMenu()
    {
        SceneController.instance.GoToLevel(0);
    }
}
