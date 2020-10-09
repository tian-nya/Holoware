namespace Micro.Defeat
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using UnityEngine.Events;
    using Lean.Localization;

    public class BossDefeatNoel : BossDefeatUnit
    {
        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            onDeath.AddListener(delegate { microgame.avatars[unitIndex].SetExpression(2); });
            onDeath.AddListener(ForceExitMenus);
            onRevive.AddListener(delegate { microgame.avatars[unitIndex].SetExpression(0); });
            onWin.AddListener(delegate { microgame.avatars[unitIndex].SetExpression(1); });

            actions[0].onUse.AddListener(delegate { StartCoroutine(Attack()); });
            actions[1].onUse.AddListener(delegate { StartCoroutine(Shield()); });
            actions[2].onUse.AddListener(delegate { StartCoroutine(Barrier()); });
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
                }
                else
                {
                    unitMenu.menuItems[0].disabled = false;
                    unitMenu.menuItems[1].disabled = mp >= actions[1].mpCost ? false : true;
                    unitMenu.menuItems[2].disabled = mp >= actions[2].mpCost ? false : true;
                }
            }
        }

        IEnumerator Attack()
        {
            Instantiate(actions[0].effects[0], (Vector2)transform.position + effectOffset, Quaternion.identity, transform);
            Instantiate(actions[0].effects[1], (Vector2)microgame.boss.transform.position + microgame.boss.effectOffset, Quaternion.identity, microgame.boss.transform);
            yield return new WaitForSeconds(0.1f);
            microgame.boss.Damage(baseAtk + atkMod, 1f);
            yield return new WaitForSeconds(0.9f);
            microgame.actionInProgress = false;
        }

        IEnumerator Shield()
        {
            Instantiate(actions[1].effects[0], (Vector2)transform.position + effectOffset, Quaternion.identity, transform);
            microgame.boss.Retarget(unitIndex);
            yield return new WaitForSeconds(1f);
            microgame.actionInProgress = false;
        }

        IEnumerator Barrier()
        {
            Instantiate(actions[2].effects[0], (Vector2)transform.position + effectOffset, Quaternion.identity, transform);
            yield return new WaitForSeconds(0.5f);
            foreach (BossDefeatUnit i in microgame.units)
            {
                i.barrier = true;
            }
            yield return new WaitForSeconds(0.75f);
            microgame.actionInProgress = false;
        }

        public void ForceExitMenus()
        {
            if (microgame.currentMenu == unitMenu)
            {
                microgame.currentMenu = microgame.mainMenu;
                microgame.currentMenu.StartCoroutine(microgame.currentMenu.MenuDelay());
            }
            unitMenu.gameObject.SetActive(false);
        }
    }
}