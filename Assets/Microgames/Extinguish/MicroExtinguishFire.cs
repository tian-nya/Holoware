namespace Micro.Extinguish
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MicroExtinguishFire : MonoBehaviour
    {
        [HideInInspector] public MicroExtinguish microgame;
        public ParticleSystem fireParticle;
        public float size = 3f;

        private void Start()
        {
            transform.localScale = Vector3.one * size;
        }

        void Update()
        {
            if (size <= 0.333f)
            {
                microgame.fireAmount--;
                fireParticle.transform.SetParent(microgame.transform);
                fireParticle.Stop();
                Destroy(gameObject);
            }
            transform.localScale = Vector3.one * (size > 0 ? size : 0);
        }
    }
}