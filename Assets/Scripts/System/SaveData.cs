using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public Save loadedSave;
    [HideInInspector] public bool loaded;

    public void Awake()
    {
        LoadFile();
    }

    [System.Serializable]
    public class Save
    {
        public int coins, highScore, highScoreHard, currentSkin;
        public List<string> ownedSkins;
        public string saveVersion;

        public Save()
        {
            coins = 0;
            highScore = 0;
            highScoreHard = 0;
            ownedSkins = new List<string>();
            ownedSkins.Add("Default");
            currentSkin = 0;
        }
    }

    public void SaveFile()
    {
        System.IO.File.WriteAllText(Application.persistentDataPath + "/saveData.json", JsonUtility.ToJson(loadedSave));
    }

    public void LoadFile()
    {
        loaded = false;
        loadedSave = new Save();
        if (!System.IO.File.Exists(Application.persistentDataPath + "/saveData.json"))
        {
            SaveFile();
        }
        loadedSave = JsonUtility.FromJson<Save>(System.IO.File.ReadAllText(Application.persistentDataPath + "/saveData.json"));
        if (loadedSave.saveVersion != Application.version)
        {
            loadedSave.saveVersion = Application.version;
            loadedSave.highScore = 0;
            loadedSave.highScoreHard = 0;
        }
        loaded = true;
    }
}
