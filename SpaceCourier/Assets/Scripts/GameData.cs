using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData {

    public Dictionary<string, int> progression = new Dictionary<string, int>();
    public Dictionary<string, Stat> statistics = new Dictionary<string, Stat>();
    public List<string> achievements = new List<string>();
    public List<LevelData> levelData = new List<LevelData>();

    public Stat InitializeStatistic(string name) {

        if (!statistics.ContainsKey(name))
            statistics.Add(name, new Stat(name, 0));

        return statistics[name];
    }

    public void AddStatistic(Stat stat) {

        InitializeStatistic(stat.name);
        statistics[stat.name].value += stat.value;
    }
}