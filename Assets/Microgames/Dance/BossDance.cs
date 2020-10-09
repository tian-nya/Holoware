namespace Micro.Dance
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using Lean.Localization;

    public class BossDance : Microgame
    {
        public SFXManager sfx;
        public TextMeshProUGUI trackName, comboDisp;
        public Image healthBar;
        public RectTransform[] columns;
        public float delay = 4f, inputRegisterWindow = 0.3f, inputAllowance = 0.15f, spawnY = -640, travelTime = 3f, missDamage = 0.2f, regen = 0.02f;
        public GameObject[] arrows;
        public GameObject okFX, missFX, fullComboFX;
        public Animator marineAnimator, matsuriAnimator;
        public Track[] tracks;
        Animator[] arrowAnimators;
        Track track;
        string mapString;
        public List<Note> notes, activeNotes;
        float health, failFXTimer, matsuriNextTime;
        double startTime, endTime;
        int nextNoteSpawn, matsuriNextNote, combo;
        bool fullCombo;

        public void Start()
        {
            arrowAnimators = new Animator[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                arrowAnimators[i] = columns[i].GetComponent<Animator>();
            }

            track = tracks[Random.Range(0, tracks.Length)];
            trackName.text = LeanLocalization.GetTranslationText("Dance/" + track.name);
            health = 1;
            notes = new List<Note>();
            activeNotes = new List<Note>();
            fullCombo = true;
            inputRegisterWindow /= Time.timeScale;
            inputAllowance /= Time.timeScale;
            travelTime /= Time.timeScale;
            marineAnimator.speed = track.bpm / 60f;
            matsuriAnimator.speed = track.bpm / 60f;
            onStart.AddListener(Game);
        }

        public void Game()
        {
            AddAvatar(0);
            AddAvatar(1);
            StartCoroutine(GameCoroutine());
        }

        IEnumerator GameCoroutine()
        {
            // schedule audio
            startTime = AudioSettings.dspTime + delay;
            bgm.audioSource.clip = track.audio;
            endTime = startTime + (track.audio.length / Time.timeScale);
            bgm.audioSource.PlayScheduled(startTime);
            sfx.PlaySFX(4);

            // parse map string
            mapString = System.Text.RegularExpressions.Regex.Replace(track.map.text, @"\s+", "");
            string genString;
            float genBeat = 0;
            Note genNote;
            do
            {
                genNote = new Note();

                genString = mapString.Remove(mapString.IndexOf(','));
                mapString = mapString.Substring(mapString.IndexOf(',') + 1);
                genNote.hitTime = startTime + genBeat * (60f / track.bpm / Time.timeScale);
                genNote.spawnTime = genNote.hitTime - travelTime;
                genBeat += float.Parse(genString);

                genString = mapString.IndexOf(',') > -1 ? mapString.Remove(mapString.IndexOf(',')) : mapString;
                mapString = mapString.IndexOf(',') > -1 ? mapString.Substring(mapString.IndexOf(',') + 1) : "";
                if (int.Parse(genString) > 1)
                {
                    genNote.multi = true;
                    List<int> indexPool = Utils.GenerateIndexPool(4);
                    genNote.noteIndices = Utils.RandomFromIntPool(indexPool, int.Parse(genString));
                } else if (int.Parse(genString) == 1)
                {
                    genNote.noteIndices.Add(Random.Range(0, 4));
                }

                if (genNote.noteIndices.Count > 0) notes.Add(genNote);
            } while (mapString.IndexOf(',') > -1);
            matsuriNextTime = (float)notes[matsuriNextNote].hitTime;

            // manage gameplay/notes
            while (AudioSettings.dspTime < endTime)
            {
                // note spawn
                if (nextNoteSpawn < notes.Count)
                {
                    if (notes[nextNoteSpawn].spawnTime <= AudioSettings.dspTime)
                    {
                        activeNotes.Add(notes[nextNoteSpawn].DeepCopy());
                        for (int i = 0; i < activeNotes[activeNotes.Count - 1].noteIndices.Count; i++)
                        {
                            activeNotes[activeNotes.Count - 1].noteTransforms.Add(Instantiate(arrows[activeNotes[activeNotes.Count - 1].noteIndices[i]],
                                columns[activeNotes[activeNotes.Count - 1].noteIndices[i]]).GetComponent<RectTransform>());
                        }
                        nextNoteSpawn++;
                    }
                }

                // note move
                for (int i = 0; i < activeNotes.Count; i++)
                {
                    for (int j = 0; j < activeNotes[i].noteTransforms.Count; j++)
                    {
                        activeNotes[i].noteTransforms[j].anchoredPosition = new Vector2(0, (float)(activeNotes[i].hitTime - AudioSettings.dspTime) / travelTime * spawnY);
                    }
                }

                // Matsuri animation
                if (matsuriNextNote < notes.Count && AudioSettings.dspTime >= matsuriNextTime)
                {
                    if (notes[matsuriNextNote].noteIndices.Count > 0)
                    {
                        matsuriAnimator.SetInteger("poseID", notes[matsuriNextNote].multi ? 4 : notes[matsuriNextNote].noteIndices[0]);
                        matsuriAnimator.SetTrigger("pose");
                    }
                    matsuriNextNote++;
                    if (matsuriNextNote < notes.Count) matsuriNextTime = (float)notes[matsuriNextNote].hitTime;
                }

                // inputs
                if (Input.GetButtonDown("Left")) CheckInput(0);
                if (Input.GetButtonDown("Up")) CheckInput(1);
                if (Input.GetButtonDown("Down")) CheckInput(2);
                if (Input.GetButtonDown("Right")) CheckInput(3);

                // late note miss
                if (activeNotes.Count > 0)
                {
                    if (AudioSettings.dspTime > activeNotes[0].hitTime + inputAllowance / 2)
                    {
                        sfx.PlaySFX(1);
                        fullCombo = false;
                        combo = 0;
                        comboDisp.text = combo.ToString();
                        health -= missDamage;
                        for (int i = 0; i < activeNotes[0].noteIndices.Count; i++)
                        {
                            Instantiate(missFX, columns[activeNotes[0].noteIndices[i]]);
                            Destroy(activeNotes[0].noteTransforms[i].gameObject);
                        }
                        activeNotes.RemoveAt(0);
                    }
                }

                // health check
                healthBar.fillAmount = health;
                if (health <= 0)
                {
                    break;
                }
                yield return null;
            }
            marineAnimator.speed = 1;
            matsuriAnimator.speed = 1;
            if (health <= 0)
            {
                marineAnimator.SetTrigger("fail");
                matsuriAnimator.SetTrigger("fail");
                avatars[0].SetExpression(2);
                avatars[1].SetExpression(2);
                failFXTimer = 3f;
                while (failFXTimer > 0)
                {
                    bgm.audioSource.pitch = Mathf.Clamp(bgm.audioSource.pitch - 0.333f * Time.deltaTime, 0f, Mathf.Infinity);
                    failFXTimer -= Time.deltaTime;
                    yield return null;
                }
            } else
            {
                cleared = true;
                marineAnimator.SetTrigger("win");
                matsuriAnimator.SetTrigger("win");
                avatars[0].SetExpression(1);
                avatars[1].SetExpression(1);
                sfx.PlaySFX(0);
                sfx.PlaySFX(5);
                if (fullCombo) fullComboFX.SetActive(true);
                yield return new WaitForSeconds(2f);
            }
            timeOver = true;
        }

        [System.Serializable]
        public class Track
        {
            public string name;
            public AudioClip audio;
            public float bpm;
            public TextAsset map;
        }

        [System.Serializable]
        public class Note
        {
            public double spawnTime, hitTime;
            public List<int> noteIndices;
            public List<RectTransform> noteTransforms;
            public bool multi;

            public Note()
            {
                noteIndices = new List<int>();
                noteTransforms = new List<RectTransform>();
            }

            public Note DeepCopy()
            {
                Note copy = (Note)this.MemberwiseClone();
                copy.noteIndices = new List<int>();
                for (int i = 0; i < noteIndices.Count; i++)
                {
                    copy.noteIndices.Add(noteIndices[i]);
                }
                for (int i = 0; i < noteTransforms.Count; i++)
                {
                    copy.noteTransforms.Add(noteTransforms[i]);
                }
                return copy;
            }
        }

        void CheckInput(int index)
        {
            arrowAnimators[index].SetTrigger("hit");
            if (activeNotes.Count == 0) return;
            if (AudioSettings.dspTime <= activeNotes[0].hitTime - inputRegisterWindow) return;
            int indexInList = activeNotes[0].noteIndices.IndexOf(index);
            if (indexInList > -1 && Mathf.Abs((float)(AudioSettings.dspTime - activeNotes[0].hitTime)) <= inputAllowance)
            {
                combo++;
                comboDisp.text = combo.ToString();
                health += regen;
                if (health > 1) health = 1;
                Instantiate(okFX, columns[activeNotes[0].noteIndices[indexInList]]);
                Destroy(activeNotes[0].noteTransforms[indexInList].gameObject);
                activeNotes[0].noteIndices.RemoveAt(indexInList);
                activeNotes[0].noteTransforms.RemoveAt(indexInList);
                if (activeNotes[0].noteIndices.Count == 0) {
                    marineAnimator.SetInteger("poseID", activeNotes[0].multi ? 4 : index);
                    marineAnimator.SetTrigger("pose");
                    sfx.PlaySFX(activeNotes[0].multi ? 3 : 2);
                    activeNotes.RemoveAt(0);
                }
            } else
            {
                marineAnimator.SetInteger("poseID", index);
                marineAnimator.SetTrigger("pose");

                sfx.PlaySFX(1);
                fullCombo = false;
                combo = 0;
                comboDisp.text = combo.ToString();
                health -= missDamage;
                for (int i = 0; i < activeNotes[0].noteIndices.Count; i++)
                {
                    Instantiate(missFX, columns[activeNotes[0].noteIndices[i]]);
                    Destroy(activeNotes[0].noteTransforms[i].gameObject);
                }
                activeNotes.RemoveAt(0);
            }
        }
    }
}