namespace Micro.Make
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MicroMake : Microgame
    {
        public SFXManager sfx;
        public Animator fubukiAnimator;
        public Transform guideAnchor, anchor;
        public MicroMakePart[] parts;
        [Range(3, 10)]
        public int numberOfParts = 5, guideBaseOrder = -90, targetBaseOrder = -30, dragOrder = -10;
        bool dragging;
        GameObject partClicked;
        MicroMakePart partInHand;
        List<int> guideCombo, combo;
        int specialBun;
        float guideHeight, height;
        bool anchorInContact, failed;

        void Start()
        {
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i].id = i;
            }
            guideCombo = new List<int>();
            combo = new List<int>();
            onStart.AddListener(Game);
        }

        public void Game()
        {
            if (Random.value < 0.1f)
            {
                specialBun = Random.Range(2, 4);
            }
            for (int i = 0; i < numberOfParts; i++)
            {
                if (i == 0) AddGuidePart(specialBun > 0 ? specialBun : 0);
                else if (i == 1) AddGuidePart(specialBun > 0 ? Random.Range(4, parts.Length) : Random.Range(2, 4));
                else if (i == numberOfParts - 1) AddGuidePart(specialBun > 0 ? specialBun : 1);
                else AddGuidePart(specialBun > 0 ? Random.Range(4, parts.Length) : Random.Range(2, parts.Length));
            }
            AddAvatar(0);
            bgm.PlayBGM(0);
            StartCoroutine(GameCoroutine());
            StartCoroutine(DragCoroutine());
        }

        IEnumerator GameCoroutine()
        {
            while (timer > 0)
            {
                if (dragging)
                {
                    partInHand.transform.position = Utils.GetMousePosition();
                }
                if (cleared || failed)
                {
                    break;
                }
                yield return null;
            }
        }

        IEnumerator DragCoroutine()
        {
            while (timer > 0 && !cleared)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    partClicked = null;
                    RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.GetMousePosition(), Vector2.zero);
                    if (hits.Length > 0)
                    {
                        foreach (RaycastHit2D i in hits)
                        {
                            if (i.collider.tag == "Pickup")
                            {
                                if (!partClicked)
                                {
                                    partClicked = i.collider.gameObject;
                                } else
                                {
                                    if (((Vector2)i.collider.transform.position - Utils.GetMousePosition()).magnitude < ((Vector2)partClicked.transform.position - Utils.GetMousePosition()).magnitude)
                                    {
                                        partClicked = i.collider.gameObject;
                                    }
                                }
                            }
                        }
                        if (partClicked)
                        {
                            dragging = true;
                            partInHand = Instantiate(partClicked, Utils.GetMousePosition(), Quaternion.identity, transform).GetComponent<MicroMakePart>();
                            partInHand.gameObject.tag = "Untagged";
                            partInHand.transform.localScale = Vector3.one;
                            partInHand.sr.sortingOrder = dragOrder;
                            sfx.PlaySFX(3);
                        }
                    }
                }
                if (Input.GetMouseButtonUp(0) && dragging)
                {
                    dragging = false;
                    anchorInContact = false;
                    RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.GetMousePosition(), Vector2.zero);
                    foreach (RaycastHit2D i in hits)
                    {
                        if (i.collider.tag == "Goal")
                        {
                            anchorInContact = true;
                            break;
                        }
                    }
                    if (anchorInContact)
                    {
                        partInHand.transform.SetParent(anchor);
                        partInHand.transform.localPosition = new Vector2(0, height);
                        height += partInHand.height;
                        partInHand.sr.sortingOrder = targetBaseOrder + combo.Count;
                        combo.Add(partInHand.id);

                        if (combo[combo.Count - 1] == guideCombo[combo.Count - 1])
                        {
                            if (combo.Count == guideCombo.Count)
                            {
                                cleared = true;
                                sfx.PlaySFX(0);
                                fubukiAnimator.SetTrigger("win");
                                avatars[0].SetExpression(1);
                            } else
                            {
                                sfx.PlaySFX(2);
                            }
                        } else
                        {
                            failed = true;
                            sfx.PlaySFX(1);
                            fubukiAnimator.SetTrigger("fail");
                            avatars[0].SetExpression(2);
                            break;
                        }
                    } else
                    {
                        Destroy(partInHand.gameObject);
                    }
                    partInHand = null;
                }
                yield return null;
            }
        }

        public void AddGuidePart(int id)
        {
            MicroMakePart part = Instantiate(parts[id], guideAnchor);
            part.gameObject.tag = "Untagged";
            part.transform.localScale = Vector3.one;
            part.transform.localPosition = new Vector2(0, guideHeight);
            guideHeight += part.height;
            part.sr.sortingOrder = guideBaseOrder + guideCombo.Count;
            guideCombo.Add(part.id);
        }
    }
}