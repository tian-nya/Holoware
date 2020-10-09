namespace Micro.Defeat
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using UnityEngine.Events;
    using Lean.Localization;

    public class BossDefeatRushia : BossDefeatUnit
    {
        public BossDefeatMenu skill2Menu;
        int allyIndex;

        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            onDeath.AddListener(delegate { microgame.avatars[unitIndex].SetExpression(2); });
            onDeath.AddListener(ForceExitMenus);
            onRevive.AddListener(delegate { microgame.avatars[unitIndex].SetExpression(0); });
            onWin.AddListener(delegate { microgame.avatars[unitIndex].SetExpression(1); });

            actions[0].onUse.AddListener(delegate { StartCoroutine(Attack()); });
            actions[1].onUse.AddListener(delegate { StartCoroutine(HealSpell()); });
            actions[2].onUse.AddListener(delegate { StartCoroutine(Resurrection()); });
        }

        // Update is called once per frame
        new void Update()
        {
            base.Update();
            if (hp < 1)
            {
                microgame.mainMenu.menuItems[unitIndex].disabled = true;
            } else
            {
                microgame.mainMenu.menuItems[unitIndex].disabled = false;
                if (timer > 0 || microgame.actionInProgress)
                {
                    unitMenu.menuItems[0].disabled = true;
                    unitMenu.menuItems[1].disabled = true;
                    unitMenu.menuItems[2].disabled = true;
                    for (int i = 0; i < skill2Menu.menuItems.Length; i++)
                    {
                        skill2Menu.menuItems[i].disabled = true;
                    }
                }
                else
                {
                    unitMenu.menuItems[0].disabled = false;
                    unitMenu.menuItems[1].disabled = mp >= actions[1].mpCost ? false : true;
                    unitMenu.menuItems[2].disabled = mp >= actions[2].mpCost && microgame.CheckForAnyUnitDown() ? false : true;
                    for (int i = 0; i < skill2Menu.menuItems.Length; i++)
                    {
                        skill2Menu.menuItems[i].disabled = mp >= actions[2].mpCost && microgame.units[i].hp < 1 ? false : true;
                    }
                }
            }
        }

        IEnumerator Attack()
        {
            Instantiate(actions[0].effects[0], (Vector2)microgame.boss.transform.position + microgame.boss.effectOffset, Quaternion.identity, microgame.boss.transform);
            microgame.boss.Damage(baseAtk + atkMod, 1f);
            yield return new WaitForSeconds(1f);
            microgame.actionInProgress = false;
        }

        IEnumerator HealSpell()
        {
            Instantiate(actions[1].effects[0], (Vector2)transform.position + effectOffset, Quaternion.identity, transform);
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < microgame.units.Length; i++)
            {
                if (microgame.units[i].hp > 0)
                {
                    microgame.units[i].Heal(25);
                }
            }
            yield return new WaitForSeconds(0.5f);
            microgame.actionInProgress = false;
        }

        IEnumerator Resurrection()
        {
            Instantiate(actions[2].effects[0], (Vector2)microgame.units[allyIndex].transform.position + microgame.units[allyIndex].effectOffset, Quaternion.identity, microgame.units[allyIndex].transform);
            yield return new WaitForSeconds(1.5f);
            microgame.units[allyIndex].Heal(microgame.units[allyIndex].maxHP);
            yield return new WaitForSeconds(0.5f);
            microgame.actionInProgress = false;
        }

        public void ForceExitMenus()
        {
            if (microgame.currentMenu == unitMenu || microgame.currentMenu == skill2Menu)
            {
                microgame.currentMenu = microgame.mainMenu;
                microgame.currentMenu.StartCoroutine(microgame.currentMenu.MenuDelay());
            }
            unitMenu.gameObject.SetActive(false);
            skill2Menu.gameObject.SetActive(false);
        }

        public void SetAllyTarget(int index)
        {
            allyIndex = index;
        }
    }
}