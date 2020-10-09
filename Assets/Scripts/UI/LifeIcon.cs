using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LifeIcon : MonoBehaviour
{
    Image icon;

    void Awake()
    {
        icon = GetComponent<Image>();
    }

    public void RemoveLife()
    {
        icon.color = Color.clear;
    }

    public void RestoreLife()
    {
        icon.color = Color.white;
    }
}
