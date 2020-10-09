namespace Micro.Defeat
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using UnityEngine.Events;
    using Lean.Localization;

    [RequireComponent(typeof(Animator))]
    public class BossDefeatUnit : MonoBehaviour
    {
        public BossDefeat microgame;
        public string unitName;
        public bool isPlayerUnit = true;
        public int maxHP = 100, baseAtk = 10, baseDef = 0;
        public float cooldown = 4f;
        public Vector2 effectOffset = Vector2.up * 1.2f;
        public GameObject barrierFX;
        public BossDefeatMenu unitMenu;
        public Image hpBar, mpBar, timerBar;
        public TextMeshProUGUI hpDisp, mpDisp, timerDisp;
        public UnitAction[] actions;
        [HideInInspector] public UnityEvent onHit, onDeath, onRevive, onWin;
        [HideInInspector] public Animator animator;
        [HideInInspector] public SFXManager sfx;
        [HideInInspector] public int unitIndex, hp, mp, atkMod, atkModClamped, defMod;
        [HideInInspector] public float timer;
        [HideInInspector] public bool barrier;

        // Start is called before the first frame update
        void Awake()
        {
            animator = GetComponent<Animator>();
            sfx = GetComponent<SFXManager>();
        }

        protected void Start()
        {
            hp = maxHP;
            mp = 100;
            onDeath.AddListener(ResetMods);
            onDeath.AddListener(delegate { timer = cooldown; });
        }

        protected void Update()
        {
            hpBar.fillAmount = (float)hp / maxHP;
            timerBar.fillAmount = (cooldown - timer) / cooldown;
            if (isPlayerUnit)
            {
                hpDisp.text = "HP " + hp + "/" + maxHP;
                mpBar.fillAmount = mp / 100f;
                mpDisp.text = "MP " + mp + "/100";
                if (timer > 0)
                {
                    timerDisp.text = "";
                } else
                {
                    timerDisp.text = "READY";
                }

                if (barrier && !barrierFX.activeInHierarchy) barrierFX.SetActive(true);
                if (!barrier && barrierFX.activeInHierarchy) barrierFX.SetActive(false);
            }
        }

        public void UseAction(int index)
        {
            if (index < 0 || index >= actions.Length) return;
            if (mp < actions[index].mpCost || microgame.actionInProgress) return;
            microgame.actionInProgress = true;
            mp -= actions[index].mpCost;
            timer = cooldown;
            actions[index].onUse.Invoke();
            microgame.SetAction(actions[index].name);
            animator.SetInteger("actionID", index);
            animator.SetTrigger("action");
        }

        [System.Serializable]
        public class UnitAction
        {
            public string name, description;
            public int mpCost;
            [HideInInspector] public UnityEvent onUse;
            public GameObject[] effects;
        }

        public void SetUnitDescription()
        {
            string description = LeanLocalization.GetTranslationText(unitName);
            description += "\nHP: " + hp + "/" + maxHP;
            description += "\nMP: " + mp + "/100";
            description += "\nATK: " + (baseAtk + atkModClamped);
            description += "\nDEF: " + (baseDef + defMod);
            microgame.SetDescription(description);
        }

        public void SetActionDescription(int index)
        {
            if (actions[index].description == "") {
                microgame.SetDescription("");
                return;
            }
            string description = LeanLocalization.GetTranslationText(actions[index].description);
            description += "\n\n" + actions[index].mpCost + " MP";
            microgame.SetDescription(description);
        }

        public void Damage(int atk, float potency)
        {
            int damage = Mathf.RoundToInt((atk - (baseDef + defMod)) * potency * Random.Range(0.9f, 1.1f));
            if (barrier) damage = Mathf.RoundToInt(damage / 2f);
            if (damage < 1) damage = 1;
            hp -= damage;
            if (hp < 0) hp = 0;
            animator.SetInteger("hp", hp);
            animator.SetTrigger("hurt");
            BossDefeatEffect effect = Instantiate(microgame.damageFX, (Vector2)transform.position + effectOffset, Quaternion.identity, microgame.transform).GetComponent<BossDefeatEffect>();
            effect.text.text = damage.ToString();
            microgame.currentMenu.menuItems[microgame.currentMenu.index].onHighlight.Invoke();
        }

        public void Heal(int amount)
        {
            int heal;
            if (hp + amount > maxHP)
            {
                heal = maxHP - hp;
                hp = maxHP;
            } else
            {
                heal = amount;
                hp += heal;
            }
            animator.SetInteger("hp", hp);
            BossDefeatEffect effect = Instantiate(microgame.healFX, (Vector2)transform.position + effectOffset, Quaternion.identity, microgame.transform).GetComponent<BossDefeatEffect>();
            effect.text.text = heal.ToString();
            microgame.currentMenu.menuItems[microgame.currentMenu.index].onHighlight.Invoke();
        }

        public void MPHeal(int amount)
        {
            int heal;
            if (mp + amount > 100)
            {
                heal = 100 - mp;
                mp = 100;
            }
            else
            {
                heal = amount;
                mp += heal;
            }
            BossDefeatEffect effect = Instantiate(microgame.mpHealFX, (Vector2)transform.position + effectOffset, Quaternion.identity, microgame.transform).GetComponent<BossDefeatEffect>();
            effect.text.text = heal.ToString();
            microgame.currentMenu.menuItems[microgame.currentMenu.index].onHighlight.Invoke();
        }

        public void ModifyAtk(int amount)
        {
            if (amount == 0) return;
            atkMod += amount;
            atkModClamped = atkMod < -Mathf.CeilToInt((float)baseAtk / 2) ? -Mathf.CeilToInt((float)baseAtk / 2) : atkMod;
            if (amount > 0)
            {
                BossDefeatEffect effect = Instantiate(microgame.buffFX, (Vector2)transform.position + effectOffset, Quaternion.identity, microgame.transform).GetComponent<BossDefeatEffect>();
                effect.text.text = "ATK↑";
            }
            else
            {
                BossDefeatEffect effect = Instantiate(microgame.debuffFX, (Vector2)transform.position + effectOffset, Quaternion.identity, microgame.transform).GetComponent<BossDefeatEffect>();
                effect.text.text = "ATK↓";
            }
            microgame.currentMenu.menuItems[microgame.currentMenu.index].onHighlight.Invoke();
        }

        public void ModifyDef(int amount)
        {
            if (amount == 0) return;
            defMod += amount;
            if (amount > 0)
            {
                BossDefeatEffect effect = Instantiate(microgame.buffFX, (Vector2)transform.position + effectOffset, Quaternion.identity, microgame.transform).GetComponent<BossDefeatEffect>();
                effect.text.text = "DEF↑";
            }
            else
            {
                BossDefeatEffect effect = Instantiate(microgame.debuffFX, (Vector2)transform.position + effectOffset, Quaternion.identity, microgame.transform).GetComponent<BossDefeatEffect>();
                effect.text.text = "DEF↓";
            }
            microgame.currentMenu.menuItems[microgame.currentMenu.index].onHighlight.Invoke();
        }

        public void ResetMods()
        {
            atkMod = 0;
            atkModClamped = atkMod;
            defMod = 0;
            microgame.currentMenu.menuItems[microgame.currentMenu.index].onHighlight.Invoke();
        }

        public void OnHitEvent()
        {
            onHit.Invoke();
        }

        public void OnDeathEvent()
        {
            onDeath.Invoke();
        }

        public void OnReviveEvent()
        {
            onRevive.Invoke();
        }

        public void OnWinEvent()
        {
            onWin.Invoke();
        }
    }
}