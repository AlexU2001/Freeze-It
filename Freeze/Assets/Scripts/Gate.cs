using System.Collections;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] private int _ID = 0;
    [SerializeField] private float _unitsToMove = 3f;
    [SerializeField] private float _speed = 3f;
    [SerializeField] private bool _isPlatform = false;
    private Vector3 _closedPos;
    private Vector3 _openPos;

    private Coroutine _coroutine;

    private void Start()
    {
        _closedPos = transform.localPosition;
        _openPos = new Vector3(_closedPos.x, _closedPos.y + _unitsToMove, _closedPos.z);
        Debug.Log("Closed: " + _closedPos + " Open: " + _openPos);
    }

    private void OnEnable()
    {
        PressurePlate.Pressured += Open;
        PressurePlate.Unpressured += Close;
    }

    private void OnDisable()
    {
        PressurePlate.Pressured -= Open;
        PressurePlate.Unpressured -= Close;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_isPlatform)
            return;

        if (!collision.gameObject.CompareTag("Player"))
            return;

        collision.transform.SetParent(transform);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!_isPlatform)
            return;

        if (!collision.gameObject.CompareTag("Player"))
            return;

        collision.transform.SetParent(null);
    }

    private void Open(int id)
    {
        if (id != _ID)
            return;
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(OpenClose(_openPos));
    }

    private void Close(int id)
    {
        if (id != _ID)
            return;
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(OpenClose(_closedPos));
    }

    private IEnumerator OpenClose(Vector3 targetPos)
    {
        Vector3 currentPos = transform.localPosition;
        float duration = Mathf.Abs(_unitsToMove) / _speed;
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
