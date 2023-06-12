using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour {

    public static AchievementManager instance;
    public static Action<Achievement> OnAchievementUnlock;
    [SerializeField] private List<Achievement> achievementList;

    void Awake() {

        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else
            Destroy(gameObject);
    }

    private void Start() {

        AddAchievements(achievementList);
	}

	private void OnEnable() {

        Ship.OnCrash += (stat, amount) => UpdateStatisticsAndAchievements(new Stat(stat, amount));
        Ship.OnDeath += (stat, amount) => UpdateStatisticsAndAchievements(new Stat(stat, amount));
        Portal.OnLevelEnd += (stat, amount) => UpdateStatisticsAndAchievements(new Stat(stat, amount));
        MusicManager.OnTrackEnd += (stat, amount) => UpdateStatisticsAndAchievements(new Stat(stat, amount));
    }

    public void UpdateStatisticsAndAchievements(Stat stat) {

        PlayerData.AddStatistic(stat);
        UpdateAchievements(stat);
        DataManager.SavePlayerData();
    }

    private void UpdateAchievements(Stat newStat) {
        
        foreach (Achievement achievement in PlayerData.achievements) {

            if (achievement.unlocked || achievement.requirement.name != newStat.name)
                continue;

            // Check if the current stat is enough for the achievement requeriment.
            if (PlayerData.statistics[newStat.name].value >= achievement.requirement.value)
                UnlockAchievement(achievement);
        }
    }

    private void AddAchievements(List<Achievement> achievementList) {

        foreach (Achievement achievement in achievementList)
            if (!PlayerData.achievements.Contains(achievement))
                PlayerData.achievements.Add(achievement);
	}

    public void UnlockAchievement(Achievement achievement) {

        achievement.unlocked = true;
        OnAchievementUnlock?.Invoke(achievement);
    }
    public void UnlockAchievement(string id) {

        Achievement achievement = achievementList.Find(x => x.id.Contains(id));

        UnlockAchievement(achievement);
    }
}
