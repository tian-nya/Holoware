using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpeechSetArray", order = 1)]
public class SpeechSetArray : ScriptableObject
{
    public SpeechSet[] sets;

    [System.Serializable]
    public class SpeechSet {
        public GameObject avatar;
        public string name;
        public int numberOfLines;
    }
}