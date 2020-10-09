namespace Micro.Defeat
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;
    using TMPro;

    public class BossDefeatMenu : MonoBehaviour
    {
        public BossDefeat microgame;
        public bool setAsCurrentOnEnable = true, exitOnSelection = true, disableDuringAction = true; // if setAsCurrentOnEnable = false, set as current menu through event
        public Color enabledColor = Color.white, disabledColor = Color.gray;
        public MenuItem[] menuItems;
        [HideInInspector] public int index;
        bool canSelect;

        void OnEnable()
        {
            if (setAsCurrentOnEnable)
            {
                microgame.currentMenu = this;
            }
            StartCoroutine(MenuDelay());
        }

        // Update is called once per frame
        void Update()
        {
            if (microgame.currentMenu == this)
            {
                if (Input.GetButtonDown("Down"))
                {
                    index++;
                    if (index == menuItems.Length)
                    {
                        index = 0;
                    }
                    microgame.sfx.PlaySFX(2);
                    menuItems[index].onHighlight.Invoke();
                }
                if (Input.GetButtonDown("Up"))
                {
                    index--;
                    if (index == -1)
                    {
                        index = menuItems.Length - 1;
                    }
                    microgame.sfx.PlaySFX(2);
                    menuItems[index].onHighlight.Invoke();
                }
                if (microgame.canAct && Input.GetButtonDown("Space") && canSelect)
                {
                    if (disableDuringAction)
                    {
                        if (!microgame.actionInProgress)
                        {
                            if (!menuItems[index].disabled)
                            {
                                if (exitOnSelection)
                                {
                                    microgame.currentMenu = microgame.mainMenu;
                                }
                                menuItems[index].onSelect.Invoke();
                                microgame.sfx.PlaySFX(3);
                                if (exitOnSelection)
                                {
                                    if (microgame.currentMenu == microgame.mainMenu)
                                    {
                                        microgame.currentMenu.StartCoroutine(microgame.currentMenu.MenuDelay());
                                    }
                                    gameObject.SetActive(false);
                                }
                            }
                        }
                    } else
                    {
                        if (!menuItems[index].disabled)
                        {
                            if (exitOnSelection)
                            {
                                microgame.currentMenu = microgame.mainMenu;
                            }
                            menuItems[index].onSelect.Invoke();
                            microgame.sfx.PlaySFX(3);
                            if (exitOnSelection)
                            {
                                if (microgame.currentMenu == microgame.mainMenu)
                                {
                                    microgame.currentMenu.StartCoroutine(microgame.currentMenu.MenuDelay());
                                }
                                gameObject.SetActive(false);
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < menuItems.Length; i++)
            {
                if (!menuItems[i].disabled)
                {
                    menuItems[i].text.color = enabledColor;
                } else
                {
                    menuItems[i].text.color = disabledColor;
                }
                if (i == index)
                {
                    if (microgame.currentMenu != this)
                    {
                        menuItems[i].selectImg.color = disabledColor;
                    }
                    else if (disableDuringAction && microgame.actionInProgress)
                    {
                        menuItems[i].selectImg.color = disabledColor;
                    }
                    else if (microgame.canAct)
                    {
                        menuItems[i].selectImg.color = enabledColor;
                    }
                    else
                    {
                        menuItems[i].selectImg.color = disabledColor;
                    }
                } else
                {
                    menuItems[i].selectImg.color = Color.clear;
                }
            }
        }

        [System.Serializable]
        public class MenuItem
        {
            public TextMeshProUGUI text;
            public Image selectImg;
            public UnityEvent onHighlight, onSelect;
            [HideInInspector] public bool disabled;
        }

        public IEnumerator MenuDelay()
        {
            canSelect = false;
            yield return null;
            canSelect = true;
            menuItems[index].onHighlight.Invoke();
        }
    }
}