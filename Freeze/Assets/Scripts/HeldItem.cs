using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class HeldItem : MonoBehaviour, IInteractable, IRespawn, IKey
{
    private Collider _collider;
    private Rigidbody _rb;

    [Header("Hold Settings")]
    [SerializeField] private float _freezeDuration = 5f;
    [SerializeField] private ParticleSystem _freezeParticles;

    private bool _held = false;
    private bool _frozen = false;

    [Header("Other")]
    [SerializeField] private int _ID = -1;

    private Vector3 _startPos;
    private const float _yLimit = -100f;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        _startPos = transform.position;
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
        _freezeParticles.Stop();
        _rb.constraints = RigidbodyConstraints.None;
        _frozen = false;
    }

    private IEnumerator Freeze()
    {
        float elapsedTime = 0f;
        while (elapsedTime < _freezeDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        OnDefrost();
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