using UnityEngine;

public class RespawnTarget : MonoBehaviour, IRespawn
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform _respawnTarget;
    private Vector3 _startingPosition;

    public static float yLimit = -100;

    private void Start()
    {
        _startingPosition = _respawnTarget.position;
    }
    public void Respawn()
    {
        if (_respawnTarget.CompareTag("Player"))
            InputManager.GrabDrop?.Invoke();
        _respawnTarget.transform.position = _startingPosition;
        _rb.velocity = Vector3.zero;
    }

    void Update()
    {
        if (_respawnTarget.transform.position.y < yLimit)
            Respawn();
    }
}
