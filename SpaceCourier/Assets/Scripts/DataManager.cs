using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class DataManager {

    static private string filePath;

    static public void InitializePlayerData() {

        filePath = Application.persistentDataPath + "/playerData.dat";

        if (!File.Exists(filePath))
            CreatePlayerData();
        else
            LoadPlayerData();
	}

    static public void CreatePlayerData() {

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(filePath, FileMode.Create);

        binaryFormatter.Serialize(fileStream, PlayerData.progression);
        fileStream.Close();
    }

    static public void SavePlayerData() {

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(filePath, FileMode.Open);

        binaryFormatter.Serialize(fileStream, PlayerData.progression);
        fileStream.Close();
    }

    static public void LoadPlayerData() {

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(filePath, FileMode.Open);

        PlayerData.progression = (int)binaryFormatter.Deserialize(fileStream);
        fileStream.Close();
    }
}