using Assets.Scripts;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class HeldItem : MonoBehaviour, IInteractable, IRespawn, IKey
{
    private Collider _collider;
    private Rigidbody _rb;

    // Status
    private bool _held = false;
    private bool _frozen = false;

    [Header("Freeze Settings")]
    [SerializeField] private ParticleSystem _freezeParticles;
    [SerializeField] private float _freezeDuration = 5f;
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
    }
    void Start()
    {
        _startPos = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
        {
            if (_audioPlayer != null && !_frozen)
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
        _held = true;
        _collider.enabled = false;
        _rb.useGravity = false;
        _rb.velocity = Vector3.zero;
    }
    public void OnDrop()
    {
        _held = false;
        _collider.enabled = true;
        _rb.useGravity = true;
    }

    public void OnFreeze()
    {
        _frozen = true;
        if (_audioPlayer != null)
            _audioPlayer.Play("Freeze", 0.25f, true);
        _rb.constraints = RigidbodyConstraints.FreezeAll;
        OnDrop();
        _freezeParticles.Play();
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

    public bool IsHeld() { return _held; }
    public bool IsFrozen() { return _frozen; }
    public void OnInteract() { }
    public void Respawn()
    {
        Debug.Log("Respawning object " + transform);
        _rb.velocity = Vector3.zero;
        transform.position = _startPos;
        transform.rotation = Quaternion.identity;
    }

    public int GetID()
    {
        return _ID;
    }
}