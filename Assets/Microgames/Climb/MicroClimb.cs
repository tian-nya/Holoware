namespace Micro.Climb
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class MicroClimb : Microgame
    {
        public SFXManager sfx;
        public GameObject player;

        public void Start()
        {
            onStart.AddListener(Game);
        }

        public void Game()
        {
            player.SetActive(true);
            AddAvatar(0);
            bgm.PlayBGM(0);
        }
    }
}