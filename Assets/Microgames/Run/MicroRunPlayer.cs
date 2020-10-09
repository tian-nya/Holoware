namespace Micro.Run
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class MicroRunPlayer : MonoBehaviour
    {
        public MicroRun microgame;
        public SpriteRenderer spriteRenderer;
        public Animator animator, doorAnimator;
        [HideInInspector] public Rigidbody2D rb;
        public Rigidbody2D fg;
        [HideInInspector] public Sprite runSprite, slideSprite, failSprite;
        public float speed = 12f, jump = 15f, slideTime = 0.333f, cameraLeftBound, cameraRightBound, cameraOffset;
        public ParticleSystem hitParticle;
        bool hit, grounded, sliding;
        float slideTimer;
        SFXManager sfx;

        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            sfx = GetComponent<SFXManager>();
            grounded = true;
            StartCoroutine(GameCoroutine());
            StartCoroutine(Move());
        }

        IEnumerator GameCoroutine()
        {
            while (!hit && !microgame.cleared) {
                if (sliding)
                {
                    slideTimer -= Time.deltaTime;
                    if (slideTimer <= 0 && !Input.GetButton("Down"))
                    {
                        sliding = false;
                        spriteRenderer.sprite = runSprite;
                        animator.SetTrigger("run");
                    }
                }
                if (grounded && !sliding)
                {
                    if (Input.GetButtonDown("Up"))
                    {
                        animator.SetTrigger("jump");
                        grounded = false;
                        rb.velocity += Vector2.up * jump;
                        sfx.PlaySFX(0);
                    }
                    else if (Input.GetButtonDown("Down"))
                    {
                        sliding = true;
                        animator.SetTrigger("slide");
                        slideTimer = slideTime;
                        sfx.PlaySFX(1);
                        spriteRenderer.sprite = slideSprite;
                    }
                }
                
                yield return null;
            }
        }

        IEnumerator Move()
        {
            while (!hit && !microgame.cleared)
            {
                Camera.main.transform.position = new Vector3(Mathf.Clamp(transform.position.x + cameraOffset, cameraLeftBound, cameraRightBound), 0, Camera.main.transform.position.z);
                fg.MovePosition(new Vector2(Mathf.Clamp(transform.position.x + cameraOffset, cameraLeftBound, cameraRightBound), 0));
                rb.velocity = new Vector2(speed, rb.velocity.y);
                yield return new WaitForFixedUpdate();
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (hit || microgame.timeOver || microgame.cleared) return;
            if (collision.tag == "Goal")
            {
                microgame.cleared = true;
                rb.velocity = Vector2.zero;
                microgame.sfx.PlaySFX(0);
                microgame.avatars[0].SetExpression(1);
                transform.position = doorAnimator.transform.position;
                animator.SetTrigger("run");
                doorAnimator.SetTrigger("win");
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (hit || microgame.timeOver || microgame.cleared) return;
            if (collision.gameObject.tag == "Untagged")
            {
                grounded = true;
                spriteRenderer.sprite = runSprite;
                animator.SetTrigger("run");
            }
            if (collision.gameObject.tag == "Hazard")
            {
                hit = true;
                collision.otherCollider.enabled = false;
                rb.velocity = new Vector2(-speed, speed);
                rb.constraints &= ~RigidbodyConstraints2D.FreezeRotation;
                rb.angularVelocity = 360f;
                spriteRenderer.sprite = failSprite;
                animator.SetTrigger("hit");
                hitParticle.Play();
                sfx.PlaySFX(2);
                microgame.sfx.PlaySFX(1);
                microgame.avatars[0].SetExpression(2);
            }
        }
    }
}