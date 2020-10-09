namespace Skins
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class SkinCharacterFeature : Skin
    {
        public Image image;
        public Animator charAnimator;
        public SpriteSet[] spriteSets;

        protected override void OnEnable()
        {
            gameStart.Invoke();
        }

        [System.Serializable]
        public class SpriteSet
        {
            public Sprite[] sprites;
        }

        public void SetSprite (int index)
        {
            image.sprite = spriteSets[index].sprites[Random.Range(0, spriteSets[index].sprites.Length)];
            charAnimator.SetTrigger("switch");
        }
    }
}