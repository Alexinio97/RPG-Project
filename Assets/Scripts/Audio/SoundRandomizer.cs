using UnityEngine;

namespace RPG.Audio
{
    public class SoundRandomizer : MonoBehaviour
    {
        [SerializeField] AudioClip[] audioClips;

        public void PlaySound()
        {
            var audioSource = GetComponent<AudioSource>();
            if (audioClips.Length > 0)
            {
                var randomIndex = Random.Range(0, audioClips.Length);
                audioSource.clip = audioClips[randomIndex];
                audioSource.Play();
            }
        }
    }
}
