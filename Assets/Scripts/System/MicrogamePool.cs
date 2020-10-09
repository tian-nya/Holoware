using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MicrogamePool", order = 1)]
public class MicrogamePool : ScriptableObject
{
    public MicrogameData[] pool;
}