using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class Skin : MonoBehaviour
{
    public SFXManager sfx;
    /* SFX IDs
     * 0 = win
     * 1 = fail
     * 2 = tick
     */
    public BGMManager bgm;
    /* BGM IDs
     * 0 = prep
     * 1 = win
     * 2 = fail
     * 3 = speed up
     * 4 = boss
     * 5 = game over
     */
    public Animator animator;
    /* Animator triggers
     * microPrep
     * microStart
     * microWin
     * microFail
     * speedUp
     * boss
     * gameOver
     */

    [Header("UI")]
    public TextMeshProUGUI actionVerb;
    public TextMeshProUGUI points;
    public LifeIcon[] lifeIcons;
    public ControlTypeArea controlType;
    public GameObject gameTimer;

    [Header("Events")]
    public UnityEvent gameStart;
    public UnityEvent microPrep, microStart, microWin, microFail, speedUp, boss, gameOver;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected virtual void OnEnable()
    {
        gameStart.Invoke();
    }
}
