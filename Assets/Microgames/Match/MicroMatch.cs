namespace Micro.Match
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MicroMatch : Microgame
    {
        public SFXManager sfx;
        public MicroMatchSet[] sets;
        public List<MicroMatchEars> earsObjectPool;
        public List<GameObject> characterObjectPool;
        bool dragging;
        Vector2 dragOffset, objectPosition;
        MicroMatchEars earsInHand;
        int matchesLeft;
        GameObject characterInContact;
        bool characterInContactMatched;

        void Start()
        {
            matchesLeft = characterObjectPool.Count;
            onStart.AddListener(Game);
        }

        public void Game()
        {
            List<int> indexPool = new List<int>();
            for (int i = 0; i < sets.Length; i++) {
                indexPool.Add(i);
            }
            int selection, currentIndex;
            for (int i = 0; i < matchesLeft; i++)
            {
                selection = Random.Range(0, indexPool.Count);
                currentIndex = indexPool[selection];
                indexPool.RemoveAt(selection);

                sets[currentIndex].character = characterObjectPool[i];
                sets[currentIndex].ears = earsObjectPool[Random.Range(0, earsObjectPool.Count)];
                earsObjectPool.Remove(sets[currentIndex].ears);
                sets[currentIndex].character.GetComponentInChildren<SpriteRenderer>().sprite = sets[currentIndex].characterIdle;
                sets[currentIndex].ears.index = currentIndex;
                sets[currentIndex].ears.spriteRenderer.sprite = sets[currentIndex].ears1;
                AddAvatar(currentIndex);
                sets[currentIndex].avatarIndex = i;
            }
            bgm.PlayBGM(0);
            StartCoroutine(GameCoroutine());
            StartCoroutine(DragCoroutine());
        }

        IEnumerator GameCoroutine()
        {
            while (timer > 0)
            {
                if (dragging && earsInHand)
                {
                    objectPosition = Utils.GetMousePosition() + dragOffset;
                    objectPosition = new Vector2(Mathf.Clamp(objectPosition.x, -6f, 6f), Mathf.Clamp(objectPosition.y, -9f, 0f));
                    earsInHand.rb.MovePosition(objectPosition);
                }
                if (matchesLeft == 0)
                {
                    cleared = true;
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
                            if (i.collider.tag == "Pickup")
                            {
                                if (!sets[i.collider.GetComponent<MicroMatchEars>().index].earsMatched)
                                {
                                    dragging = true;
                                    dragOffset = (Vector2)i.rigidbody.transform.position - Utils.GetMousePosition();
                                    earsInHand = i.collider.GetComponent<MicroMatchEars>();
                                    sfx.PlaySFX(3);
                                    break;
                                }
                            }
                        }
                    }
                }
                if (Input.GetMouseButtonUp(0) && dragging)
                {
                    characterInContact = null;
                    characterInContactMatched = false;
                    dragging = false;
                    foreach (Collider2D i in earsInHand.inContact)
                    {
                        if (!characterInContact)
                        {
                            characterInContact = i.gameObject;
                        } else
                        {
                            if ((i.transform.position - earsInHand.transform.position).magnitude < (characterInContact.transform.position - earsInHand.transform.position).magnitude)
                            {
                                characterInContact = i.gameObject;
                            }
                        }
                    }

                    if (characterInContact)
                    {
                        for (int i = 0; i < sets.Length; i++)
                        {
                            if (sets[i].character == characterInContact && sets[i].characterMatched)
                            {
                                characterInContactMatched = true;
                            }
                        }

                        if (!characterInContactMatched) {
                            earsInHand.transform.SetParent(characterInContact.GetComponentInChildren<SpriteRenderer>().transform.parent);
                            earsInHand.transform.localPosition = Vector2.zero;
                            sets[earsInHand.index].earsMatched = true;
                            if (sets[earsInHand.index].character == characterInContact)
                            {
                                matchesLeft--;
                                sets[earsInHand.index].characterMatched = true;
                                earsInHand.spriteRenderer.color = Color.clear;
                                characterInContact.GetComponentInChildren<SpriteRenderer>().sprite = sets[earsInHand.index].characterWin;
                                characterInContact.GetComponent<Animator>().SetTrigger("win");
                                sfx.PlaySFX(matchesLeft == 0 ? 0 : 2);
                                avatars[sets[earsInHand.index].avatarIndex].SetExpression(1);
                            } else
                            {
                                earsInHand.spriteRenderer.sprite = sets[earsInHand.index].ears2;
                                for (int i = 0; i < sets.Length; i++)
                                {
                                    if (sets[i].character == characterInContact)
                                    {
                                        sets[i].characterMatched = true;
                                        characterInContact.GetComponentInChildren<SpriteRenderer>().sprite = sets[i].characterFail;
                                        avatars[sets[i].avatarIndex].SetExpression(2);
                                        break;
                                    }
                                }
                                characterInContact.GetComponent<Animator>().SetTrigger("fail");
                                sfx.PlaySFX(1);
                            }
                        }
                    }

                    earsInHand = null;
                }
                yield return null;
            }
        }

        [System.Serializable]
        public class MicroMatchSet
        {
            public Sprite characterIdle, characterWin, characterFail, ears1, ears2;
            [HideInInspector] public MicroMatchEars ears;
            [HideInInspector] public GameObject character;
            [HideInInspector] public bool earsMatched, characterMatched;
            [HideInInspector] public int avatarIndex;
        }
    }
}