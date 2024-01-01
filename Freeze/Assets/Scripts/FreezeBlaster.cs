using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FreezeBlaster : MonoBehaviour
{
    public ParticleSystem particles;

    void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject other)
    {
        //Debug.Log("Collided");
        if (!other.CompareTag("Trigger"))
            return;

        HeldItem item = other.gameObject.GetComponent<HeldItem>();
        if (item == null)
            return;
        if (!item.IsFrozen())
            item.OnFreeze();
        item.AddFreezeTime(1f);

    }
}
