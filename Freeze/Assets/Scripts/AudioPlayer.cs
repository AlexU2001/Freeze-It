using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] private Sound[] sounds;
        [SerializeField] private float spatialBlend = 1;
        [SerializeField] private float minDistance = 1;
        [SerializeField] private float maxDistance = 3;

        private void Awake()
        {
            InitializeSounds();
        }

        private void InitializeSounds()
        {
            foreach (var sound in sounds)
            {
                if (sound.clip == null)
                    continue;

                sound.audioSource = gameObject.AddComponent<AudioSource>();
                sound.audioSource.clip = sound.clip;

                sound.audioSource.playOnAwake = false;
                sound.audioSource.loop = sound.loop;

                if (sound.volume <= 0)
                    sound.volume = 1;
                sound.audioSource.volume = sound.volume;
                sound.audioSource.spatialBlend = spatialBlend;
                sound.audioSource.minDistance = minDistance;
                sound.audioSource.maxDistance = maxDistance;

                sound.audioSource.rolloffMode = AudioRolloffMode.Linear;
            }
        }

        public void Play(string soundName)
        {
            if (GetSound(soundName) == null)
                return;

            Sound sound = GetSound(soundName);
            AudioSource audioSource = sound.audioSource;
            audioSource.pitch = Random.Range(sound.pitchMin, sound.pitchMax);
            audioSource.Play();
        }
        public void Play(string soundName, float percent, bool randomPoint)
        {
            Sound sound = GetSound(soundName);
            if (sound == null || sound.audioSource == null)
                return;

            sound.audioSource.pitch = Random.Range(sound.pitchMin, sound.pitchMax);
            if (sound.mutation != null)
                StopCoroutine(sound.mutation);
            sound.mutation = StartCoroutine(PlayForPeriod(sound, percent, randomPoint));
        }

        private IEnumerator PlayForPeriod(Sound sound, float percent, bool randomPoint)
        {
            if (sound == null || sound.audioSource == null)
                yield break;

            percent = Mathf.Clamp(percent, 0, 1);
            float length = sound.audioSource.clip.length;
            float duration = length * percent;
            if (randomPoint)
            {
                float point = Random.Range(0, length - duration);
                sound.audioSource.time = point;
                Debug.Log(string.Format("Percent: {0} Length: {1} Duration: {2} Point: {3}", percent, length, duration, point));
            }
            sound.audioSource.Play();

            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                /*Debug.Log("Sound Progress: " + sound.audioSource.time);*/
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            sound.audioSource.Stop();
        }
        public void Stop(string name)
        {
            Sound sound = GetSound(name);
            if (sound == null || sound.audioSource == null || !sound.audioSource.isPlaying)
                return;
            sound.audioSource.Stop();
        }

        public void FadeOut(string name, float duration)
        {
            Sound sound = GetSound(name);
            if (sound == null || sound.audioSource == null || !sound.audioSource.isPlaying)
                return;
            if (sound.mutation != null)
                StopCoroutine(sound.mutation);
            sound.mutation = StartCoroutine(FadeSound(sound, duration, false));
        }

        private IEnumerator FadeSound(Sound sound, float duration, bool fadeIn)
        {
            float start = fadeIn ? sound.volume : 0;
            float end = fadeIn ? 0 : sound.volume;
            float elapsedTime = 0;
            float factor = 0;
            float value = 0;
            while (elapsedTime < duration)
            {
                factor = elapsedTime / duration;
                value = Mathf.Lerp(start, end, factor);
                sound.audioSource.volume = value;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            sound.audioSource.Stop();
            sound.audioSource.volume = sound.volume;
        }

        // Getters
        public bool IsPlaying(string soundName)
        {
            Sound sound = GetSound(soundName);
            if (sound == null || sound.audioSource == null)
                return false;
            return GetSound(soundName).audioSource.isPlaying;
        }

        private Sound GetSound(string soundName)
        {
            foreach (var item in sounds)
            {
                if (item.name.Equals(soundName) && item.audioSource != null)
                    return item;
            }
            return null;
        }

        [System.Serializable]
        public class Sound
        {
            public string name;
            [HideInInspector] public AudioSource audioSource;
            public AudioClip clip;
            [Range(0.01f, 1)]
            public float volume = 1;
            public float pitchMin = 0f;
            public float pitchMax = 1f;
            public bool loop = false;
            public Coroutine mutation;
        }
    }
}