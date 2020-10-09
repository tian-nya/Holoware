using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuGameOver : Menu
{
    public TextMeshProUGUI score, highScore, coins, newRecord;
    int coinsEarned;

    protected override void OnEnable()
    {
        base.OnEnable();
        score.text = GameManager.gm.points.ToString();
        if (GameSettings.hardMode == 1)
        {
            if (GameManager.gm.points > GameManager.gm.save.loadedSave.highScoreHard)
            {
                GameManager.gm.save.loadedSave.highScoreHard = GameManager.gm.points;
                newRecord.gameObject.SetActive(true);
            }
            else
            {
                newRecord.gameObject.SetActive(false);
            }
            highScore.text = GameManager.gm.save.loadedSave.highScoreHard.ToString();
            coinsEarned = Mathf.FloorToInt((GameManager.gm.points - 4) * 1.5f);
        } else
        {
            if (GameManager.gm.points > GameManager.gm.save.loadedSave.highScore)
            {
                GameManager.gm.save.loadedSave.highScore = GameManager.gm.points;
                newRecord.gameObject.SetActive(true);
            }
            else
            {
                newRecord.gameObject.SetActive(false);
            }
            highScore.text = GameManager.gm.save.loadedSave.highScore.ToString();
            coinsEarned = GameManager.gm.points - 4;
        }
        if (coinsEarned < 0) coinsEarned = 0;
        coins.text = "+" + coinsEarned;
        GameManager.gm.save.loadedSave.coins += coinsEarned;
        GameManager.gm.save.SaveFile();
    }
}