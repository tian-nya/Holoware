namespace Micro.Whack
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class MicroWhack : Microgame
    {
        public SFXManager sfx;
        public GameObject hammer, crosshair, targetObject;
        public GameObject[] hazardObjects;
        public Animator hammerAnimator;
        public int objectsPerCycle = 3, targetsPerCycle = 1, numberOfCycles = 7, targetGoal = 5;
        public float cycleInterval = 0.8f;
        public Vector2 hitSize;
        public ParticleSystem hitParticle;
        public Text hitCount;
        public Transform[] holes;
        [HideInInspector] public MicroWhackObject[] activeObjects;
        int targetsHit;
        bool canMove;
        List<int> availableHoles;

        public void Start()
        {
            activeObjects = new MicroWhackObject[holes.Length];
            availableHoles = new List<int>();
            canMove = true;
            onStart.AddListener(Game);
        }

        public void Game()
        {
            AddAvatar(0);
            bgm.PlayBGM(0);
            StartCoroutine(GameCoroutine());
            StartCoroutine(SpawnCoroutine());
        }

        IEnumerator GameCoroutine()
        {
            while (timer > 0) {
                if (!canMove)
                {
                    cleared = false;
                    sfx.PlaySFX(1);
                    avatars[0].SetExpression(2);
                    crosshair.SetActive(false);
                    hitCount.color = Color.red;
                    break;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    Hit(hammer.transform.position);
                }
                hitCount.text = targetsHit.ToString() + "/" + targetGoal.ToString();
                hammer.transform.position = Utils.GetMousePosition();
                yield return null;
            }
        }

        IEnumerator SpawnCoroutine()
        {
            while (numberOfCycles > 0)
            {
                yield return new WaitForSeconds(cycleInterval);
                SpawnObjects();
                numberOfCycles--;
            }
        }

        void SpawnObjects()
        {
            int index;
            MicroWhackObject spawnedObject;
            FindAvailableHoles();
            int spawnCount = availableHoles.Count < objectsPerCycle ? availableHoles.Count : objectsPerCycle;
            for (int i = 0; i < spawnCount; i++)
            {
                index = Random.Range(0, availableHoles.Count);
                if (i < targetsPerCycle)
                {
                    spawnedObject = Instantiate(targetObject, holes[availableHoles[index]].position, Quaternion.identity, transform).GetComponent<MicroWhackObject>();
                } else
                {
                    spawnedObject = Instantiate(hazardObjects[Random.Range(0, hazardObjects.Length)], holes[availableHoles[index]].position, Quaternion.identity, transform).GetComponent<MicroWhackObject>();
                }
                spawnedObject.microgame = this;
                activeObjects[availableHoles[index]] = spawnedObject;
                availableHoles.RemoveAt(index);
            }
        }

        void Hit(Vector2 position)
        {
            Collider2D[] hits;
            bool hitObject = false;
            hammerAnimator.SetTrigger("hit");
            hits = Physics2D.OverlapBoxAll(position, hitSize, 0f);
            foreach (Collider2D i in hits)
            {
                if (i.tag == "Pickup")
                {
                    targetsHit++;
                    hitObject = true;
                    i.GetComponent<MicroWhackObject>().Hit();
                }
                if (i.tag == "Hazard")
                {
                    canMove = false;
                    hitObject = true;
                    i.GetComponent<MicroWhackObject>().Hit();
                }
            }
            sfx.PlaySFX(3);
            if (hitObject) sfx.PlaySFX(4);
            hitParticle.Play();
            if (!cleared && targetsHit >= targetGoal && canMove)
            {
                cleared = true;
                avatars[0].SetExpression(1);
                sfx.PlaySFX(0);
                hitCount.color = Color.green;
            } else if (hitObject && canMove)
            {
                sfx.PlaySFX(2);
            }
        }

        void FindAvailableHoles()
        {
            availableHoles.Clear();
            for (int i = 0; i < activeObjects.Length; i++)
            {
                if (!activeObjects[i])
                {
                    availableHoles.Add(i);
                }
            }
        }
    }
}