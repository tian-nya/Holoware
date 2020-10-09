namespace Micro.Run
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class MicroRun : Microgame
    {
        public SFXManager sfx;
        public MicroRunPlayer player;
        public MicroRunSet[] sets;
        public MicroRunSpawn[] spawnObjects;
        public int spawnAmount;
        public float firstSpawnPosition = 15f, maxSpawnDistance = 50f, spawnIntervalVariation = 2f;
        int playerID, spawnID;
        float spawnX;

        public void Start()
        {
            spawnX = firstSpawnPosition;
            while (spawnX < maxSpawnDistance)
            {
                spawnID = Random.Range(0, spawnObjects.Length);
                Instantiate(spawnObjects[spawnID].spawnObject, new Vector2(spawnX, 0), Quaternion.identity, transform);
                spawnX += spawnObjects[spawnID].intervalToNextSpawn + Random.Range(0, spawnIntervalVariation);
            }
            onStart.AddListener(Game);
        }

        public void Game()
        {
            playerID = Random.Range(0, sets.Length);
            player.gameObject.SetActive(true);
            player.runSprite = sets[playerID].run;
            player.slideSprite = sets[playerID].slide;
            player.failSprite = sets[playerID].fail;
            player.spriteRenderer.sprite = player.runSprite;
            AddAvatar(playerID);
            bgm.PlayBGM(0);
        }

        [System.Serializable]
        public class MicroRunSet
        {
            public Sprite run, slide, fail;
        }

        [System.Serializable]
        public class MicroRunSpawn
        {
            public GameObject spawnObject;
            public float intervalToNextSpawn;
        }
    }
}