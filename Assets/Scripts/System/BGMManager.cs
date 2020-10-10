using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] bgmList;
    [Header("Play on Awake Options")]
    public bool playOnAwake;
    public bool playRandom;
    public int index; // if playRandom is false, index points to BGM to play

    void Awake()
    {
        if (!audioSource) audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (playOnAwake && bgmList.Length > 0)
        {
            if (playRandom)
            {
                PlayBGM(Random.Range(0, bgmList.Length));
            } else
            {
                PlayBGM(Mathf.Clamp(index, 0, bgmList.Length - 1));
            }
        }
    }

    void Update()
    {
        audioSource.volume = GameSettings.bgmVolume;
    }

    // play BGM at specified index
    public void PlayBGM(int index)
    {
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (index < 0 || index >= bgmList.Length) return;
        audioSource.Stop();
        audioSource.clip = bgmList[index];
        audioSource.Play();
    }

    // directly provide audio clip to play
    public void PlayBGM(AudioClip clip)
    {
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}