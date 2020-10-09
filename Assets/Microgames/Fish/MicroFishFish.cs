namespace Micro.Fish
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D))]
    public class MicroFishFish : MonoBehaviour
    {
        Rigidbody2D rb;
        public SpriteRenderer sprite;
        [HideInInspector] public MicroFish microgame;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player" && !microgame.caughtFish)
            {
                microgame.caughtFish = this;
                rb.simulated = false;
                transform.SetParent(microgame.hookTip);
                transform.localPosition = Vector2.zero;
                transform.eulerAngles = new Vector3(0, 0, -90);
                sprite.flipX = true;
                microgame.sfx.PlaySFX(1);
                if (microgame.cArrow) Destroy(microgame.cArrow);
                microgame.ccArrow.SetActive(true);
            }
        }
    }
}