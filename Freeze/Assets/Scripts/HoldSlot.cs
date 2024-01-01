using System.Collections.Generic;
using UnityEngine;

public class HoldSlot : MonoBehaviour
{
    private List<HeldItem> _items = new List<HeldItem>();
    [SerializeField] private Transform _objectGrabPoint;
    public void Hold(HeldItem target)
    {
        if (IsHolding() || target.IsHeld() || _objectGrabPoint == null || !target.IsInteractable())
            return;

        if (target.IsFrozen())
            target.Unfreeze();

        //Debug.Log("Status of " + target.transform + " is " + target.IsInteractable());
        _items.Add(target);
        target.OnHold();
        target.transform.SetParent(_objectGrabPoint, false);
        target.transform.position = transform.position;
    }

    public void Drop()
    {
        if (!IsHolding())
            return;

        foreach (var item in _items)
            item.OnDrop();
        _objectGrabPoint.DetachChildren();
        _items.Clear();
    }

    public void Freeze()
    {
        if (!IsHolding())
            return;
        foreach (var item in _items)
            item.OnFreeze();
        Drop();
    }

    public bool IsHolding()
    {
        return _items.Count > 0;
    }
}
