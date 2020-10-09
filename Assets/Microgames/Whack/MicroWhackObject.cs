namespace Micro.Whack
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MicroWhackObject : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public Sprite hitSprite;
        Animator animator;
        [HideInInspector] public MicroWhack microgame;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Hit()
        {
            animator.SetTrigger("hit");
            spriteRenderer.sprite = hitSprite;
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}