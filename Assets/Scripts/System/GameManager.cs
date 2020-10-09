using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using Lean.Localization;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public static int practiceIndex;

    // public bool paused = false, canPause = true;
    public AudioMixer audioMixer;
    public AudioMixerSnapshot defaultMixerSnapshot, microMixerSnapshot;
    [HideInInspector] public LoadingScreen loadingScreen;
    public StandaloneInputModule inputModule;
    [HideInInspector] public SaveData save;
    public MenuGameOver gameOverMenu;
    public bool practiceMode = false;
    public float beatLength = 0.5f, hardModeSpeed = 1.3f, hardModeBossSpeed = 1.2f, speedUpAdd = 0.125f, speedUpAddHard = 0.15f, bossSpeedUpAdd = 0.125f, bossSpeedUpAddHard = 0.15f;
    public int speedUpInterval = 4, bossInterval = 12;
    public MicrogamePool microgames, bosses;
    public Skin skin;
    public GameObject[] skinChoices;
    [Header("Live Stats")]
    public int points = 0;
    public int lives = 4;
    public int bossesCleared = 0;
    public List<int> gameIndexPool, bossIndexPool;
    public Microgame currentGame;
    public float speedUpMult;
    public bool microgameCleared;
    int lastGameIndex, lastBossIndex;

    /*
    [Header("Events")]
    public UnityEvent onPause;
    float prevTimeScale = 1f;
    bool reloadQueued;
    */

    private void Awake()
    {
        if (gm != null)
        {
            Destroy(gameObject);
        }
        gm = this;

        GameSettings.LoadSettings();
        save = GetComponent<SaveData>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartGameCoroutine());
    }

    IEnumerator StartGameCoroutine()
    {
        AudioListener.pause = false;
        defaultMixerSnapshot.TransitionTo(0f);

        while (!save.loaded)
        {
            yield return null;
        }

        if (save.loadedSave.currentSkin < skinChoices.Length)
        {
            skin = Instantiate(skinChoices[save.loadedSave.currentSkin].gameObject, Vector2.zero, Quaternion.identity).GetComponent<Skin>();
        } else
        {
            skin = Instantiate(skinChoices[0].gameObject, Vector2.zero, Quaternion.identity).GetComponent<Skin>();
        }
        skin.microPrep.AddListener(delegate { skin.bgm.PlayBGM(0); });
        skin.microStart.AddListener(skin.bgm.audioSource.Stop);
        skin.microWin.AddListener(delegate { skin.bgm.PlayBGM(1); });
        skin.microFail.AddListener(delegate { skin.bgm.PlayBGM(2); });
        skin.speedUp.AddListener(delegate { skin.bgm.PlayBGM(3); });
        skin.boss.AddListener(delegate { skin.bgm.PlayBGM(4); });
        skin.gameOver.AddListener(delegate { skin.bgm.PlayBGM(5); });

        StartCoroutine(GameFlow());
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && lives > 0)
        {
            StopAllCoroutines();
            loadingScreen.LoadScene("Title");
        }
    }

    /*
    public void Pause()
    {
        if (!canPause || reloadQueued) return;
        if (!paused)
        {
            paused = true;
            StartCoroutine(PauseCoroutine());
        }
    }

    public void PauseNoEvent()
    {
        if (!canPause || reloadQueued) return;
        if (!paused)
        {
            paused = true;
            StartCoroutine(PauseNoEventCoroutine());
        }
    }

    public void Unpause()
    {
        if (paused)
        {
            paused = false;
            StartCoroutine(UnpauseCoroutine());
        }
    }

    public void ReloadWithDelay(float delay)
    {
        if (reloadQueued) return;
        StartCoroutine(ReloadWithDelayCoroutine(delay));
    }

    IEnumerator ReloadWithDelayCoroutine(float delay)
    {
        canPause = false;
        reloadQueued = true;
        yield return new WaitForSecondsRealtime(delay);
        if (loadingScreen)
        {
            loadingScreen.LoadScene(SceneManager.GetActiveScene().name);
        } else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void EnablePause(bool value)
    {
        canPause = value;
    }

    IEnumerator PauseCoroutine()
    {
        yield return null;
        prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        onPause.Invoke();
    }

    IEnumerator PauseNoEventCoroutine()
    {
        yield return null;
        prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    IEnumerator UnpauseCoroutine()
    {
        yield return null;
        Time.timeScale = prevTimeScale;
        paused = false;
        AudioListener.pause = false;
    }
    */

    public void LoadMicrogame(GameObject game)
    {
        Camera.main.transform.position = new Vector3(0, 0, -10f);
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 5f;
        UnloadMicrogame();
        microgameCleared = false;
        currentGame = Instantiate(game.gameObject, Vector3.zero, Quaternion.identity).GetComponent<Microgame>();
    }

    public void UnloadMicrogame()
    {
        if (!currentGame) return;
        Camera.main.transform.position = new Vector3(0, 0, -10f);
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 5f;
        Destroy(currentGame.gameObject);
        currentGame = null;
    }

    public void SetGameSpeed(float mult)
    {
        Time.timeScale = mult;
        audioMixer.SetFloat("GamePitch", mult);
        audioMixer.SetFloat("GamePitchMicro", mult);
    }

    public void RefillPool(List<int> pool, int amount)
    {
        pool.Clear();
        List<int> source = new List<int>();
        int index;
        for (int i = 0; i < amount; i++)
        {
            source.Add(i);
        }
        for (int i = 0; i < amount; i++)
        {
            index = Random.Range(0, source.Count);
            pool.Add(source[index]);
            source.RemoveAt(index);
        }
    }

    public virtual IEnumerator GameFlow()
    {
        int index = 0;
        lastGameIndex = -1;
        lastBossIndex = -1;
        // Start game
        if (GameSettings.hardMode == 1) speedUpMult = hardModeSpeed;
        SetGameSpeed(speedUpMult);
        skin.gameStart.Invoke();
        yield return new WaitForSeconds(beatLength * 4);

        while (lives > 0)
        {
            // Handle speed up, microgame/boss loading
            if (points > 0 && points % bossInterval == 0)
            {
                yield return new WaitForSeconds(beatLength * 4);
                SetGameSpeed((GameSettings.hardMode == 1 ? hardModeBossSpeed : 1f) + ((GameSettings.hardMode == 1 ? bossSpeedUpAddHard : bossSpeedUpAdd) * bossesCleared));
                skin.boss.Invoke();
                skin.animator.SetTrigger("boss");
                yield return new WaitForSeconds(beatLength * 4);
                if (bossIndexPool.Count == 0)
                {
                    RefillPool(bossIndexPool, bosses.pool.Length);
                }
                do
                {
                    index = Random.Range(0, bossIndexPool.Count);
                } while (bossIndexPool[index] == lastBossIndex && bosses.pool.Length > 1);
                LoadMicrogame(GameSettings.hardMode == 1 ? bosses.pool[bossIndexPool[index]].hardMicrogame : bosses.pool[bossIndexPool[index]].microgame);
                skin.controlType.SetIcons(bosses.pool[bossIndexPool[index]].type);
                if (bosses.pool.Length > 1) lastBossIndex = bossIndexPool[index];
                bossIndexPool.RemoveAt(index);
            }
            else
            {
                if (points > 0 && points % speedUpInterval == 0)
                {
                    yield return new WaitForSeconds(beatLength * 4);
                    speedUpMult += GameSettings.hardMode == 1 ? speedUpAddHard : speedUpAdd;
                    skin.speedUp.Invoke();
                    SetGameSpeed(speedUpMult);
                    skin.animator.SetTrigger("speedUp");
                    yield return new WaitForSeconds(beatLength * 8);
                    skin.microPrep.Invoke();
                    yield return new WaitForSeconds(beatLength * 4);
                }
                if (gameIndexPool.Count == 0)
                {
                    RefillPool(gameIndexPool, microgames.pool.Length);
                }
                do
                {
                    index = Random.Range(0, gameIndexPool.Count);
                } while (gameIndexPool[index] == lastGameIndex && microgames.pool.Length > 1);
                LoadMicrogame(GameSettings.hardMode == 1 ? microgames.pool[gameIndexPool[index]].hardMicrogame : microgames.pool[gameIndexPool[index]].microgame);
                skin.controlType.SetIcons(microgames.pool[gameIndexPool[index]].type);
                if (microgames.pool.Length > 1) lastGameIndex = gameIndexPool[index];
                gameIndexPool.RemoveAt(index);
            }
            points++;
            skin.points.text = points.ToString();
            skin.animator.SetTrigger("microPrep");
            yield return new WaitForSeconds(beatLength * 4);

            // Transition to microgame
            skin.actionVerb.text = LeanLocalization.GetTranslationText("Game/" + currentGame.actionVerb);
            microMixerSnapshot.TransitionTo(0f);
            skin.microStart.Invoke();
            currentGame.OnStart();
            skin.animator.SetTrigger("microStart");
            while (!currentGame.timeOver)
            {
                yield return null;
            }
            
            // Transition to home
            SetGameSpeed(speedUpMult);
            defaultMixerSnapshot.TransitionTo(0f);
            currentGame.CheckWinCondition();
            if (microgameCleared)
            {
                skin.microWin.Invoke();
                skin.animator.SetTrigger("microWin");
                yield return new WaitForSeconds(GameManager.gm.beatLength);
                skin.sfx.PlaySFX(0);
            }
            else
            {
                lives--;
                skin.lifeIcons[lives].RemoveLife();
                if (lives > 0)
                {
                    skin.microFail.Invoke();
                    skin.animator.SetTrigger("microFail");
                    yield return new WaitForSeconds(GameManager.gm.beatLength);
                    skin.sfx.PlaySFX(1);
                }
                else
                {
                    SetGameSpeed(1f);
                    skin.gameOver.Invoke();
                    skin.animator.SetTrigger("gameOver");
                    yield return new WaitForSeconds(GameManager.gm.beatLength);
                }
            }
            UnloadMicrogame();
            yield return new WaitForSeconds(beatLength * 3);

            // Handle game over
            if (lives == 0)
            {
                gameOverMenu.gameObject.SetActive(true);
            }
        }
    }
}