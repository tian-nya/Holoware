namespace Micro.Silence
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class MicroSilence : Microgame
    {
        public SFXManager sfx;
        public GameObject crosshair, dodo, explosion;
        public int dodoAmount = 10, dodoTarget = 6, ammo = 3;
        public float dodoSpeedMin = 3f, dodoSpeedMax = 10f, explosionRadius = 2f, rocketCooldown = 0.333f;
        public Text ammoCount, dodoCount;
        int dodosHit;
        float cooldown;

        public void Start()
        {
            onStart.AddListener(Game);
            Rigidbody2D rb;
            for (int i = 0; i < dodoAmount; i++)
            {
                rb = Instantiate(dodo, new Vector2(Random.Range(-5f, 5f), Random.Range(-3.5f, 3.5f)), Quaternion.identity, transform).GetComponent<Rigidbody2D>();
                rb.velocity = Random.insideUnitCircle.normalized * Random.Range(dodoSpeedMin, dodoSpeedMax);
            }
        }

        public void Game()
        {
            AddAvatar(0);
            bgm.PlayBGM(0);
            StartCoroutine(GameCoroutine());
        }

        IEnumerator GameCoroutine()
        {
            while (timer > 0)
            {
                if (cooldown > 0)
                {
                    cooldown -= Time.deltaTime;
                }
                else if (Input.GetMouseButtonDown(0) && ammo > 0)
                {
                    StartCoroutine(Explosion(Utils.GetMousePosition()));
                    ammo--;
                    iTween.ScaleFrom(crosshair, iTween.Hash("scale", new Vector3(1.2f, 1.2f, 1f), "time", rocketCooldown, "easetype", iTween.EaseType.easeOutBack));
                }
                ammoCount.text = ammo.ToString();
                dodoCount.text = dodosHit.ToString() + "/" + dodoTarget.ToString();
                crosshair.transform.position = Utils.GetMousePosition();
                yield return null;
            }
        }

        IEnumerator Explosion(Vector2 position)
        {
            Collider2D[] hits;
            Rigidbody2D rb;
            sfx.PlaySFX(1);
            yield return new WaitForSeconds(0.2f);
            Instantiate(explosion, position, Quaternion.identity, transform);
            sfx.PlaySFX(2);
            hits = Physics2D.OverlapCircleAll(position, explosionRadius);
            foreach (Collider2D i in hits)
            {
                if (i.tag == "Enemy")
                {
                    i.enabled = false;
                    rb = i.GetComponent<Rigidbody2D>();
                    rb.velocity = ((Vector2)i.transform.position - position).normalized * 20f;
                    rb.constraints &= ~RigidbodyConstraints2D.FreezeRotation;
                    rb.angularVelocity = Random.Range(360f, 1440f) * (Random.value < 0.5f ? 1 : -1);
                    dodosHit++;
                    i.GetComponent<MicroSilenceDodo>().Die();
                }
            }
            if (!cleared && dodosHit >= dodoTarget)
            {
                cleared = true;
                avatars[0].SetExpression(1);
                sfx.PlaySFX(0);
                dodoCount.color = Color.green;
            }
        }
    }
}