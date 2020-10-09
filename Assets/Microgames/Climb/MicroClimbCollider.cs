namespace Micro.Climb {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(Collider2D))]
    public class MicroClimbCollider : MonoBehaviour
    {
        MicroClimbPlayer player;
        public int sfxIndex;
        public float maxVolumeSpeed = 30f;
        float speed;
        Vector3 lastPosition;

        void Awake()
        {
            player = GetComponentInParent<MicroClimbPlayer>();
            lastPosition = transform.position;
        }

        private void FixedUpdate()
        {
            speed = (transform.position - lastPosition).magnitude / Time.fixedDeltaTime;
            lastPosition = transform.position;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            player.sfx.PlaySFX(sfxIndex, Mathf.Clamp01(speed / maxVolumeSpeed));
        }
    }
}