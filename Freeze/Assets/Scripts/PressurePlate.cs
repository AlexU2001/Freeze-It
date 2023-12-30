using System;
using System.Collections;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public static Action<int> Pressured;
    public static Action<int> Unpressured;

    [Header("Function")]
    [SerializeField] private int _ID = 0;
    [SerializeField] private bool _active = false;
    [SerializeField] private bool _requireID = false;

    [Header("Animation")]
    [SerializeField] private float _unitsToMove = 3f;
    [SerializeField] private float _speed = 3f;
    private Coroutine _doorRoutine;

    private Vector3 _defaultPos;
    private Vector3 _triggeredPos;
    private void Start()
    {
        _defaultPos = transform.localPosition;
        _triggeredPos = new Vector3(_defaultPos.x, _defaultPos.y + _unitsToMove, _defaultPos.z);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Trigger"))
            return;

        if (_requireID && !collision.gameObject.CompareTag("Player"))
        {
            IKey key;
            if (collision.gameObject.TryGetComponent(out key))
                if (key.GetID() != _ID)
                    return;

            if (key == null)
                return;
        }

        if (!_active)
            TriggerPressure(collision);
    }

    private void Update()
    {
        if (_active && transform.childCount <= 0)
        {
            UntriggerPressure(null);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (_active)
            UntriggerPressure(collision);
    }

    void TriggerPressure(Collision collision)
    {
        _active = true;
        collision.transform.SetParent(transform);

        if (_doorRoutine != null)
            StopCoroutine(_doorRoutine);
        _doorRoutine = StartCoroutine(OpenClose(_triggeredPos));

        Pressured?.Invoke(_ID);
    }

    private void UntriggerPressure(Collision collision)
    {
        _active = false;
        if (collision != null)
            collision.transform.SetParent(null);

        if (_doorRoutine != null)
            StopCoroutine(_doorRoutine);
        _doorRoutine = StartCoroutine(OpenClose(_defaultPos));

        Unpressured?.Invoke(_ID);
    }
    private IEnumerator OpenClose(Vector3 targetPos)
    {
        Vector3 currentPos = transform.localPosition;
        float duration = _unitsToMove / _speed;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            Vector3 pos = Vector3.Lerp(currentPos, targetPos, elapsedTime / duration);
            transform.localPosition = pos;
            yield return null;
        }
        transform.localPosition = targetPos;
    }
}
