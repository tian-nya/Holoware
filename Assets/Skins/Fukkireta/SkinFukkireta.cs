namespace Skins
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using UnityEngine.Events;
    using Lean.Localization;

    public class SkinFukkireta : Skin
    {
        public Image characterImg1, characterImg2, bg1, bg2;
        public TextMeshProUGUI nameDisp, birthdayDisp, heightDisp, flavorDisp;
        public CharacterData[] characterData;
        int index;

        [System.Serializable]
        public struct CharacterData
        {
            public string name, birthday, height;
            public string[] flavorText;
            public Color bgColor1, bgColor2;
            public Sprite[] sprites;
        }

        protected override void OnEnable()
        {
            index = Random.Range(0, characterData.Length);
            StartCoroutine(SetCharacter(0));
            microPrep.AddListener(delegate { StartCoroutine(SetCharacter(0, false)); });
            microWin.AddListener(delegate { StartCoroutine(SetCharacter(1, true)); });
            microFail.AddListener(delegate { StartCoroutine(SetCharacter(2, true)); });
            speedUp.AddListener(delegate { StartCoroutine(SetCharacter(0, false)); });
            boss.AddListener(delegate { StartCoroutine(SetCharacter(0, false)); });
            gameStart.Invoke();
        }

        IEnumerator SetCharacter(int expression, bool newCharacter = false)
        {
            yield return null;
            if (newCharacter) index = Random.Range(0, characterData.Length);
            nameDisp.text = LeanLocalization.GetTranslationText(characterData[index].name);
            flavorDisp.text = "";
            for (int i = 0; i < characterData[index].flavorText.Length; i++)
            {
                if (i > 0) flavorDisp.text += "\n";
                flavorDisp.text += LeanLocalization.GetTranslationText(characterData[index].flavorText[i]);
            }
            birthdayDisp.text = characterData[index].birthday;
            heightDisp.text = characterData[index].height;
            bg1.color = characterData[index].bgColor1;
            bg2.color = characterData[index].bgColor2;
            characterImg1.sprite = characterData[index].sprites[expression];
            characterImg2.sprite = characterData[index].sprites[expression];
        }
    }
}