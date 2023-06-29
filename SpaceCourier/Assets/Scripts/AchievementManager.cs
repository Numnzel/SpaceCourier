using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour {

    public static AchievementManager instance;
    public static Action<Achievement> OnAchievementUnlock;
    public List<Achievement> achievementList;

    void Awake() {

        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else
            Destroy(gameObject);
    }

	private void OnEnable() {

        Ship.OnCrash += (stat, amount) => UpdateStatisticsAndAchievements(new Stat(stat, amount));
        Ship.OnDeath += (stat, amount) => UpdateStatisticsAndAchievements(new Stat(stat, amount));
        Portal.OnLevelEnd += (stat, amount) => UpdateStatisticsAndAchievements(new Stat(stat, amount));
        MusicManager.OnTrackEnd += (stat, amount) => UpdateStatisticsAndAchievements(new Stat(stat, amount));
    }

    public void UpdateStatisticsAndAchievements(Stat stat) {

        GameDataManager.gameData.AddStatistic(stat);
        UpdateAchievements(stat);
        GameDataManager.SaveGameData();
    }

    private void UpdateAchievements(Stat newStat) {
        
        foreach (Achievement achievement in achievementList) {

            // Abort if we already have that achievement or the updated stat has no relation to it
            if (GameDataManager.gameData.achievements.Contains(achievement.id) || achievement.requirement.name != newStat.name)
                continue;

            // Check if the current stat is enough for the achievement requeriment.
            if (GameDataManager.gameData.statistics[newStat.name].value >= achievement.requirement.value)
                UnlockAchievement(achievement);
        }
    }

    public void UnlockAchievement(Achievement achievement) {

        GameDataManager.gameData.achievements.Add(achievement.id);
        OnAchievementUnlock?.Invoke(achievement);
    }
}
