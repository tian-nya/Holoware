namespace Micro.Slice
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class MicroSlice : Microgame
    {
        public SFXManager sfx;
        public GameObject fruit;
        public Transform slash;
        public Animator playerAnimator, slashAnimator;
        public float spawnMinX = 2f, spawnMaxX = 5.5f, spawnY = -6f, minSpawnDelay = 0.333f, maxSpawnDelay = 1f, minFruitSpeedY = 10f, maxFruitSpeedY = 15f, slashWidth;
        public ParticleSystem slashParticle;
        int fruitsHit;
        float angle, speedYAverage;
        Collider2D[] hits;

        public void Start()
        {
            onStart.AddListener(Game);
            speedYAverage = (minFruitSpeedY + maxFruitSpeedY) / 2;
        }

        public void Game()
        {
            AddAvatar(0);
            bgm.PlayBGM(0);
            StartCoroutine(GameCoroutine());
            StartCoroutine(FruitSpawn());
        }

        IEnumerator GameCoroutine()
        {
            while (timer > 0)
            {
                angle = Vector2.SignedAngle(Vector2.right, Utils.GetMousePosition());
                slash.eulerAngles = new Vector3(0, 0, angle);
                if (Input.GetMouseButtonDown(0))
                {
                    playerAnimator.SetTrigger("slash");
                    slashAnimator.SetTrigger("slash");
                    sfx.PlaySFX(2);
                    slashParticle.Play();

                    hits = Physics2D.OverlapBoxAll(slash.position, new Vector2(40f, slashWidth), angle);
                    foreach (Collider2D i in hits)
                    {
                        if (i.tag == "Pickup")
                        {
                            fruitsHit++;
                            i.GetComponent<MicroSliceFruit>().Split(angle);
                        }
                    }

                    if (fruitsHit >= 2)
                    {
                        cleared = true;
                    }
                    break;
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
            if (fruitsHit > 0)
            {
                sfx.PlaySFX(3);
            }
            yield return new WaitForSeconds(0.4f);
            if (cleared)
            {
                sfx.PlaySFX(0);
                avatars[0].SetExpression(1);
            } else
            {
                sfx.PlaySFX(1);
                avatars[0].SetExpression(2);
            }
        }

        IEnumerator FruitSpawn()
        {
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
            MicroSliceFruit spawnedFruit1, spawnedFruit2;
            spawnedFruit1 = Instantiate(fruit, new Vector2(Random.Range(spawnMinX, spawnMaxX), spawnY), Quaternion.identity, transform).GetComponent<MicroSliceFruit>();
            spawnedFruit1.microgame = this;
            spawnedFruit1.Throw(Random.Range(minFruitSpeedY, maxFruitSpeedY));
            spawnedFruit2 = Instantiate(fruit, new Vector2(Random.Range(-spawnMaxX, -spawnMinX), spawnY), Quaternion.identity, transform).GetComponent<MicroSliceFruit>();
            spawnedFruit2.microgame = this;
            spawnedFruit2.Throw(Random.Range(spawnedFruit1.whole.velocity.y < speedYAverage ? speedYAverage : minFruitSpeedY, maxFruitSpeedY));
        }
    }
}