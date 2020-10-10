using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    public SFXData[] sfxData;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        for (int i = 0; i < sfxData.Length; i++)
        {
            if (sfxData[i].playOnEnable) PlaySFX(i);
        }
    }

    // play SFX at specified index
    public void PlaySFX(int index)
    {
        if (index < 0 || index >= sfxData.Length) return;
        if (!audioSource) return;
        audioSource.PlayOneShot(sfxData[index].audioClip[Random.Range(0, sfxData[index].audioClip.Length)], GameSettings.sfxVolume * sfxData[index].volume);
    }

    // same as above but override volume
    public void PlaySFX(int index, float volume)
    {
        if (index < 0 || index >= sfxData.Length) return;
        if (!audioSource) return;
        audioSource.PlayOneShot(sfxData[index].audioClip[Random.Range(0, sfxData[index].audioClip.Length)], GameSettings.sfxVolume * sfxData[index].volume * volume);
    }

    [System.Serializable]
    public struct SFXData
    {
        public AudioClip[] audioClip; // a random audio clip is selected when SFX is played
        [Range(0f, 1f)]
        public float volume;
        public bool playOnEnable;
    }
}