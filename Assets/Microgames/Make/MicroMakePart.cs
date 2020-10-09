namespace Micro.Make
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MicroMakePart : MonoBehaviour
    {
        [HideInInspector] public int id;
        public float height = 0.25f;
        [HideInInspector] public SpriteRenderer sr;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }
    }
}