using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider _sensSlider;
    public static Action<float> SensSlider;

    [SerializeField] private TextMeshProUGUI _sensText;

    private void OnEnable()
    {
        if (PlayerPrefs.HasKey("SensX"))
            _sensSlider.value = PlayerPrefs.GetFloat("SensX");
        _sensText.text = "Sensitivity: " + _sensSlider.value;
    }
    public void ApplyChange()
    {
        PlayerPrefs.SetFloat("SensX", _sensSlider.value);
        PlayerPrefs.SetFloat("SensY", _sensSlider.value);

        SensSlider?.Invoke(_sensSlider.value);
        _sensText.text = "Sensitivity: " + _sensSlider.value;
    }
}
