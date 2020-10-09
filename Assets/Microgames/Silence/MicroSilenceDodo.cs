namespace Micro.Silence
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D))]
    public class MicroSilenceDodo : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        Rigidbody2D rb;
        SFXManager sfx;
        bool dead;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            sfx = GetComponent<SFXManager>();
            StartCoroutine(Sounds());
        }

        void Update()
        {
            if (!dead)
            {
                spriteRenderer.flipX = rb.velocity.x > 0 ? false : true;
            }
        }

        IEnumerator Sounds()
        {
            while (!dead)
            {
                sfx.PlaySFX(0);
                yield return new WaitForSeconds(Random.Range(0.5f, 2f));
            }
        }

        public void Die()
        {
            sfx.PlaySFX(1);
            dead = true;
        }
    }
}