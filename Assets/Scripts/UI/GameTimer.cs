using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(SFXManager))]
public class GameTimer : MonoBehaviour
{
    public Image bar, icon, marker;
    public Vector2 markerStart, markerEnd;
    public TextMeshProUGUI timeLeft;
    public Sprite icon1, icon2;
    SFXManager sfx;

    void Awake()
    {
        sfx = GetComponent<SFXManager>();
    }

    void OnEnable()
    {
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        int ticks = 3;
        icon.sprite = icon1;
        timeLeft.text = "";
        marker.color = Color.white;
        while (GameManager.gm.currentGame.timer > 0)
        {
            bar.fillAmount = GameManager.gm.currentGame.timer / (GameManager.gm.currentGame.baseBeats * GameManager.gm.beatLength);
            marker.rectTransform.localPosition = Vector2.Lerp(markerEnd, markerStart, bar.fillAmount);
            if (GameManager.gm.currentGame.timer < ticks * GameManager.gm.beatLength)
            {
                sfx.PlaySFX(0);
                timeLeft.text = ticks.ToString();
                ticks--;
                iTween.ScaleFrom(timeLeft.gameObject, iTween.Hash("scale", new Vector3(0f, 0f, 1f), "time", GameManager.gm.beatLength / 2f, "easetype", iTween.EaseType.easeOutBack));
            }
            yield return null;
        }
        timeLeft.text = "";
        icon.sprite = icon2;
        marker.color = Color.clear;
        yield return new WaitForSeconds(GameManager.gm.beatLength);
        gameObject.SetActive(false);
    }
}
