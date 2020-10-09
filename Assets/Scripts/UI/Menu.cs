using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public UnityEvent onOpen, onReturn;
    public GameObject defaultSelection;
    public Menu parentMenu;
    StandaloneInputModule inputModule;
    GameObject lastSelected;
    bool submitReleased, cancelReleased;

    void Awake()
    {
        inputModule = EventSystem.current.GetComponent<StandaloneInputModule>();
    }

    protected virtual void Update()
    {
        if (!EventSystem.current.sendNavigationEvents) return;

        if ((Input.GetAxisRaw(inputModule.horizontalAxis) != 0 || Input.GetAxisRaw(inputModule.verticalAxis) != 0) && !EventSystem.current.currentSelectedGameObject)
        {
            EventSystem.current.SetSelectedGameObject(defaultSelection);
        }
        lastSelected = EventSystem.current.currentSelectedGameObject ? EventSystem.current.currentSelectedGameObject : null;
        if (Input.GetButtonDown(inputModule.cancelButton))
        {
            GoToParentMenu();
        }
    }

    protected virtual void OnEnable()
    {
        if (parentMenu) parentMenu.gameObject.SetActive(false);
        StartCoroutine(InputDelay());

        if (lastSelected)
        {
            EventSystem.current.SetSelectedGameObject(lastSelected);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(defaultSelection);
        }
        EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().OnSelect(new BaseEventData(EventSystem.current));

        onOpen.Invoke();
    }

    public void GoToParentMenu()
    {
        onReturn.Invoke();

        if (!parentMenu) return;
        parentMenu.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void EnableCursor(bool value)
    {
        if (value)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ClearLastSelected()
    {
        lastSelected = null;
    }

    public void LoadSceneSync(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator InputDelay()
    {
        EventSystem.current.sendNavigationEvents = false;
        yield return null;
        EventSystem.current.sendNavigationEvents = true;
    }
}