namespace Micro.Imitate
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using Lean.Localization;

    public class MicroImitate : Microgame
    {
        public SFXManager sfx;
        public SpeechSetArray speechSetArray;
        public TextMeshProUGUI speech;
        public Button[] buttons;
        public Animator kanataAnimator;
        public RectTransform canvas;
        public Vector2 avatarPosition;
        int correctAnswerIndex;
        List<int> choices;

        void Start()
        {
            List<int> setPool = Utils.GenerateIndexPool(speechSetArray.sets.Length);
            choices = Utils.RandomFromIntPool(setPool, buttons.Length);
            correctAnswerIndex = Random.Range(0, choices.Count);
            onStart.AddListener(Game);
        }

        public void Game()
        {
            for (int i = 0; i < choices.Count; i++)
            {
                buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = LeanLocalization.GetTranslationText("Interview/" + speechSetArray.sets[choices[i]].name + "_Greeting_" + Random.Range(0, speechSetArray.sets[choices[i]].numberOfLines));
            }
            AddAvatar(0);
            avatars.Add(Instantiate(speechSetArray.sets[choices[correctAnswerIndex]].avatar, canvas).GetComponent<CharacterAvatar>());
            avatars[1].transform.localScale = Vector3.one * 1.5f;
            avatars[1].GetComponent<RectTransform>().anchoredPosition = avatarPosition;
            bgm.PlayBGM(0);
            foreach (Button i in buttons)
            {
                i.interactable = true;
            }
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
            foreach (Button i in buttons)
            {
                i.interactable = false;
            }
        }

        public void CheckAnswer(int buttonIndex)
        {
            speech.transform.parent.gameObject.SetActive(true);
            speech.text = buttons[buttonIndex].GetComponentInChildren<TextMeshProUGUI>().text;
            kanataAnimator.SetTrigger("win");
            if (buttonIndex == correctAnswerIndex)
            {
                cleared = true;
                avatars[0].SetExpression(1);
                avatars[1].SetExpression(1);
                sfx.PlaySFX(0);
            } else
            {
                avatars[0].SetExpression(2);
                avatars[1].SetExpression(2);
                sfx.PlaySFX(1);
            }
            foreach (Button i in buttons)
            {
                i.interactable = false;
            }
        }
    }
}