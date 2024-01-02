using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FreezeBlaster : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private List<FreezeProfile> profiles;

    private Coroutine _coroutine;

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

        if (item.IsFrozen())
        {
            item.AddFreezeTime(1f);
            return;
        }

        FreezeProfile profile = GetProfile(item);
        if (profile.ReadyToFreeze())
        {
            item.OnFreeze();
        }
        else if (_coroutine == null)
            _coroutine = StartCoroutine(UpdateProfile());
        profile.AddToFreezeAmount(1);
    }

    private IEnumerator UpdateProfile()
    {
        Queue<FreezeProfile> removeQueue = new Queue<FreezeProfile>();
        while (profiles.Count > 0)
        {
            for (int i = 0; i < profiles.Count; i++)
            {
                FreezeProfile profile = profiles[i];
                profile.AddToFreezeAmount(-1);
                if (profile.ReadyToRemove())
                    removeQueue.Enqueue(profile);
            }
            for (int i = 0; i < removeQueue.Count; i++)
            {
                FreezeProfile profile = removeQueue.Dequeue();
                profiles.Remove(profile);
            }
            yield return new WaitForSeconds(1);
        }
    }

    // Getters
    private FreezeProfile GetProfile(HeldItem item)
    {
        foreach (var profile in profiles)
            if (profile.item.Equals(item))
                return profile;
        FreezeProfile newProfile = new FreezeProfile(item);
        profiles.Add(newProfile);
        return newProfile;
    }

    [System.Serializable]
    private class FreezeProfile
    {
        public HeldItem item { get; private set; }
        private int _freezeAmount = 0;

        public FreezeProfile(HeldItem item)
        {
            this.item = item;
            _freezeAmount = 0;
        }

        public void AddToFreezeAmount(int amount)
        {
            if (item.IsFrozen())
                return;

            _freezeAmount += amount;
        }

        public bool ReadyToFreeze()
        {
            bool result = _freezeAmount > item.GetThreshold() && !item.IsFrozen();
            if (result)
                _freezeAmount = 0;
            return result;
        }

        public bool ReadyToRemove()
        {
            if (_freezeAmount <= 0)
                return true;
            return false;
        }
    }
}
