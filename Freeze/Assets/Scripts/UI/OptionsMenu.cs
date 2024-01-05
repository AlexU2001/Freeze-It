using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
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

    [SerializeField] private AudioMixerGroup[] _mixers;
    [SerializeField] private Slider[] _volumeSliders;

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
        HideAll(_content[0]);
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

        UpdateResolutionText();
    }
    public void LoadVolume()
    {
        float value;
        for (int i = 0; i < _mixers.Length; i++)
        {
            if (PlayerPrefs.HasKey(_mixers[i].name))
                value = PlayerPrefs.GetFloat(_mixers[i].name);
            else
                value = 0.5f;
            _volumeSliders[i].value = value;
            //Debug.Log(string.Format("Got float {1} for {0}", _sliders[i].name, value));
            UpdateMixer(i);
        }
    }
    public void SelectTab(GameObject self)
    {
        HideAll(self);
        if (!self.activeSelf)
            self.SetActive(true);
    }

    private void HideAll(GameObject exception)
    {
        foreach (var content in _content)
        {
            if (content == null)
                continue;
            if (exception != null && content.Equals(exception))
                continue;
            content.gameObject.SetActive(false);
        }
    }
    private void UpdateResolutionText()
    {
        _resolutionTxt.text = _resolution.Get().ToString();
    }
    public void ApplyGraphics()
    {
        if (_vSync.isOn)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;
        Resolution res = _resolution.Get();
        Screen.SetResolution(res.width, res.height, _fullScreen.isOn);
    }

    public void DisplayNext(int target)
    {
        switch (target)
        {
            case 0:
                _resolution.Next();
                break;
        }
        UpdateResolutionText();
    }

    public void DisplayPrevious(int target)
    {
        switch (target)
        {
            case 0:
                _resolution.Previous();
                break;
        }
        UpdateResolutionText();
    }

    public void ApplySensitivityChange()
    {
        PlayerPrefs.SetFloat("SensX", _sensSlider.value);
        PlayerPrefs.SetFloat("SensY", _sensSlider.value);

        SensSlider?.Invoke(_sensSlider.value);
        _sensText.text = "Sensitivity: " + _sensSlider.value;
    }
    public void UpdateMixer(int i)
    {
        float value = _volumeSliders[i].value;
        if (value > 0)
            _mixers[i].audioMixer.SetFloat("Volume" + i, Mathf.Log10(value) * 20);
        else
            _mixers[i].audioMixer.SetFloat("Volume" + i, -80);
        PlayerPrefs.SetFloat(_mixers[i].name, value);
        //Debug.Log(string.Format("Set float for {0} to {1}", _mixers[i].name, value));
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
            if (index < 0)
                index = options.Count - 1;
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
