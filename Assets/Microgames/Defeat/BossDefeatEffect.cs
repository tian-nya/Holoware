namespace Micro.Defeat
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;

    public class BossDefeatEffect : MonoBehaviour
    {
        public TextMeshPro text;
        public float lifeTime = 1f;

        void Start()
        {
            Destroy(gameObject, lifeTime);
        }
    }
}