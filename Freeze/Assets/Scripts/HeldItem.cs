using Assets.Scripts;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class HeldItem : MonoBehaviour, IInteractable, IRespawn, IKey
{
    private Collider _collider;
    private static Collider _playerCollider;
    private Rigidbody _rb;

    // Status
    private bool _held = false;
    private bool _frozen = false;

    [Header("Freeze Settings")]
    [SerializeField] private bool _pickUpOnFreeze = true;
    [SerializeField] private ParticleSystem _freezeParticles;
    [SerializeField] private float _freezeDuration = 5f;
    [SerializeField] private int _freezeThreshold = 5;
    private float _elapsedTime = 0;

    public Action OnUnfreeze;

    [Header("Other")]
    [SerializeField] private int _ID = -1;

    // Respawn settings
    private Vector3 _startPos;
    private const float _yLimit = -100f;

    // Sound Settings
    private AudioPlayer _audioPlayer;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();
        _audioPlayer = GetComponentInChildren<AudioPlayer>();
        if (_playerCollider == null)
            _playerCollider = GameObject.FindWithTag("Player").GetComponentInChildren<Collider>();
    }
    void Start()
    {
        _startPos = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
        {
            if (_audioPlayer == null || IsFrozen() || IsHeld())
                return;
            _audioPlayer.Play("Thud");
        }
    }

    void Update()
    {
        if (transform.position.y < _yLimit)
        {
            Respawn();
        }
    }
    public void OnHold()
    {
        if (!_pickUpOnFreeze && IsFrozen())
            return;

        _held = true;
        _rb.isKinematic = true;
        Physics.IgnoreCollision(_collider, _playerCollider, true);
        _rb.useGravity = false;
    }
    public void OnDrop()
    {
        _held = false;
        _rb.useGravity = true;
        _rb.isKinematic = false;
        Physics.IgnoreCollision(_collider, _playerCollider, false);
    }

    public void OnFreeze()
    {
        _frozen = true;
        if (_audioPlayer != null)
            _audioPlayer.Play("Freeze", 0.25f, true);
        _rb.constraints = RigidbodyConstraints.FreezeAll;
        _freezeParticles.Play();
        if (IsHeld())
            InputManager.GrabDrop?.Invoke();
        //OnDrop();
        StartCoroutine(Freeze());
    }

    public void Unfreeze()
    {
        StopAllCoroutines();
        OnDefrost();
    }

    private void OnDefrost()
    {
        OnUnfreeze?.Invoke();
        _elapsedTime = _freezeDuration;
        _freezeParticles.Stop();
        _rb.constraints = RigidbodyConstraints.None;
        _frozen = false;
    }

    private IEnumerator Freeze()
    {
        _elapsedTime = 0f;
        while (_elapsedTime < _freezeDuration)
        {
            _elapsedTime += Time.deltaTime;
            yield return null;
        }
        OnDefrost();
    }

    public void AddFreezeTime(float seconds)
    {
        float result = _elapsedTime - seconds;

        if (result > 0)
            _elapsedTime -= seconds;
        else
            _elapsedTime = 0f;
    }
    public void OnInteract() { }
    public void Respawn()
    {
        Debug.Log("Respawning object " + transform);
        if (!_rb.isKinematic)
            _rb.velocity = Vector3.zero;
        transform.position = _startPos;
        transform.rotation = Quaternion.identity;
        if (IsHeld())
            InputManager.GrabDrop?.Invoke();
    }
    // Getters
    public bool IsHeld() { return _held; }
    public bool IsFrozen() { return _frozen; }

    public int GetID()
    {
        return _ID;
    }

    public int GetThreshold() { return _freezeThreshold; }

    public bool IsInteractable()
    {
        if (_pickUpOnFreeze)
            return true;

        bool result = !_pickUpOnFreeze && !IsFrozen();
        //Debug.Log("Result: " + result);
        return result;
    }
}