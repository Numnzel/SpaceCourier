using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class DataManager {

    static private string filePath;

    static public void InitializePlayerData() {

        filePath = Application.persistentDataPath + "/playerData.dat";
	}

    static public void SavePlayerData() {

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Create(filePath);
        
        SerializeAllData(binaryFormatter, fileStream);
        fileStream.Close();
    }

    static public bool LoadPlayerData() {

        if (!File.Exists(filePath))
            return false;

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Open(filePath, FileMode.Open);

        DeserializeAllData(binaryFormatter, fileStream);
        fileStream.Close();
        return true;
    }

    static private void SerializeAllData(BinaryFormatter binaryFormatter, FileStream fileStream) {

        binaryFormatter.Serialize(fileStream, PlayerData.progression);
        binaryFormatter.Serialize(fileStream, PlayerData.optionValue_mapAlpha);
        binaryFormatter.Serialize(fileStream, PlayerData.optionValue_uiScale);
        binaryFormatter.Serialize(fileStream, PlayerData.optionValue_music);
        binaryFormatter.Serialize(fileStream, PlayerData.optionValue_sound);
    }

    static private void DeserializeAllData(BinaryFormatter binaryFormatter, FileStream fileStream) {

        PlayerData.progression = (int)binaryFormatter.Deserialize(fileStream);
        PlayerData.optionValue_mapAlpha = (float)binaryFormatter.Deserialize(fileStream);
        PlayerData.optionValue_uiScale = (float)binaryFormatter.Deserialize(fileStream);
        PlayerData.optionValue_music = (float)binaryFormatter.Deserialize(fileStream);
        PlayerData.optionValue_sound = (float)binaryFormatter.Deserialize(fileStream);
    }
}