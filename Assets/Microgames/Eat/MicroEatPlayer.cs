namespace Micro.Eat
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class MicroEatPlayer : MonoBehaviour
    {
        public MicroEat microgame;
        public SpriteRenderer spriteRenderer;
        public Animator animator;
        [HideInInspector] public Rigidbody2D rb;
        public Sprite failSprite;
        public Image bar;
        public float speed = 5f;
        public int winPoints = 20;
        bool canMove;
        SFXManager sfx;
        int points;

        // Start is called before the first frame update
        void Awake()
        {
            canMove = true;
            rb = GetComponent<Rigidbody2D>();
            sfx = GetComponent<SFXManager>();
            StartCoroutine(Move());
        }

        IEnumerator Move()
        {
            while (canMove) {
                rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * speed, 0);
                if (Input.GetAxisRaw("Horizontal") != 0) spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") < 0 ? true : false;
                animator.SetFloat("speed", Input.GetAxisRaw("Horizontal") != 0 ? 1 : 0);
                bar.fillAmount = (float)points / winPoints;
                yield return null;
            }
            rb.velocity = Vector2.zero;
            animator.SetFloat("speed", 0f);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!canMove || microgame.timeOver) return;
            if (collision.tag == "Pickup")
            {
                Destroy(collision.gameObject);
                points++;
                if (points == winPoints)
                {
                    sfx.PlaySFX(1);
                    bar.color = Color.green;
                    microgame.cleared = true;
                    microgame.avatars[0].SetExpression(1);
                } else
                {
                    sfx.PlaySFX(0);
                }
            } else if (collision.tag == "Hazard")
            {
                Destroy(collision.gameObject);
                sfx.PlaySFX(2);
                sfx.PlaySFX(3);
                bar.color = Color.red;
                microgame.cleared = false;
                canMove = false;
                spriteRenderer.sprite = failSprite;
                animator.SetTrigger("down");
                microgame.avatars[0].SetExpression(2);
            }
        }
    }
}