namespace Micro.Draw
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MicroDraw : Microgame
    {
        public SFXManager sfx;
        public AudioSource pencilSfx;
        public float nodeSize = 1f, minStrokeLength = 0.1f, minNodeCoverage = 0.9f, minLineAccuracy = 0.75f, pencilSfxMinTime = 0.2f;
        public GameObject[] guides;
        public GameObject stroke;
        public Transform cursor;
        [HideInInspector] public int nodeAmount, nodesCovered;
        int vertAmount, accurateVertAmount;

        void Start()
        {
            MicroDrawGuide guide = Instantiate(guides[Random.Range(0, guides.Length)], transform).GetComponent<MicroDrawGuide>();
            if (GameSettings.hardMode == 1) guide.transform.eulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));
            guide.microgame = this;
            guide.NodePrep();
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
                if (Input.GetMouseButtonDown(0))
                {
                    StartCoroutine(DrawCoroutine());
                }
                cursor.position = Utils.GetMousePosition();
                yield return null;
            }
        }

        IEnumerator DrawCoroutine()
        {
            cursor.localScale = Vector3.one * 0.75f;
            sfx.PlaySFX(1);
            LineRenderer lineRenderer = Instantiate(stroke, transform).GetComponent<LineRenderer>();
            bool accurateVert;
            float pencilSfxTimer = pencilSfxMinTime;

            lineRenderer.SetPosition(0, Utils.GetMousePosition());
            RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.GetMousePosition(), Vector2.zero);
            vertAmount++;
            accurateVert = false;
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.tag == "Goal")
                {
                    accurateVert = true;
                    hits[i].collider.GetComponent<MicroDrawNode>().CoverNode();
                }
            }
            if (accurateVert) accurateVertAmount++;
            Vector2 lastPosition = Utils.GetMousePosition();
            CheckAccuracy();

            while (timer > 0)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    cursor.localScale = Vector3.one;
                    pencilSfx.volume = 0;
                    break;
                }
                if ((Utils.GetMousePosition() - lastPosition).magnitude >= minStrokeLength)
                {
                    pencilSfx.volume = GameSettings.sfxVolume;
                    pencilSfxTimer = pencilSfxMinTime;
                    lineRenderer.positionCount++;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, Utils.GetMousePosition());
                    hits = Physics2D.RaycastAll(Utils.GetMousePosition(), Vector2.zero);
                    vertAmount++;
                    accurateVert = false;
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].collider.tag == "Goal")
                        {
                            accurateVert = true;
                            hits[i].collider.GetComponent<MicroDrawNode>().CoverNode();
                        }
                    }
                    if (accurateVert) accurateVertAmount++;
                    lastPosition = Utils.GetMousePosition();
                    CheckAccuracy();
                } else if (pencilSfxTimer > 0)
                {
                    pencilSfxTimer -= Time.deltaTime;
                    if (pencilSfxTimer <= 0) pencilSfx.volume = 0;
                }
                yield return null;
            }
        }

        void CheckAccuracy()
        {
            if (cleared) return;
            if ((float)nodesCovered / nodeAmount >= minNodeCoverage && (float)accurateVertAmount / vertAmount >= minLineAccuracy)
            {
                cleared = true;
                avatars[0].SetExpression(1);
                sfx.PlaySFX(0);
            }
        }
    }
}