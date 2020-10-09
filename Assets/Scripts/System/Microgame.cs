using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BGMManager))]
public abstract class Microgame : MonoBehaviour
{
    public RectTransform avatarArea;
    public List<GameObject> avatarSelection;
    public bool isBoss = false;
    public int baseBeats = 8;
    [HideInInspector] public float timer;
    public string actionVerb;
    public List<GameObject> objectsToEnable;
    public bool useStandardTimer = true;
    public UnityEvent onStart;
    [HideInInspector] public BGMManager bgm;
    [HideInInspector] public bool cleared, timeOver;
    [HideInInspector] public List<CharacterAvatar> avatars;
    public bool Cleared
    {
        get { return cleared; }
        set { cleared = value; }
    }

    protected void Awake()
    {
        bgm = GetComponent<BGMManager>();
    }

    public void OnStart()
    {
        cleared = false;
        timeOver = false;
        foreach (GameObject i in objectsToEnable)
        {
            i.SetActive(true);
        }
        if (useStandardTimer)
        {
            timer = baseBeats * GameManager.gm.beatLength;
            GameManager.gm.skin.gameTimer.SetActive(true);
            StartCoroutine(Countdown());
        }
        onStart.Invoke();
    }

    protected virtual IEnumerator Countdown()
    {
        do
        {
            timer -= Time.deltaTime;
            yield return null;
        } while (timer > 0);
        timeOver = true;
    }

    public virtual void CheckWinCondition()
    {
        GameManager.gm.microgameCleared = cleared;
        if (isBoss)
        {
            if (cleared && GameManager.gm.lives < 4)
            {
                GameManager.gm.lives++;
                GameManager.gm.skin.lifeIcons[GameManager.gm.lives - 1].RestoreLife();
            }
            GameManager.gm.bossesCleared++;
        }
    }

    public void AddAvatar(int index)
    {
        avatars.Add(Instantiate(avatarSelection[index], avatarArea).GetComponent<CharacterAvatar>());
        avatars[avatars.Count - 1].transform.localScale = Vector3.one;
        avatars[avatars.Count - 1].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (avatars.Count - 1) * -160f);
    }
}
