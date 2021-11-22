using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class SaveReadWrite
{
    [SerializeField] public static SaveData data = new SaveData();
    private static string filePath = Application.persistentDataPath + "/save.json";

    public static void SaveToJSON()
    {
        string json = JsonUtility.ToJson(data);
        System.IO.File.WriteAllText(filePath, json);
        Debug.Log("Saved data to JSON");
    }

    public static void ReadFromJSON()
    {
        string json = System.IO.File.ReadAllText(filePath);
        data = JsonUtility.FromJson<SaveData>(json);
    }

    public static bool DoesFileExist()
    {
        return System.IO.File.Exists(filePath);
    }
}

[System.Serializable]
public class SaveData
{
    public int previousLoginDate;
}