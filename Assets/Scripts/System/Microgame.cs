using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BGMManager))]
public abstract class Microgame : MonoBehaviour
{
    public RectTransform avatarArea;
    public List<GameObject> avatarSelection; // avatar prefabs can be found in Prefabs/Avatars
    public bool isBoss = false; // if true, clearing the microgame restores 1 life
    public int baseBeats = 8; // # of beats when using standard timer; use multiples of 4 (keep in mind most microgame BGM maxes out at 16 beats)
    [HideInInspector] public float timer;
    public string actionVerb; // used to point to the microgame name in localization files (Localization/Game_EN & Localization/Game_JP)
    public List<GameObject> objectsToEnable; // any game objects that must be enabled once the microgame starts (NOT when instantiated) can be added in here
    public bool useStandardTimer = true; // when true, the timer UI element will show and the microgame will exit once timer hits 0
    public UnityEvent onStart; // fires when microgame starts (NOT when instantiated)
    [HideInInspector] public BGMManager bgm;
    [HideInInspector] public bool cleared; // a microgame is considered cleared if cleared = true
    [HideInInspector] public bool timeOver; // once set to true, the microgame will exit; must be set manually if useStandardTimer = false
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

    // add avatar from avatarSelection at specified index to avatars list
    public void AddAvatar(int index)
    {
        avatars.Add(Instantiate(avatarSelection[index], avatarArea).GetComponent<CharacterAvatar>());
        avatars[avatars.Count - 1].transform.localScale = Vector3.one;
        avatars[avatars.Count - 1].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (avatars.Count - 1) * -160f);
    }
}
