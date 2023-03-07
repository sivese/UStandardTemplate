using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Std.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class Sound : MonoBehaviour
    {
        protected AudioSource source;

        /// <summary>
        /// Play the specified clip.
        /// </summary>
        /// <param name="clip">Clip.</param>
        public void Play(AudioClip clip, float volume = 1f)
        {
            if (source == null)
            {
                source = gameObject.GetComponent<AudioSource>();
            }

            gameObject.name = clip.name;

            source.clip = clip;
            source.Play();
            source.volume = volume;

            var delay= new WaitForSeconds(source.clip.length + 0.2f);

            StartCoroutine(DelayDespawn(delay));
        }

        protected IEnumerator DelayDespawn(WaitForSeconds delay)
        {
            yield return delay;

            gameObject.SetActive(false);
        }
    }
}