namespace Micro.Defeat
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using Lean.Localization;
    using UnityEngine.Events;

    public class BossDefeat : Microgame
    {
        public SFXManager sfx;
        public TextMeshProUGUI descriptionDisp, actionDisp;
        public GameObject actionDispFrame;
        public BossDefeatMenu mainMenu;
        public float mpRegenInterval = 0.5f;
        public GameObject damageFX, healFX, mpHealFX, buffFX, debuffFX;
        public BossDefeatUnit[] units;
        public BossDefeatFubuking boss;
        public UnityEvent onBattleStart;
        [HideInInspector] public BossDefeatMenu currentMenu;
        public BossDefeatMenu CurrentMenu { set { currentMenu = value; } }
        [HideInInspector] public bool canAct, allUnitsDown, actionInProgress;

        new void Awake()
        {
            base.Awake();
            for (int i = 0; i < units.Length; i++)
            {
                units[i].unitIndex = i;
            }
        }

        public void Start()
        {
            boss.gameObject.SetActive(false);
            onStart.AddListener(Game);
        }

        public void Game()
        {
            if (GameSettings.hardMode == 1)
            {
                AddAvatar(0);
                AddAvatar(1);
                AddAvatar(2);
                AddAvatar(3);
                AddAvatar(4);
            } else
            {
                AddAvatar(0);
                AddAvatar(2);
                AddAvatar(4);
            }
            
            bgm.PlayBGM(0);
            StartCoroutine(GameCoroutine());
        }

        IEnumerator GameCoroutine()
        {
            currentMenu = mainMenu;
            yield return new WaitForSeconds(2f);
            boss.gameObject.SetActive(true);
            yield return new WaitForSeconds(2f);
            onBattleStart.Invoke();
            canAct = true;
            StartCoroutine(MPRegen());
            boss.SetNextAction();

            while (true)
            {
                if (!actionInProgress)
                {
                    if (actionDispFrame.activeInHierarchy) actionDispFrame.SetActive(false);
                    if (boss.hp < 1)
                    {
                        cleared = true;
                        break;
                    }
                    allUnitsDown = CheckForAllUnitsDown();
                    if (allUnitsDown)
                    {
                        break;
                    }

                    for (int i = 0; i < units.Length; i++)
                    {
                        if (units[i].hp > 0 && units[i].timer > 0)
                        {
                            units[i].timer -= Time.deltaTime;
                        }
                    }
                    if (boss.timer > 0)
                    {
                        boss.timer -= Time.deltaTime;
                    }
                    if (boss.timer <= 0)
                    {
                        boss.UseAction(boss.nextActionIndex);
                    }
                }
                yield return null;
            }

            actionDispFrame.SetActive(false);
            canAct = false;
            currentMenu = null;
            yield return new WaitForSeconds(1f);
            if (cleared)
            {
                foreach (BossDefeatUnit i in units)
                {
                    i.animator.SetTrigger("win");
                }
                SetDescriptionLocalized("Defeat/Win");
                sfx.PlaySFX(0);
            } else
            {
                SetDescriptionLocalized("Defeat/Lose");
            }
            yield return new WaitForSeconds(3f);
            timeOver = true;
        }

        IEnumerator MPRegen()
        {
            float mpTimer = mpRegenInterval;
            while (!cleared && !allUnitsDown)
            {
                if (actionInProgress)
                {
                    yield return new WaitUntil(() => actionInProgress == false);
                }
                mpTimer -= Time.deltaTime;
                if (mpTimer <= 0)
                {
                    mpTimer = mpRegenInterval;
                    for (int i = 0; i < units.Length; i++)
                    {
                        if (units[i].hp > 0 && units[i].mp < 100)
                        {
                            units[i].mp += 1;
                        }
                    }
                    currentMenu.menuItems[currentMenu.index].onHighlight.Invoke();
                }
                yield return null;
            }
        }

        public void SetAction(string text)
        {
            actionDisp.text = LeanLocalization.GetTranslationText(text);
            actionDispFrame.SetActive(true);
        }

        public void SetDescription(string text)
        {
            descriptionDisp.text = text;
        }

        public void SetDescriptionLocalized(string text)
        {
            descriptionDisp.text = LeanLocalization.GetTranslationText(text);
        }

        public bool CheckForAllUnitsDown()
        {
            for (int i = 0; i < units.Length; i++)
            {
                if (units[i].hp > 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool CheckForAnyUnitDown()
        {
            for (int i = 0; i < units.Length; i++)
            {
                if (units[i].hp < 1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}