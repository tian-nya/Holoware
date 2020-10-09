namespace Micro.Slice
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MicroSliceFruit : MonoBehaviour
    {
        [HideInInspector] public MicroSlice microgame;
        public Rigidbody2D whole, topHalf, bottomHalf;
        public Sprite[] topHalfSprites, bottomHalfSprites;
        public float speedX = 1f, turnSpeed = 180f, splitSpeedX = 0.5f, splitSpeedY = 3f, splitTurnSpeed = 360f;

        public void Throw(float speedY)
        {
            whole.velocity = new Vector2(Random.Range(-speedX, speedX), speedY);
            whole.angularVelocity = Random.Range(-turnSpeed, turnSpeed);
        }

        public void Split(float angle)
        {
            transform.eulerAngles = new Vector3(0, 0, angle);
            topHalf.gameObject.SetActive(true);
            topHalf.transform.SetParent(microgame.transform);
            topHalf.GetComponent<SpriteRenderer>().sprite = topHalfSprites[Random.Range(0, topHalfSprites.Length)];
            topHalf.simulated = true;
            topHalf.velocity = transform.right * Random.Range(-splitSpeedX, splitSpeedX) + transform.up * splitSpeedY;
            topHalf.angularVelocity = Random.Range(-splitTurnSpeed, splitTurnSpeed);
            bottomHalf.gameObject.SetActive(true);
            bottomHalf.transform.SetParent(microgame.transform);
            bottomHalf.GetComponent<SpriteRenderer>().sprite = bottomHalfSprites[Random.Range(0, bottomHalfSprites.Length)];
            bottomHalf.simulated = true;
            bottomHalf.velocity = transform.right * Random.Range(-splitSpeedX, splitSpeedX) - transform.up * splitSpeedY;
            bottomHalf.angularVelocity = Random.Range(-splitTurnSpeed, splitTurnSpeed);
            Destroy(gameObject);
        }
    }
}
