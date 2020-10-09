namespace Micro.Fish
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class MicroFish : Microgame
    {
        public SFXManager sfx;
        public GameObject fish, cArrow, ccArrow;
        public Animator armAnimator;
        public Transform reel, tip, hook, hookTip;
        public float lowerBound, upperBound, waterLevel = 0f, reelSpeed = 1f, clickInterval = 30f;
        public int fishAmount = 3;
        public float spawnLowerBound, spawnUpperBound, spawnMinX = 10f, spawnMaxX = 15f, fishMinSpeed = 3f, fishMaxSpeed = 5f;
        public LineRenderer line;
        public ParticleSystem splashParticle;
        float angle, angleDiff, rotationUntilClick;
        [HideInInspector] public MicroFishFish caughtFish;

        public void Start()
        {
            onStart.AddListener(Game);
            rotationUntilClick = clickInterval;
        }

        public void Game()
        {
            AddAvatar(0);
            bgm.PlayBGM(0);
            StartCoroutine(GameCoroutine());
            MicroFishFish spawnedFish;
            int side;
            for (int i = 0; i < fishAmount; i++)
            {
                side = Random.value < 0.5 ? -1 : 1;
                spawnedFish = Instantiate(fish, new Vector2(Random.Range(spawnMinX, spawnMaxX) * side, Random.Range(spawnLowerBound, spawnUpperBound)), Quaternion.identity, transform).GetComponent<MicroFishFish>();
                spawnedFish.sprite.flipX = side < 0 ? false : true;
                spawnedFish.GetComponent<Rigidbody2D>().velocity = Vector2.right * Random.Range(fishMinSpeed, fishMaxSpeed) * -side;
                spawnedFish.microgame = this;
            }
        }

        void Update()
        {
            line.SetPositions(new Vector3[] { tip.position + Vector3.back, hook.position + Vector3.back });
        }

        IEnumerator GameCoroutine()
        {
            while (timer > 0)
            {
                if (!cleared)
                {
                    angleDiff = Utils.ConvertReflexAngle(Vector2.SignedAngle(Vector2.right, Utils.GetMousePosition() - (Vector2)reel.position) - angle);
                    angle = Vector2.SignedAngle(Vector2.right, Utils.GetMousePosition() - (Vector2)reel.position);
                } else
                {
                    if (hook.position.y < upperBound)
                    {
                        angleDiff = 1080f / reelSpeed * Time.fixedDeltaTime;
                        angle += angleDiff;
                    } else
                    {
                        angleDiff = 0;
                    }
                }

                if (hook.position.y > waterLevel && hook.position.y + (angleDiff / 360f * reelSpeed) <= waterLevel)
                {
                    sfx.PlaySFX(2);
                } else if (hook.position.y + (angleDiff / 360f * reelSpeed) >= waterLevel && !cleared && caughtFish)
                {
                    cleared = true;
                    avatars[0].SetExpression(1);
                    sfx.PlaySFX(0);
                    sfx.PlaySFX(3);
                    splashParticle.Play();
                }

                hook.position += Vector3.up * (angleDiff / 360f * reelSpeed);
                hook.position = new Vector2(hook.position.x, Mathf.Clamp(hook.position.y, lowerBound, upperBound));
                rotationUntilClick -= Mathf.Abs(angleDiff);
                if (rotationUntilClick <= 0)
                {
                    sfx.PlaySFX(4);
                    rotationUntilClick = clickInterval;
                }
                armAnimator.SetFloat("angle", angle % 360f / 360f);

                yield return new WaitForFixedUpdate();
            }
        }
    }
}