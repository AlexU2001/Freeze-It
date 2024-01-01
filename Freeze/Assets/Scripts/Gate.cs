using Assets.Scripts;
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
    private bool _stuck = false;
    private HeldItem _heldItem;

    private AudioPlayer _audioPlayer;
    private void Awake()
    {
        _audioPlayer = GetComponentInChildren<AudioPlayer>();
    }
    private void Start()
    {
        _closedPos = transform.localPosition;
        _openPos = new Vector3(_closedPos.x, _closedPos.y + _unitsToMove, _closedPos.z);
        //Debug.Log("Closed: " + _closedPos + " Open: " + _openPos);
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
        if (collision.gameObject.CompareTag("Trigger"))
        {
            HeldItem item = collision.gameObject.GetComponent<HeldItem>();
            if (item == null || !item.IsFrozen())
                return;

            _heldItem = item;
            _heldItem.OnUnfreeze += ResumeOperation;
            _stuck = true;
            return;
        }

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

    private void ResumeOperation()
    {
        if (_stuck)
            _stuck = false;
        _heldItem.OnUnfreeze -= ResumeOperation;
    }

    private void Open(int id)
    {
        if (id != _ID)
            return;
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(OpenClose(_openPos));
        _audioPlayer.Play("Open");
        _audioPlayer.FadeOut("Close", 1f);
    }

    private void Close(int id)
    {
        if (id != _ID)
            return;
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(OpenClose(_closedPos));
        _audioPlayer.Play("Close");
        _audioPlayer.FadeOut("Open", 1f);
    }

    private IEnumerator OpenClose(Vector3 targetPos)
    {
        Vector3 currentPos = transform.localPosition;
        float duration = Mathf.Abs(_unitsToMove) / _speed;
        float elapsedTime = 0f;
        WaitWhile ifStuck = new WaitWhile(() => _stuck);
        while (elapsedTime < duration)
        {
            yield return ifStuck;
            elapsedTime += Time.deltaTime;
            Vector3 pos = Vector3.Lerp(currentPos, targetPos, elapsedTime / duration);
            transform.localPosition = pos;
        }

        transform.localPosition = targetPos;
        if (targetPos.Equals(_openPos))
            _audioPlayer.Stop("Open");
        else
            _audioPlayer.Stop("Close");
    }
}
