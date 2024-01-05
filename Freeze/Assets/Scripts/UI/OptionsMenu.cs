using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Toggles")]
    [SerializeField] private Toggle _fullScreen;
    [SerializeField] private Toggle _vSync;

    [Header("Displays")]
    [SerializeField] private TextMeshProUGUI _resolutionTxt;
    private OptionsDisplay<Resolution> _resolution;

    [Header("Sliders")]
    [SerializeField] private TextMeshProUGUI _sensText;
    [SerializeField] private Slider _sensSlider;
    public static Action<float> SensSlider;

    // Other
    private List<GameObject> _content;

    private void Awake()
    {
        AddResolutions();
        FindContent();
    }
    private void OnEnable()
    {
        LoadPreferences();
        LoadGraphicsSettings();
    }
    private void AddResolutions()
    {
        if (_resolution == null)
            _resolution = new OptionsDisplay<Resolution>();
        if (_resolution.options.Count <= 0)
        {
            _resolution.AddToOptions(new Resolution(1920, 1080));
            _resolution.AddToOptions(new Resolution(1600, 900));
            _resolution.AddToOptions(new Resolution(1280, 720));
            _resolution.AddToOptions(new Resolution(854, 480));
        }
    }

    private void FindContent()
    {
        if (_content == null)
            _content = new List<GameObject>();
        foreach (var content in GameObject.FindGameObjectsWithTag("ContentUI"))
            _content.Add(content);
    }
    private void LoadPreferences()
    {
        if (PlayerPrefs.HasKey("SensX"))
            _sensSlider.value = PlayerPrefs.GetFloat("SensX");
        _sensText.text = "Sensitivity: " + _sensSlider.value;
    }

    private void LoadGraphicsSettings()
    {
        _fullScreen.isOn = Screen.fullScreen;
        _vSync.isOn = QualitySettings.vSyncCount > 0 ? true : false;

        bool foundRes = false;
        Resolution selectedRes = _resolution.Get();
        if (Screen.width != selectedRes.width || Screen.height != selectedRes.height)
        {
            for (int i = 0; i < _resolution.options.Count; i++)
            {
                if (_resolution.options[i].width == Screen.width && _resolution.options[i].height == Screen.height)
                {
                    _resolution.GoToIndex(i);
                    foundRes = true;
                    break;
                }
            }
        }

        if (!foundRes)
        {
            _resolution.AddToOptions(new Resolution(Screen.width, Screen.height));
            _resolution.GoToIndex(_resolution.options.Count - 1);
        }

        UpdateResolution();
    }

    private void UpdateResolution()
    {
        _resolutionTxt.text = _resolution.Get().ToString();
    }

    public void SelectTab(GameObject self)
    {
        HideAll(self);
        if (!self.activeSelf)
            self.SetActive(true);
    }

    private void HideAll(GameObject except)
    {
        foreach (var item in _content)
        {
            if (item.Equals(except))
                continue;
            item.gameObject.SetActive(false);
        }
    }

    public void DisplayNext(int target)
    {
        Debug.Log("Going to next");
        switch (target)
        {
            case 0:
                _resolution.Next();
                break;
        }
    }

    public void DisplayPrevious(int target)
    {
        Debug.Log("Going to previous");
        switch (target)
        {
            case 0:
                _resolution.Previous();
                break;
        }
    }

    public void ApplySensitivityChange()
    {
        PlayerPrefs.SetFloat("SensX", _sensSlider.value);
        PlayerPrefs.SetFloat("SensY", _sensSlider.value);

        SensSlider?.Invoke(_sensSlider.value);
        _sensText.text = "Sensitivity: " + _sensSlider.value;
    }
    public class Resolution
    {
        public int width;
        public int height;

        public Resolution(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
        public override string ToString()
        {
            return width + "x" + height;
        }
    }
    public class OptionsDisplay<T>
    {
        public int index { private set; get; }
        public List<T> options { private set; get; }

        public OptionsDisplay()
        {
            options = new List<T>();
            index = 0;
        }
        public void Next()
        {
            index++;
            index = index % options.Count;
        }
        public void Previous()
        {
            index--;
            index = index % options.Count;
        }
        public void GoToIndex(int index)
        {
            this.index = index;
        }

        public void AddToOptions(T newOption)
        {
            options.Add(newOption);
        }
        public T Get()
        {
            return options[index];
        }
    }
}
