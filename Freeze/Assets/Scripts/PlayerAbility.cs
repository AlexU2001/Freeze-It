using UnityEngine;
using UnityEngine.UI;

public class PlayerAbility : MonoBehaviour
{
    [Tooltip("How far to shoot out a raycast for holding an object")]
    [SerializeField] private float _rayDistance = 5f;
    [SerializeField] private HoldSlot _slot;

    RaycastHit _hit;
    [SerializeField] private Transform _cameraPoint;
    [SerializeField] private Image _crosshair;
    [SerializeField] private Color _interactColor;
    [SerializeField] private Color _uninteractableColor;

    private int _mask;

    private void Start()
    {
        _mask = 1 << 6;
        _mask = ~(_mask);
        if (_cameraPoint == null)
            _cameraPoint = Camera.main.transform;
    }
    private void OnEnable()
    {
        InputManager.GrabDrop += ShootRay;
        InputManager.FreezeUnfreeze += FreezeHeld;
    }
    private void OnDisable()
    {
        InputManager.GrabDrop -= ShootRay;
        InputManager.FreezeUnfreeze += FreezeHeld;
    }
    private void FixedUpdate()
    {
        bool result = Physics.Raycast(_cameraPoint.position, _cameraPoint.forward, out _hit, _rayDistance, _mask);
        Debug.DrawLine(_cameraPoint.position, _cameraPoint.forward * _rayDistance);

        if (_crosshair == null)
            return;
        _crosshair.color = _uninteractableColor;
        if (!result)
            return;

        if (_hit.transform.TryGetComponent(out MonoBehaviour script))
        {
            if (script is IInteractable)
            {
                IInteractable interactable = (IInteractable)script;
                if (interactable.IsInteractable())
                    _crosshair.color = _interactColor;
            }
        }
    }

    private void ShootRay()
    {
        if (_slot.IsHolding())
        {
            _slot.Drop();
            return;
        }

        if (_hit.transform == null)
            return;

        Debug.Log("Hit " + _hit.transform.name);
        HeldItem item = _hit.transform.GetComponentInChildren<HeldItem>();
        if (item != null)
            _slot.Hold(item);
    }

    private void FreezeHeld()
    {
        _slot.Freeze();
    }
}
