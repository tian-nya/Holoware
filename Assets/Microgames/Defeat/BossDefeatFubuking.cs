namespace Micro.Defeat
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using UnityEngine.Events;
    using Lean.Localization;

    public class BossDefeatFubuking : BossDefeatUnit
    {
        public TextMeshProUGUI nextActionDisp;
        public Transform targetImg;
        [HideInInspector] public int nextActionIndex;
        int turnNumber, targetIndex, buffUpgrade;

        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            unitIndex = -1;
            timer = cooldown;
            turnNumber = 1;
            targetIndex = -1;

            actions[0].onUse.AddListener(delegate { StartCoroutine(Attack()); });
            actions[1].onUse.AddListener(delegate { StartCoroutine(WhiteFang()); });
            actions[2].onUse.AddListener(delegate { StartCoroutine(IcicleFall()); });
            actions[3].onUse.AddListener(delegate { StartCoroutine(Blizzard()); });
            actions[4].onUse.AddListener(delegate { StartCoroutine(FoxyWiles()); });
        }

        // Update is called once per frame
        new void Update()
        {
            base.Update();
        }

        public void SetNextAction()
        {
            if (microgame.CheckForAllUnitsDown()) return;
            if (turnNumber % 6 == 0)
            {
                nextActionIndex = 4;
            } else if (turnNumber % 2 == 0) {
                nextActionIndex = Random.Range(1, 4);
                if (nextActionIndex == 1)
                {
                    if (targetIndex < 0) Retarget(RandomTarget());
                }
            } else
            {
                nextActionIndex = 0;
                if (targetIndex < 0) Retarget(RandomTarget());
            }

            nextActionDisp.text = "NEXT: " + LeanLocalization.GetTranslationText(actions[nextActionIndex].name);
            turnNumber++;
        }

        public int RandomTarget()
        {
            List<int> livingUnitIndices = new List<int>();
            for (int i = 0; i < microgame.units.Length; i++)
            {
                if (microgame.units[i].hp > 0)
                {
                    livingUnitIndices.Add(i);
                }
            }
            if (livingUnitIndices.Count == 0) return -1;
            return livingUnitIndices[Random.Range(0, livingUnitIndices.Count)];
        }

        public void Retarget(int index)
        {
            targetIndex = index;
            targetImg.position = (Vector2)microgame.units[index].transform.position + microgame.units[index].effectOffset;
            targetImg.gameObject.SetActive(true);
        }

        IEnumerator Attack()
        {
            targetImg.gameObject.SetActive(false);
            Instantiate(actions[0].effects[0], (Vector2)microgame.units[targetIndex].transform.position + microgame.units[targetIndex].effectOffset, Quaternion.identity, microgame.units[targetIndex].transform);
            yield return new WaitForSeconds(0.25f);
            microgame.units[targetIndex].Damage(baseAtk + atkMod, 1f);
            yield return new WaitForSeconds(0.75f);
            for (int i = 0; i < microgame.units.Length; i++)
            {
                microgame.units[i].barrier = false;
            }
            targetIndex = -1;
            SetNextAction();
            microgame.actionInProgress = false;
        }

        IEnumerator WhiteFang()
        {
            targetImg.gameObject.SetActive(false);
            Instantiate(actions[1].effects[0], (Vector2)microgame.units[targetIndex].transform.position + microgame.units[targetIndex].effectOffset, Quaternion.identity, microgame.units[targetIndex].transform);
            yield return new WaitForSeconds(0.25f);
            microgame.units[targetIndex].Damage(baseAtk + atkMod, 3f);
            yield return new WaitForSeconds(0.75f);
            if (microgame.units[targetIndex].hp > 0)
            {
                microgame.units[targetIndex].ModifyAtk(GameSettings.hardMode == 1 ? -2 : -1);
                microgame.sfx.PlaySFX(5);
            }
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < microgame.units.Length; i++)
            {
                microgame.units[i].barrier = false;
            }
            targetIndex = -1;
            SetNextAction();
            microgame.actionInProgress = false;
        }

        IEnumerator IcicleFall()
        {
            targetImg.gameObject.SetActive(false);
            Instantiate(actions[2].effects[0], (Vector2)transform.position + effectOffset, Quaternion.identity, transform);
            yield return new WaitForSeconds(1f);
            int target;
            for (int i = 0; i < (GameSettings.hardMode == 1 ? 12 : 8); i++)
            {
                if (targetIndex != -1)
                {
                    microgame.units[targetIndex].Damage(baseAtk + atkMod, 0.5f);
                    if (microgame.units[targetIndex].hp < 1) targetIndex = -1;
                }
                else
                {
                    target = RandomTarget();
                    if (target != -1) microgame.units[target].Damage(baseAtk + atkMod, 0.5f);
                }
                yield return new WaitForSeconds(GameSettings.hardMode == 1 ? 0.083f : 0.125f);
            }
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < microgame.units.Length; i++)
            {
                microgame.units[i].barrier = false;
            }
            targetIndex = -1;
            SetNextAction();
            microgame.actionInProgress = false;
        }

        IEnumerator Blizzard()
        {
            targetImg.gameObject.SetActive(false);
            Instantiate(actions[3].effects[0], (Vector2)transform.position + effectOffset, Quaternion.identity, transform);
            yield return new WaitForSeconds(2f);
            for (int i = 0; i < microgame.units.Length; i++)
            {
                if (microgame.units[i].hp > 0) microgame.units[i].Damage(baseAtk + atkMod, 1.5f);
            }
            yield return new WaitForSeconds(1f);
            if (GameSettings.hardMode == 1 && !microgame.CheckForAllUnitsDown())
            {
                for (int i = 0; i < microgame.units.Length; i++)
                {
                    if (microgame.units[i].hp > 0) microgame.units[i].ModifyDef(-1);
                }
                microgame.sfx.PlaySFX(5);
                yield return new WaitForSeconds(0.5f);
            }
            for (int i = 0; i < microgame.units.Length; i++)
            {
                microgame.units[i].barrier = false;
            }
            targetIndex = -1;
            SetNextAction();
            microgame.actionInProgress = false;
        }

        IEnumerator FoxyWiles()
        {
            Instantiate(actions[4].effects[0], (Vector2)transform.position + effectOffset, Quaternion.identity, transform);
            yield return new WaitForSeconds(1f);
            ModifyAtk(2 + buffUpgrade);
            microgame.sfx.PlaySFX(4);
            yield return new WaitForSeconds(0.5f);
            ModifyDef(1 + buffUpgrade);
            microgame.sfx.PlaySFX(4);
            yield return new WaitForSeconds(0.5f);
            if (GameSettings.hardMode == 1) buffUpgrade++;
            SetNextAction();
            microgame.actionInProgress = false;
        }
    }
}