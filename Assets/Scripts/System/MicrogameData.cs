using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MicrogameData
{
    public GameObject microgame, hardMicrogame;
    [System.Flags]
    public enum MicrogameType {
        Arrows = (1 << 0),
        Space = (1 << 1),
        Mouse = (1 << 2),
        Click = (1 << 3)
    }
    public MicrogameType type;
}