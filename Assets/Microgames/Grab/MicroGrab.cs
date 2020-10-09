namespace Micro.Grab
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MicroGrab : Microgame
    {
        public SFXManager sfx;
        public GameObject[] drops;
        public MicroGrabGoal goal;
        public int dropNumber = 10, targetNumber = 3;
        public Vector2 spawnCorner1, spawnCorner2;
        public GameObject claw;
        public Animator clawAnimator;
        public float clawSpeed = 5f, clawDetectRadius = 0.5f, xBound = 5f, lowerBound = 0f, upperBound = 3f, xBoundRestrictVertical = -1.25f, yBoundRestrictHorizontal = 2f, clickInterval = 0.5f;
        FixedJoint2D joint;
        Vector2 clawPosition;
        float lastClawX, distanceUntilClick;
        int clawState;
        Rigidbody2D clawRb;
        [HideInInspector] public bool fail;
        Collider2D[] hits;
        GameObject pickup;

        void Start()
        {
            joint = claw.GetComponent<FixedJoint2D>();
            clawRb = claw.GetComponent<Rigidbody2D>();
            clawState = 0;
            distanceUntilClick = clickInterval;
            goal.microgame = this;
            for (int i = 0; i < dropNumber; i++)
            {
                if (i < targetNumber)
                {
                    Instantiate(drops[0], new Vector2(Random.Range(spawnCorner1.x + 1f, spawnCorner2.x - 1f), Random.Range(spawnCorner1.y, spawnCorner2.y)),
                                        Quaternion.Euler(new Vector3(0, 0, Random.Range(-180f, 180f))), transform);
                } else
                {
                    Instantiate(drops[Random.Range(1, drops.Length)], new Vector2(Random.Range(spawnCorner1.x, spawnCorner2.x), Random.Range(spawnCorner1.y, spawnCorner2.y)),
                    Quaternion.Euler(new Vector3(0, 0, Random.Range(-180f, 180f))), transform);
                }
            }
            onStart.AddListener(Game);
        }

        public void Game()
        {
            AddAvatar(0);
            bgm.PlayBGM(0);
            StartCoroutine(GameCoroutine());
            StartCoroutine(Claw());
        }

        IEnumerator GameCoroutine()
        {
            while (timer > 0)
            {
                if (Input.GetButtonDown("Space") && clawState == 0)
                {
                    clawState = 1;
                    hits = Physics2D.OverlapCircleAll((Vector2)claw.transform.position + joint.anchor, clawDetectRadius);
                    foreach (Collider2D i in hits)
                    {
                        if (i.tag == "Pickup")
                        {
                            pickup = i.gameObject;
                            break;
                        }
                        if (i.tag == "Hazard")
                        {
                            pickup = i.gameObject;
                        }
                    }
                    if (pickup)
                    {
                        joint.connectedBody = pickup.GetComponent<Rigidbody2D>();
                        sfx.PlaySFX(2);
                    }
                    clawAnimator.SetBool("open", true);
                }
                if (Input.GetButtonUp("Space") && clawState == 1)
                {
                    clawState = -1;
                    joint.connectedBody = null;
                    clawAnimator.SetBool("open", false);
                }
                if (cleared)
                {
                    avatars[0].SetExpression(1);
                    sfx.PlaySFX(0);
                    clawState = -1;
                    joint.connectedBody = null;
                    clawAnimator.SetBool("open", false);
                    break;
                }
                if (fail && !cleared)
                {
                    avatars[0].SetExpression(2);
                    sfx.PlaySFX(1);
                    clawState = -1;
                    joint.connectedBody = null;
                    clawAnimator.SetBool("open", false);
                    fail = false;
                }
                yield return null;
            }
        }

        IEnumerator Claw()
        {
            while (timer > 0 && clawState > -1)
            {
                lastClawX = claw.transform.position.x;
                clawPosition = (Vector2)claw.transform.position + new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * clawSpeed * Time.fixedDeltaTime;
                if (lastClawX < xBoundRestrictVertical)
                {
                    if (clawPosition.x < xBoundRestrictVertical && clawPosition.y < yBoundRestrictHorizontal)
                    {
                        clawPosition = new Vector2(clawPosition.x, yBoundRestrictHorizontal);
                    }
                } else
                {
                    if (clawPosition.x < xBoundRestrictVertical && clawPosition.y < yBoundRestrictHorizontal)
                    {
                        clawPosition = new Vector2(xBoundRestrictVertical, clawPosition.y);
                    }
                }
                clawPosition = new Vector2(Mathf.Clamp(clawPosition.x, -xBound, xBound), Mathf.Clamp(clawPosition.y, lowerBound, upperBound));
                distanceUntilClick -= Mathf.Abs((clawPosition - (Vector2)claw.transform.position).magnitude);
                clawRb.MovePosition(clawPosition);
                if (distanceUntilClick <= 0)
                {
                    sfx.PlaySFX(3);
                    distanceUntilClick = clickInterval;
                }
                yield return new WaitForFixedUpdate();
            }
        }
    }
}