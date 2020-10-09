using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlTypeArea : MonoBehaviour
{
    public Image arrowsIcon, spaceIcon, mouseIcon;
    public Sprite mouseIcon1, mouseIcon2;

    // Start is called before the first frame update
    void Start()
    {
        SetIcons(0);
    }

    public void SetIcons(MicrogameData.MicrogameType type)
    {
        arrowsIcon.color = new Color(1f, 1f, 1f, 0.333f); ;
        spaceIcon.color = new Color(1f, 1f, 1f, 0.333f); ;
        mouseIcon.color = new Color(1f, 1f, 1f, 0.333f); ;
        mouseIcon.sprite = mouseIcon1;

        if (type.HasFlag(MicrogameData.MicrogameType.Arrows))
        {
            arrowsIcon.color = Color.white;
        }
        if (type.HasFlag(MicrogameData.MicrogameType.Space))
        {
            spaceIcon.color = Color.white;
        }
        if (type.HasFlag(MicrogameData.MicrogameType.Mouse))
        {
            mouseIcon.color = Color.white;
            if (type.HasFlag(MicrogameData.MicrogameType.Click))
            {
                mouseIcon.sprite = mouseIcon2;
            }
        }
    }
}
