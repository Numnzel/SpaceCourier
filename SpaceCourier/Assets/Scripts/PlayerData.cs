using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class PlayerData {

    public static int progression;
    public static int progressionExpert;
    public static float optionValue_sound;
    public static float optionValue_music;
    public static float optionValue_mapAlpha;
    public static float optionValue_arrowsAlpha;
    public static float optionValue_uiScale;
    public static bool optionValue_mutePropulsion;
    public static bool optionValue_hideRadio;
    public static Dictionary<int, float> levelTime = new Dictionary<int, float>();
    public static Dictionary<string, Stat> statistics = new Dictionary<string, Stat>();
    public static List<Achievement> achievements = new List<Achievement>();

    public static void SetDefaults() {

        progression = 0;
        progressionExpert = 0;
        optionValue_sound = 0.5f;
        optionValue_music = 0.5f;
        optionValue_mapAlpha = 1.0f;
        optionValue_arrowsAlpha = 1.0f;
        optionValue_uiScale = 1.0f;
        optionValue_mutePropulsion = false;
        optionValue_hideRadio = false;
    }

    public static Stat InitializeStatistic(string name) {

        if (!statistics.ContainsKey(name))
            statistics.Add(name, new Stat(name, 0));

        return statistics[name];
    }

    public static void AddStatistic(Stat stat) {

        InitializeStatistic(stat.name);
        statistics[stat.name].value += stat.value;
    }
}