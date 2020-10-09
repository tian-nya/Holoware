using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class LoadingScreen : MonoBehaviour
{
    public Image loadingBar;
    public TextMeshProUGUI loadingProgress;
    public List<GameObject> loadingScreenObjects;
    public bool canLoad = true, loadOnAwake = false;
    public float transitionTime = 0.5f;
    public string loadOnAwakeSceneName;
    Animator animator;
    bool isLoading;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.gm != null)
        {
            if (!GameManager.gm.loadingScreen)
            {
                GameManager.gm.loadingScreen = this;
            }
            else if (GameManager.gm.loadingScreen != this)
            {
                Debug.LogWarning("Unnecessary Loading Screen instance");
            }
        }

        animator = GetComponent<Animator>();
        if (loadingBar) loadingScreenObjects.Add(loadingBar.gameObject);
        if (loadingProgress) loadingScreenObjects.Add(loadingProgress.gameObject);
        if (!loadOnAwake)
        {
            StartCoroutine(EnterTransition());
        }
        else
        {
            StartCoroutine(LoadSceneAsyncByName(loadOnAwakeSceneName));
        }
    }

    public void LoadScene(int index)
    {
        if (isLoading) return;
        StartCoroutine(LoadSceneAsyncByIndex(index));
    }

    public void LoadScene(string name)
    {
        if (isLoading) return;
        StartCoroutine(LoadSceneAsyncByName(name));
    }

    public void EnableLoad()
    {
        canLoad = true;
    }

    public void DisableLoad()
    {
        canLoad = false;
    }

    IEnumerator EnterTransition()
    {
        foreach (GameObject i in loadingScreenObjects)
        {
            i.SetActive(false);
        }
        isLoading = true;
        yield return new WaitForSecondsRealtime(transitionTime);
        isLoading = false;
        if (EventSystem.current) EventSystem.current.sendNavigationEvents = true;
    }

    IEnumerator LoadSceneAsyncByIndex(int index)
    {
        isLoading = true;
        if (EventSystem.current) EventSystem.current.sendNavigationEvents = false;
        animator.SetTrigger("exit");
        yield return new WaitForSecondsRealtime(transitionTime);
        foreach (GameObject i in loadingScreenObjects)
        {
            i.SetActive(true);
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);
        while (!asyncLoad.isDone)
        {
            if (loadingProgress) loadingProgress.text = Mathf.Floor(asyncLoad.progress * 100f) + "%";
            if (loadingBar) loadingBar.fillAmount = asyncLoad.progress;
            yield return null;
        }
    }

    IEnumerator LoadSceneAsyncByName(string name)
    {
        isLoading = true;
        if (EventSystem.current) EventSystem.current.sendNavigationEvents = false;
        animator.SetTrigger("exit");
        yield return new WaitForSecondsRealtime(transitionTime);
        foreach (GameObject i in loadingScreenObjects)
        {
            i.SetActive(true);
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
        while (!asyncLoad.isDone)
        {
            if (loadingProgress) loadingProgress.text = Mathf.Floor(asyncLoad.progress * 100f) + "%";
            if (loadingBar) loadingBar.fillAmount = asyncLoad.progress;
            yield return null;
        }
    }
}