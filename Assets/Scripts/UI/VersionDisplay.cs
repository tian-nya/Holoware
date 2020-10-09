using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class VersionDisplay : MonoBehaviour
{
    Text text;

    void Awake()
    {
        text = GetComponent<Text>();
        text.text = "v" + Application.version;
    }
}
