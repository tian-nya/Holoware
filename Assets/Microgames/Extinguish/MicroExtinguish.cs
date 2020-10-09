namespace Micro.Extinguish
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class MicroExtinguish : Microgame
    {
        public SFXManager sfx, fireSfx;
        public GameObject fire;
        public Transform player;
        public LineRenderer[] water;
        public int fireAmount = 5;
        public float xBound = 5f, spawnLowerBound, spawnUpperBound, waterSpeed = 10f, waterRaycastLength = 8f, waterExtinguishRate = 0.4f;
        AudioSource fireSfxSource;
        int maxFire;
        float raycastLength;
        RaycastHit2D hit;

        public void Start()
        {
            fireSfxSource = fireSfx.GetComponent<AudioSource>();
            maxFire = fireAmount;
            FireSpawn();
            onStart.AddListener(Game);
        }

        public void Game()
        {
            AddAvatar(0);
            bgm.PlayBGM(0);
            StartCoroutine(GameCoroutine());
        }

        IEnumerator GameCoroutine()
        {
            sfx.PlaySFX(1);
            while (timer > 0)
            {
                if (fireAmount == 0 && !cleared)
                {
                    cleared = true;
                    avatars[0].SetExpression(1);
                    sfx.PlaySFX(0);
                    fireSfxSource.volume = 0;
                }
                fireSfxSource.volume = GameSettings.sfxVolume * fireSfx.sfxData[0].volume * Utils.Sin01((float)fireAmount / maxFire);
                player.position = new Vector2(Mathf.Clamp(Utils.GetMousePosition().x, -xBound, xBound), player.position.y);

                foreach(LineRenderer i in water)
                {
                    raycastLength = Mathf.Clamp(Mathf.Abs(i.GetPosition(1).y - waterSpeed * Time.fixedDeltaTime), 0, waterRaycastLength);
                    if (!cleared)
                    {
                        hit = Physics2D.Raycast(i.transform.position, Vector2.down, raycastLength);
                        if (hit)
                        {
                            if (hit.collider.tag == "Hazard")
                            {
                                hit.collider.GetComponent<MicroExtinguishFire>().size -= waterExtinguishRate * Time.fixedDeltaTime;
                                raycastLength = Mathf.Abs(hit.point.y - i.transform.position.y);
                            }
                        }
                    }
                    i.SetPosition(1, Vector3.up * -raycastLength);
                }

                yield return new WaitForFixedUpdate();
            }
        }

        void FireSpawn()
        {
            MicroExtinguishFire spawnedFire;
            for (int i = 0; i < fireAmount; i++)
            {
                spawnedFire = Instantiate(fire, new Vector2(Random.Range(-xBound, xBound), Random.Range(spawnLowerBound, spawnUpperBound)), Quaternion.identity, transform).GetComponent<MicroExtinguishFire>();
                spawnedFire.microgame = this;
            }
        }
    }
}