namespace Micro.Eat
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MicroEat : Microgame
    {
        public MicroEatPlayer player;
        public MicroEatSet[] sets;
        public GameObject bombDrop;
        public float dropInterval = 0.1f, foodSpeed = 3f, hazardSpeed = 5f;
        public int hazardInterval = 10;
        int playerID, dropsSpawned;

        void Start()
        {
            onStart.AddListener(Game);
        }

        public void Game()
        {
            playerID = Random.Range(0, sets.Length);
            player.spriteRenderer.sprite = sets[playerID].run;
            player.failSprite = sets[playerID].fail;
            AddAvatar(playerID);
            bgm.PlayBGM(playerID);
            StartCoroutine(DropFood());
        }

        IEnumerator DropFood()
        {
            Rigidbody2D food;

            while (timer > 0)
            {
                dropsSpawned++;
                if (dropsSpawned % hazardInterval != 0)
                {
                    food = Instantiate(sets[playerID].drop, new Vector2(Random.Range(-6f, 6f), 6f), Quaternion.identity, transform).GetComponent<Rigidbody2D>();
                    food.velocity = new Vector2(0, -foodSpeed);
                    food.angularVelocity = Random.Range(-360f, 360f);
                } else
                {
                    food = Instantiate(bombDrop, new Vector2(Random.Range(-6f, 6f), 6f), Quaternion.identity, transform).GetComponent<Rigidbody2D>();
                    food.velocity = new Vector2(0, -hazardSpeed);
                    food.angularVelocity = Random.Range(-360f, 360f);
                }
                yield return new WaitForSeconds(dropInterval);
            }
        }

        [System.Serializable]
        public class MicroEatSet {
            public Sprite run, fail;
            public GameObject drop;
        }
    }
}