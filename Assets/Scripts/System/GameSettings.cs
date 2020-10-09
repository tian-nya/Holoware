using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings
{
    public static int fullScreen, lang, hardMode;
    public static float sfxVolume, bgmVolume;

    public static void ResetToDefaults()
    {
        fullScreen = 0;
        sfxVolume = 1f;
        bgmVolume = 0.5f;
        hardMode = 0;
    }

    public static void SaveSettings()
    {
        PlayerPrefs.SetInt("fullScreen", fullScreen);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.SetFloat("bgmVolume", bgmVolume);
        PlayerPrefs.SetInt("lang", lang);
        PlayerPrefs.SetInt("hardMode", hardMode);
        PlayerPrefs.Save();
    }

    public static void LoadSettings()
    {
        fullScreen = PlayerPrefs.GetInt("fullScreen", 0);
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 1f);
        bgmVolume = PlayerPrefs.GetFloat("bgmVolume", 0.5f);
        lang = PlayerPrefs.GetInt("lang", 0);
        hardMode = PlayerPrefs.GetInt("hardMode", 0);
    }
}