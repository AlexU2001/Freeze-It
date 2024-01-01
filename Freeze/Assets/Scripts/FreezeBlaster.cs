using System.Collections.Generic;
using UnityEngine;

public class FreezeBlaster : MonoBehaviour
{

    public ParticleSystem particles;
    public List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        particles = GetComponentInChildren<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        Debug.Log("Collided");
        if (!other.CompareTag("Trigger"))
            return;

        HeldItem item = other.gameObject.GetComponent<HeldItem>();
        if (item == null || item.IsFrozen())
            return;

        item.OnFreeze();
    }
}
