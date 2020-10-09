using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Lean.Localization;

public class SettingsLoader : MonoBehaviour
{
    public AudioMixer audioMixer;
    public LeanLocalization localization;

    void Awake()
    {
        GameSettings.LoadSettings();

        Screen.SetResolution(1280, 720, GameSettings.fullScreen == 1 ? true : false);
    }

    void Start()
    {
        AudioListener.pause = false;
        Time.timeScale = 1f;
        audioMixer.SetFloat("GamePitch", 1);
        if (localization) localization.SetCurrentLanguage(GameSettings.lang);
    }
}