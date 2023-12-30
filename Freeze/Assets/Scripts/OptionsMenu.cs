using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider _sensSlider;
    public static Action<float> SensSlider;
    public static Action PausePressed;

    [SerializeField] private TextMeshProUGUI _sensText;
    [SerializeField] private GameObject _panel;
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
        if (_panel == null)
            return;

        _paused = !_paused;
        if (_paused)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f;
        }
        Cursor.visible = _paused;
        _panel.SetActive(_paused);
        if (_crosshair != null)
            _crosshair.SetActive(!_paused);
    }
    public void ApplyChange()
    {
        PlayerPrefs.SetFloat("SensX", _sensSlider.value);
        PlayerPrefs.SetFloat("SensY", _sensSlider.value);

        SensSlider?.Invoke(_sensSlider.value);
        _sensText.text = "Sensitivity: " + _sensSlider.value;
    }
}
