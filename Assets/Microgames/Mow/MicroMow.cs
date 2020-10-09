namespace Micro.Mow
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class MicroMow : Microgame
    {
        public SFXManager sfx;
        public GameObject grass;
        public Transform player;
        public Rigidbody2D playerRb;
        public Animator playerAnimator;
        public int grassPatches = 2, grassPerPatch = 10;
        public float xBound = 6f, yBound = 4.5f, spawnXBound = 4.5f, spawnYBound = 3.5f, spawnRadius = 2f, playerSpeed = 6f, turnTime = 0.2f;
        public AnimationCurve turnCurve;
        [HideInInspector] public int grassAmount;
        Vector2 playerPosition, moveVector;
        Coroutine turn;
        float currentDirection;

        public void Start()
        {
            GrassSpawn();
            playerPosition = player.position;
            onStart.AddListener(Game);
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
                if (grassAmount == 0 && !cleared)
                {
                    cleared = true;
                    avatars[0].SetExpression(1);
                    sfx.PlaySFX(0);
                }
                moveVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                playerAnimator.SetFloat("speed", moveVector.magnitude);
                if (moveVector.magnitude > 0)
                {
                    if (Mathf.Sign(moveVector.x) != currentDirection)
                    {
                        currentDirection = Mathf.Sign(moveVector.x);
                        if (turn != null) StopCoroutine(turn);
                        turn = StartCoroutine(TurnCoroutine(Mathf.Sign(moveVector.x)));
                    }
                    playerPosition += Vector2.ClampMagnitude(moveVector, 1f) * playerSpeed * Time.fixedDeltaTime;
                    playerPosition = new Vector2(Mathf.Clamp(playerPosition.x, -xBound, xBound), Mathf.Clamp(playerPosition.y, -yBound, yBound));
                    playerRb.MovePosition(playerPosition);
                }
                yield return new WaitForFixedUpdate();
            }
        }

        IEnumerator TurnCoroutine(float direction)
        {
            float startX = player.localScale.x;
            float endX = direction;
            float timer = 0;
            while (timer < turnTime)
            {
                timer += Time.deltaTime;
                player.localScale = new Vector3(Mathf.Lerp(startX, endX, turnCurve.Evaluate(timer / turnTime)), 1f, 1f);
                yield return null;
            }
            player.localScale = new Vector3(endX, 1f, 1f);
        }

        void GrassSpawn()
        {
            MicroMowGrass spawnedGrass;
            Vector2 patchPosition, spawnPosition;
            for (int i = 0; i < grassPatches; i++)
            {
                patchPosition = new Vector2(Random.Range(-spawnXBound, spawnXBound), Random.Range(-spawnYBound, spawnYBound));
                for (int j = 0; j < grassPerPatch; j++)
                {
                    spawnPosition = patchPosition + Random.insideUnitCircle * spawnRadius;
                    spawnPosition = new Vector2(Mathf.Clamp(spawnPosition.x, -xBound, xBound), Mathf.Clamp(spawnPosition.y, -yBound, yBound));
                    spawnedGrass = Instantiate(grass, spawnPosition, Quaternion.identity, transform).GetComponent<MicroMowGrass>();
                    spawnedGrass.microgame = this;
                    grassAmount++;
                }
            }
        }
    }
}