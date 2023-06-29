using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameDataManager : MonoBehaviour {

    [SerializeField] static public GameData gameData = new GameData();
    static private string filePath;

    public static GameDataManager instance;

    void Awake() {

        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else
            Destroy(gameObject);

        filePath = Application.persistentDataPath + "/gameData.dat";
    }

    static public void SaveGameData() {

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Create(filePath);
        
        SerializeGameData(binaryFormatter, fileStream);
        fileStream.Close();
    }

    static public bool LoadGameData() {

        if (!File.Exists(filePath))
            return false;

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Open(filePath, FileMode.Open);

        DeserializeGameData(binaryFormatter, fileStream);
        fileStream.Close();
        return true;
    }

    static private void SerializeGameData(BinaryFormatter binaryFormatter, FileStream fileStream) {

        binaryFormatter.Serialize(fileStream, gameData);
    }

    static private void DeserializeGameData(BinaryFormatter binaryFormatter, FileStream fileStream) {

        gameData = (GameData)binaryFormatter.Deserialize(fileStream);
    }
}