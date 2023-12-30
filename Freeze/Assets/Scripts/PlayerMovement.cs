using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[SelectionBase]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 10f;

    [Header("Jumping")]
    [SerializeField] private float _jumpForce = 12;
    [SerializeField] private float _jumpCooldown = 0.2f;
    [SerializeField] private float _airMultiplier = 0.4f;
    bool _readyToJump = true;

    [Header("Ground Settings")]
    [SerializeField] private float _playerHeight = 2f;
    [SerializeField] LayerMask _groundMask;
    [SerializeField] private float _groundDrag = 5f;
    [SerializeField] private Transform[] _points;
    bool _grounded;

    [SerializeField] private Transform _orientation;
    float _horizontalInput;
    float _verticalInput;

    Vector3 _moveDirection;

    Rigidbody _rb;

    private void Awake()
    {
        if (_rb == null)
            _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    private void Update()
    {
        MyInput();
        int hits = 0;
        foreach (Transform t in _points)
            if (Physics.Raycast(t.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _groundMask))
                hits++;
        _grounded = hits > 0;

        if (_grounded)
            _rb.drag = _groundDrag;
        else
            _rb.drag = 0f;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        SpeedControl();
    }

    void MyInput()
    {
        Vector2 moveAxis = InputManager.instance.GetPlayerMovement();
        _horizontalInput = moveAxis.x;
        _verticalInput = moveAxis.y;

        if (InputManager.instance.PlayerJumped() && _readyToJump && _grounded)
        {
            _readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), _jumpCooldown);
        }
    }

    void MovePlayer()
    {
        _moveDirection = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;

        if (_grounded)
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);
        else if (!_grounded)
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f * _airMultiplier, ForceMode.Force);

    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        flatVel = Vector3.ClampMagnitude(flatVel, _moveSpeed);
        _rb.velocity = new Vector3(flatVel.x, _rb.velocity.y, flatVel.z);

    }

    void Jump()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        _readyToJump = true;
    }
}
