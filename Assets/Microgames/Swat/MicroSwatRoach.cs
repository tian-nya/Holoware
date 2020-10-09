namespace Micro.Swat
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D))]
    public class MicroSwatRoach : MonoBehaviour
    {
        [HideInInspector] public MicroSwat microgame;
        public float speed = 5f;
        Rigidbody2D rb;
        bool reachedPlayer;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            if (transform.position.x <= microgame.player.transform.position.x)
            {
                reachedPlayer = true;
            }
            if (!reachedPlayer)
            {
                rb.velocity = new Vector2(-speed, 0f);
            } else
            {
                rb.velocity = (microgame.player.transform.position - transform.position).normalized * speed;
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                microgame.canMove = false;
                microgame.cleared = false;
            }
        }
    }
}