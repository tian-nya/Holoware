namespace Micro.Solve
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;

    public class MicroSolve : Microgame
    {
        public SFXManager sfx;
        public NumberPad numberPad;
        public TextMeshProUGUI blackboard;
        public Animator chocoAnimator;
        int[] num;
        int op; // 0 = add, 1 = subtract, 2 = multiply, 3 = divide
        int numAsX;

        void Start()
        {
            num = new int[3];
            blackboard.text = "";
            op = Random.Range(0, 4);
            if (GameSettings.hardMode == 0)
            {
                numAsX = 2;
                switch (op)
                {
                    case 0:
                        num[0] = Random.Range(0, 26);
                        num[1] = Random.Range(0, 26);
                        num[2] = num[0] + num[1];
                        break;
                    case 1:
                        num[0] = Random.Range(0, 26);
                        num[1] = Random.Range(0, num[0] + 1);
                        num[2] = num[0] - num[1];
                        break;
                    case 2:
                        num[0] = Random.Range(0, 10);
                        num[1] = Random.Range(0, 10);
                        num[2] = num[0] * num[1];
                        break;
                    case 3:
                        num[2] = Random.Range(1, 10);
                        num[1] = Random.Range(1, 10);
                        num[0] = num[1] * num[2];
                        break;
                }
            }
            else
            {
                numAsX = Random.Range(0, 3);
                switch (op)
                {
                    case 0:
                        num[0] = Random.Range(-25, 26);
                        num[1] = Random.Range(-25, 26);
                        num[2] = num[0] + num[1];
                        break;
                    case 1:
                        num[0] = Random.Range(-25, 26);
                        num[1] = Random.Range(0, Mathf.Abs(num[0]) + 1) * (Random.Range(0, 2) == 1 ? 1 : -1);
                        num[2] = num[0] - num[1];
                        break;
                    case 2:
                        num[0] = Random.Range(1, 13) * (Random.Range(0, 2) == 1 ? 1 : -1);
                        num[1] = Random.Range(1, 13) * (Random.Range(0, 2) == 1 ? 1 : -1);
                        num[2] = num[0] * num[1];
                        break;
                    case 3:
                        num[2] = Random.Range(1, 13) * (Random.Range(0, 2) == 1 ? 1 : -1);
                        num[1] = Random.Range(1, 13) * (Random.Range(0, 2) == 1 ? 1 : -1);
                        num[0] = num[1] * num[2];
                        break;
                }
            }

            if (numAsX != 0)
            {
                blackboard.text += num[0] >= 0 ? num[0].ToString() : "(" + num[0] + ")";
            }
            else
            {
                blackboard.text += "<b><i>x</i></b>";
            }
            switch (op)
            {
                case 0: blackboard.text += " + "; break;
                case 1: blackboard.text += " - "; break;
                case 2: blackboard.text += " × "; break;
                case 3: blackboard.text += " ÷ "; break;
            }
            if (numAsX != 1)
            {
                blackboard.text += num[1] >= 0 ? num[1].ToString() : "(" + num[1] + ")";
            }
            else
            {
                blackboard.text += "<b><i>x</i></b>";
            }
            blackboard.text += " = ";
            if (numAsX != 2)
            {
                blackboard.text += num[2] >= 0 ? num[2].ToString() : "(" + num[2] + ")";
            }
            else
            {
                blackboard.text += GameSettings.hardMode == 1 ? "<b><i>x</i></b>" : "?";
            }
            if (GameSettings.hardMode == 1)
            {
                blackboard.text += "\n\n<b><i>x</i></b> = ?";
            }

            onStart.AddListener(Game);
        }

        public void Game()
        {
            AddAvatar(0);
            bgm.PlayBGM(0);
            StartCoroutine(GameCoroutine());
        }

        IEnumerator GameCoroutine()
        {
            while (timer > 0)
            {
                if (cleared)
                {
                    break;
                }
                yield return null;
            }
            numberPad.canUse = false;
        }

        public void CheckAnswer()
        {
            if (!numberPad.canUse) return;
            numberPad.canUse = false;
            if (int.Parse(numberPad.stringInput) == num[numAsX])
            {
                cleared = true;
                numberPad.display.color = Color.green;
                chocoAnimator.SetTrigger("win");
                sfx.PlaySFX(0);
                avatars[0].SetExpression(1);
            }
            else
            {
                numberPad.display.color = Color.red;
                chocoAnimator.SetTrigger("fail");
                sfx.PlaySFX(1);
                avatars[0].SetExpression(2);
            }
        }
    }
}