namespace Micro.Wear
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MicroWear : Microgame
    {
        public SFXManager sfx;
        public float spawnXBound = 5f, spawnYBound = 3.5f, spawnAvoidRadius, moveSpeed = 5f, turnSpeed = 180f;
        public Transform miko, spawnAvoidSource;
        public Rigidbody2D shadesRb;
        public MicroWearColliders[] shadeColliders;
        public Vector2 shadesEndPosition;
        public ParticleSystem explosion;
        Animator animator;
        bool dragging;
        Vector2 dragOffset, shadesPosition;

        void Start()
        {
            animator = GetComponent<Animator>();
            if (GameSettings.hardMode == 1)
            {
                miko.eulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));
                shadesRb.transform.eulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));
            } else
            {
                miko.eulerAngles = new Vector3(0, 0, Random.Range(-120f, 120f));
            }
            do
            {
                shadesPosition = new Vector2(Random.Range(-spawnXBound, spawnXBound), Random.Range(-spawnYBound, spawnYBound));
            } while ((shadesPosition - (Vector2)spawnAvoidSource.transform.position).magnitude < spawnAvoidRadius);
            shadesRb.transform.position = shadesPosition;
            onStart.AddListener(Game);
        }

        public void Game()
        {
            AddAvatar(0);
            bgm.PlayBGM(0);
            StartCoroutine(GameCoroutine());
            StartCoroutine(DragCoroutine());
        }

        IEnumerator GameCoroutine()
        {
            while (timer > 0)
            {
                // shadesRb.velocity = shadesRb.transform.up * GameManager.gm.input.Directional.Y * moveSpeed;

                if (dragging)
                {
                    shadesPosition = Utils.GetMousePosition() + dragOffset;
                    shadesPosition = new Vector2(Mathf.Clamp(shadesPosition.x, -6f, 6f), Mathf.Clamp(shadesPosition.y, -5f, 5f));
                    shadesRb.MovePosition(shadesPosition);
                } else
                {
                    shadesRb.velocity = Vector2.zero;
                }
                shadesRb.angularVelocity = Input.GetAxisRaw("Horizontal") * -turnSpeed;
                if (shadeColliders[0].touchingGoal && shadeColliders[1].touchingGoal)
                {
                    dragging = false;
                    shadesRb.velocity = Vector2.zero;
                    shadesRb.angularVelocity = 0;
                    shadesRb.isKinematic = true;
                    shadesRb.transform.parent = miko;
                    shadesRb.transform.localPosition = shadesEndPosition;
                    shadesRb.transform.localEulerAngles = Vector3.zero;
                    cleared = true;
                    avatars[0].SetExpression(1);
                    animator.SetTrigger("success");
                    sfx.PlaySFX(0);
                    sfx.PlaySFX(1);
                    explosion.Play();
                    break;
                }
                yield return new WaitForFixedUpdate();
            }
        }

        IEnumerator DragCoroutine()
        {
            while (timer > 0 && !cleared)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.GetMousePosition(), Vector2.zero);
                    if (hits.Length > 0)
                    {
                        foreach (RaycastHit2D i in hits)
                        {
                            if (i.collider.tag == "Player")
                            {
                                dragging = true;
                                dragOffset = (Vector2)i.rigidbody.transform.position - Utils.GetMousePosition();
                                sfx.PlaySFX(2);
                                break;
                            }
                        }
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    dragging = false;
                }
                yield return null;
            }
        }
    }
}