namespace Micro.Swat
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class MicroSwat : Microgame
    {
        public SFXManager sfx;
        public GameObject player, roach, target;
        public Animator playerAnimator;
        public int roachAmount = 3;
        public float lowerBound, upperBound, swatCooldown = 0.25f, spawnX = 8f, spawnMinInterval = 0.5f, spawnMaxInterval = 1f;
        public Vector2 swatSize;
        public ParticleSystem hitParticle, deadParticle;
        int roachesHit;
        float cooldown;
        [HideInInspector] public bool canMove;

        public void Start()
        {
            onStart.AddListener(Game);
            canMove = true;
        }

        public void Game()
        {
            AddAvatar(0);
            bgm.PlayBGM(0);
            StartCoroutine(GameCoroutine());
            StartCoroutine(RoachSpawn());
        }

        IEnumerator GameCoroutine()
        {
            while (timer > 0)
            {
                if (!canMove)
                {
                    playerAnimator.SetTrigger("dead");
                    deadParticle.Play();
                    sfx.PlaySFX(1);
                    avatars[0].SetExpression(2);
                    break;
                }
                player.transform.position = new Vector2(player.transform.position.x, Mathf.Clamp(Utils.GetMousePosition().y, lowerBound, upperBound));
                if (cooldown > 0)
                {
                    cooldown -= Time.deltaTime;
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    Swat(target.transform.position);
                }
                yield return null;
            }
        }

        IEnumerator RoachSpawn()
        {
            MicroSwatRoach spawnedRoach;
            for (int i = 0; i < roachAmount; i++)
            {
                yield return new WaitForSeconds(Random.Range(spawnMinInterval, spawnMaxInterval));
                spawnedRoach = Instantiate(roach, new Vector2(spawnX, Random.Range(lowerBound, upperBound)), Quaternion.identity, transform).GetComponent<MicroSwatRoach>();
                spawnedRoach.microgame = this;
            }
        }

        void Swat(Vector2 position)
        {
            Collider2D[] hits;
            bool hitRoach = false;
            playerAnimator.SetTrigger("swat");
            hits = Physics2D.OverlapBoxAll(position, swatSize, 0f);
            foreach (Collider2D i in hits)
            {
                if (i.tag == "Enemy")
                {
                    roachesHit++;
                    hitRoach = true;
                    Destroy(i.gameObject);
                }
            }
            sfx.PlaySFX(hitRoach ? 3 : 2);
            hitParticle.Play();
            cooldown = swatCooldown;
            if (!cleared && roachesHit >= roachAmount)
            {
                cleared = true;
                avatars[0].SetExpression(1);
                sfx.PlaySFX(0);
            }
        }
    }
}