using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Lean.Localization;

public class StatsDisplay : MonoBehaviour
{
    public SaveData save;
    public LeanLocalizedTextMeshProUGUI mode;
    public TextMeshProUGUI highScore, coins;

    public void OnEnable()
    {
        StartCoroutine(UpdateDisplayCoroutine());
    }

    public void UpdateDisplay()
    {
        if (PlayerPrefs.GetInt("hardMode") == 1)
        {
            mode.TranslationName = "HardMode";
            highScore.text = save.loadedSave.highScoreHard.ToString();
        } else
        {
            mode.TranslationName = "NormalMode";
            highScore.text = save.loadedSave.highScore.ToString();
        }
        coins.text = save.loadedSave.coins.ToString();
    }

    IEnumerator UpdateDisplayCoroutine()
    {
        while (!save.loaded)
        {
            yield return null;
        }
        UpdateDisplay();
    }
}
