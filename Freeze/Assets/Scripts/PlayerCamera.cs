using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float sensX = 20;
    [SerializeField] private float sensY = 20;

    [SerializeField] private Transform _orientation;
    [SerializeField] private Transform _handSlot;

    float xRotation;
    float yRotation;

    private bool _paused = false;
    private void Start()
    {
        LoadFromPrefs();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        OptionsMenu.SensSlider += UpdateSens;
        OptionsMenu.PausePressed += OnPauseUnpause;
    }

    private void OnDisable()
    {
        OptionsMenu.SensSlider -= UpdateSens;
        OptionsMenu.PausePressed -= OnPauseUnpause;
    }

    private void Update()
    {
        // Using cinemachine rn instead
        MoveCamera();
    }

    private void MoveCamera()
    {
        if (_paused)
            return;

        Vector2 mouseDelta = InputManager.instance.GetMouseDelta();
        float mouseX = mouseDelta.x * Time.deltaTime * sensX;
        float mouseY = mouseDelta.y * Time.deltaTime * sensX;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        _orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
        _handSlot.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    private void OnPauseUnpause()
    {
        _paused = !_paused;
    }

    private void UpdateSens(float sens)
    {
        sensX = sens;
        sensY = sens;
    }

    public void LoadFromPrefs()
    {
        if (PlayerPrefs.HasKey("SensX"))
            sensX = PlayerPrefs.GetFloat("SensX");
        if (PlayerPrefs.HasKey("SensY"))
            sensY = PlayerPrefs.GetFloat("SensY");
    }
}
