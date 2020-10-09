namespace Micro
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using UnityEngine.Events;

    public class NumberPad : MonoBehaviour
    {
        public TextMeshProUGUI display;
        public string stringInput;
        public int maxDigits = 5;
        public bool canUse;

        void Start()
        {
            display.text = stringInput;
        }

        public void InputNumber(int number)
        {
            if (!canUse) return;
            if (stringInput.Length >= (stringInput.StartsWith("-") ? maxDigits + 1 : maxDigits)) return;
            stringInput += number.ToString();
            display.text = stringInput;
        }

        public void Backspace()
        {
            if (!canUse) return;
            stringInput = stringInput.Remove(stringInput.Length - 1);
            display.text = stringInput;
        }

        public void Clear()
        {
            if (!canUse) return;
            stringInput = "";
            display.text = stringInput;
        }

        public void Negate()
        {
            if (!canUse) return;
            if (stringInput.Length == 0)
            {
                stringInput = "-";
            } else
            {
                if (stringInput.StartsWith("-"))
                {
                    stringInput = stringInput.Remove(0, 1);
                } else
                {
                    stringInput = stringInput.Insert(0, "-");
                }
            }
            display.text = stringInput;
        }
    }
}