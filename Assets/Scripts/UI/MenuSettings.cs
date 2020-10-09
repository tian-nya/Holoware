using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Localization;

public class MenuSettings : Menu
{
    public Toggle fullScreenUI, hardModeUI;
    public Slider sfxVolumeUI, bgmVolumeUI;
    public Dropdown langUI;
    public LeanLocalization localization;

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateSettingsUI();
    }

    protected override void Update()
    {
        base.Update();
        GameSettings.fullScreen = fullScreenUI.isOn ? 1 : 0;
        GameSettings.hardMode = hardModeUI.isOn ? 1 : 0;
        GameSettings.sfxVolume = sfxVolumeUI.value;
        GameSettings.bgmVolume = bgmVolumeUI.value;
        GameSettings.lang = langUI.value;
    }

    public void ResetToDefaults()
    {
        GameSettings.ResetToDefaults();
        UpdateSettingsUI();
    }

    public void SaveSettings()
    {
        GameSettings.SaveSettings();
    }

    public void LoadSettings()
    {
        GameSettings.LoadSettings();
    }

    void UpdateSettingsUI()
    {
        fullScreenUI.isOn = GameSettings.fullScreen == 1 ? true : false;
        hardModeUI.isOn = GameSettings.hardMode == 1 ? true : false;
        sfxVolumeUI.value = GameSettings.sfxVolume;
        bgmVolumeUI.value = GameSettings.bgmVolume;
        langUI.value = GameSettings.lang;
        localization.SetCurrentLanguage(GameSettings.lang);
    }

    public void UpdateFullscreen()
    {
        Screen.SetResolution(1280, 720, GameSettings.fullScreen == 1 ? true : false);
    }
}