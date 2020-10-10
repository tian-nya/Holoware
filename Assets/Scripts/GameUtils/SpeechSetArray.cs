using UnityEngine;

// used in Imitate and Interview
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpeechSetArray", order = 1)]
public class SpeechSetArray : ScriptableObject
{
    public SpeechSet[] sets;

    [System.Serializable]
    public class SpeechSet {
        public GameObject avatar;
        // refer to localization files (Microgames/Interview/Interview_EN & Microgames/Interview/Interview_JP)
        public string name;
        public int numberOfLines;
    }
}