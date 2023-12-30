using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private void Update()
    {
        if (_target != null)
            transform.position = _target.position;
    }
}
