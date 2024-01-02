using UnityEngine;

public class HoldSlot : MonoBehaviour
{
    private HeldItem item;
    [SerializeField] private Transform _objectGrabPoint;
    public void Hold(HeldItem target)
    {
        if (IsHolding() || target.IsHeld() || _objectGrabPoint == null || !target.IsInteractable())
            return;

        if (target.IsFrozen())
            target.Unfreeze();

        //Debug.Log("Status of " + target.transform + " is " + target.IsInteractable());
        item = target;
        target.OnHold();
        target.transform.SetParent(_objectGrabPoint, false);
        target.transform.position = transform.position;
    }

    public void Drop()
    {
        if (!IsHolding())
            return;

        item.OnDrop();
        _objectGrabPoint.DetachChildren();
        item = null;
    }

    public void Freeze()
    {
        if (!IsHolding())
            return;

        item.OnFreeze();
        Drop();
    }

    public bool IsHolding()
    {
        return item != null;
    }
}
