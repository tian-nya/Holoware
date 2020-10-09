namespace Micro.Drumroll
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class MicroDrumroll : Microgame
    {
        public SFXManager sfx;
        public RectTransform gauge1, gauge2, gaugeTarget, gaugeMarker;
        public Image gaugeFrame;
        public float gaugeLength = 720f, target = 0.75f, minPower = 0.05f, maxPower = 0.1f, effectDrain = 0.25f;
        public Animator rushiaAnimator;
        public ParticleSystem hitParticle1, hitParticle2, failParticle;
        public SpriteRenderer armL, armR, leftArrow, rightArrow;
        public Sprite armL1, armL2, armR1, armR2;
        public AnimationCurve effectCurve;
        float progress, lastProgress;
        bool right, reachedTarget;

        void Start()
        {
            gauge1.anchoredPosition = new Vector2(0, gauge1.anchoredPosition.y);
            gauge1.sizeDelta = new Vector2(gaugeLength * target, gauge1.sizeDelta.y);
            gauge2.anchoredPosition = new Vector2(gaugeLength * target, gauge2.anchoredPosition.y);
            gauge2.sizeDelta = new Vector2(gaugeLength * (1 - target), gauge2.sizeDelta.y);
            gaugeTarget.anchoredPosition = new Vector2(gaugeLength * target, gaugeTarget.anchoredPosition.y);
            right = Random.value < 0.5f ? true : false;
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
                progress -= effectDrain * Time.deltaTime;
                if (progress < 0) progress = 0;
                if ((right && Input.GetButtonDown("Left")) || (!right && Input.GetButtonDown("Right")))
                {
                    right = !right;
                    progress += minPower + effectCurve.Evaluate(progress) * (maxPower - minPower);
                    if (progress >= 1)
                    {
                        sfx.PlaySFX(3);
                        rushiaAnimator.SetTrigger("fail");
                    } else
                    {
                        sfx.PlaySFX(2);
                        rushiaAnimator.SetTrigger("hit");
                    }
                    if (right)
                    {
                        hitParticle2.Play();
                    } else
                    {
                        hitParticle1.Play();
                    }
                    progress = Mathf.Clamp01(progress);
                }
                gaugeMarker.anchoredPosition = new Vector2(progress * gaugeLength, gaugeMarker.anchoredPosition.y);
                leftArrow.color = right ? Color.white : Color.white * 0.5f;
                leftArrow.transform.localScale = right ? Vector3.one : Vector3.one * 0.75f;
                rightArrow.color = !right ? Color.white : Color.white * 0.5f;
                rightArrow.transform.localScale = !right ? Vector3.one : Vector3.one * 0.75f;
                armL.sprite = right ? armL1 : armL2;
                armR.sprite = !right ? armR1 : armR2;
                if (progress >= 1)
                {
                    cleared = false;
                    sfx.PlaySFX(1);
                    avatars[0].SetExpression(2);
                    failParticle.Play();
                    leftArrow.color = Color.clear;
                    rightArrow.color = Color.clear;
                    gaugeFrame.color = Color.red;
                    break;
                }
                else if (progress >= target)
                {
                    cleared = true;
                    gaugeFrame.color = Color.green;
                    if (lastProgress < target)
                    {
                        avatars[0].SetExpression(1);
                        if (!reachedTarget)
                        {
                            sfx.PlaySFX(0);
                            reachedTarget = true;
                        }
                    }
                }
                else
                {
                    cleared = false;
                    gaugeFrame.color = Color.black;
                    if (lastProgress >= target)
                    {
                        avatars[0].SetExpression(0);
                    }
                }
                lastProgress = progress;
                yield return null;
            }
        }
    }
}