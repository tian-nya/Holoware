namespace Micro.Defeat
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using UnityEngine.Events;
    using Lean.Localization;

    public class BossDefeatFlare : BossDefeatUnit
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
            actions[1].onUse.AddListener(delegate { StartCoroutine(Fireball()); });
            actions[2].onUse.AddListener(delegate { StartCoroutine(OmegaFlare()); });
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
            microgame.boss.Damage(baseAtk + atkMod, 1f);
            Instantiate(actions[0].effects[0], (Vector2)microgame.boss.transform.position + microgame.boss.effectOffset, Quaternion.identity, microgame.boss.transform);
            yield return new WaitForSeconds(1f);
            microgame.actionInProgress = false;
        }

        IEnumerator Fireball()
        {
            Instantiate(actions[1].effects[0], (Vector2)microgame.boss.transform.position + microgame.boss.effectOffset, Quaternion.identity, microgame.boss.transform);
            yield return new WaitForSeconds(0.5f);
            microgame.boss.Damage(baseAtk + atkMod, 2.5f);
            yield return new WaitForSeconds(1f);
            microgame.actionInProgress = false;
        }

        IEnumerator OmegaFlare()
        {
            Instantiate(actions[2].effects[0], (Vector2)microgame.boss.transform.position + microgame.boss.effectOffset, Quaternion.identity, microgame.boss.transform);
            yield return new WaitForSeconds(1.5f);
            microgame.boss.Damage(baseAtk + atkMod, 4.5f);
            yield return new WaitForSeconds(1.5f);
            if (microgame.boss.hp > 0)
            {
                microgame.boss.ModifyDef(-2);
                microgame.sfx.PlaySFX(5);
            }
            yield return new WaitForSeconds(0.5f);
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