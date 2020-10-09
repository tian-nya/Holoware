using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class CharacterAvatar : MonoBehaviour
{
    public AvatarExpression[] expressions;
    public Image image, frame;
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetExpression(int index)
    {
        image.sprite = expressions[index].expression;
        frame.color = expressions[index].frameColor;
        animator.SetTrigger("change");
    }

    [System.Serializable]
    public class AvatarExpression
    {
        public Sprite expression;
        public Color frameColor;
    }
}