using System.IO;
using UnityEngine;

public static class PlayerDataManager {

    private static string saveFile = Application.persistentDataPath + "/playerData.json";
    public static PlayerDataList playerData = new PlayerDataList();

    public static void ReadPlayerData() {

        if (File.Exists(saveFile)) {

            string fileContents = File.ReadAllText(saveFile);
            playerData = JsonUtility.FromJson<PlayerDataList>(fileContents);
        }
    }

    public static void WritePlayerData() {

        string jsonString = JsonUtility.ToJson(playerData);
        File.WriteAllText(saveFile, jsonString);
    }
}