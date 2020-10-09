namespace Micro.Match
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D))]
    public class MicroMatchEars : MonoBehaviour
    {
        [HideInInspector] public int index;
        [HideInInspector] public List<Collider2D> inContact;
        [HideInInspector] public Rigidbody2D rb;
        public SpriteRenderer spriteRenderer;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            inContact = new List<Collider2D>();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Goal")
            {
                for (int i = 0; i < inContact.Count; i++)
                {
                    if (inContact[i] == collision)
                    {
                        return;
                    }
                }
                inContact.Add(collision);
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Goal")
            {
                foreach (Collider2D i in inContact)
                {
                    if (i == collision)
                    {
                        inContact.Remove(i);
                        break;
                    }
                }
            }
        }
    }
}