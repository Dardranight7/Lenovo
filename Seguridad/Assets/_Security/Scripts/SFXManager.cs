using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public AudioSource audioSource;
    public List<SFX> sfxList = new List<SFX>();

    public void PlaySFX(string key)
    {
        SFX sfx = sfxList.Find(s => s.key == key);
        if (sfx != null)
        {
            audioSource.pitch = sfx.pitch;
            audioSource.volume = sfx.volume;
            AudioClip selectedClip = sfx.clip[Random.Range(0, sfx.clip.Count)];
            audioSource.PlayOneShot(selectedClip);
        }
        else
        {
            Debug.LogWarning($"SFX with key '{key}' not found!");
        }
    }

    [System.Serializable]
    public class SFX
    {
        public string key;
        public List<AudioClip> clip = new List<AudioClip>();
        public float volume = 1.0f;
        public float pitch = 1.0f;
    }
}
