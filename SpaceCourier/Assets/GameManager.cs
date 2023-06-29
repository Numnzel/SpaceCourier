using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {

    [Header("Properties")]
    public static GameManager instance;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private MeshRenderer backgroundPlane;
    [SerializeField] private ParallaxManager parallaxManager;
    [SerializeField] private Volume postProcessingManager;
    public ObjectiveArrows objectiveArrows;

    [Header("Levels")]
    public LevelSO levelMainScene;
    public List<LevelSO> levels = new List<LevelSO>();
    public List<Episode> episodes;
    private LevelSO currentLevel;

    public bool runningWindows = Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;

	public bool RunningWindows { get => runningWindows; }

	void Awake() {

        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else
            Destroy(gameObject);
    }

	private void Start() {

        Physics.gravity = Vector3.zero;

        //scenesManager.SetCurrentScene(titleSceneIndex);

        // Create a default game data file if it does not exist.
        if (!GameDataManager.LoadGameData())
            GameDataManager.SaveGameData();

        // Initialize progression
		foreach (Episode episode in episodes)
            if (!GameDataManager.gameData.progression.ContainsKey(episode.title))
                GameDataManager.gameData.progression.Add(episode.title, 0);

        GameDataManager.SaveGameData();

        // Apply stored player options.
        PlayerDataManager.ReadPlayerData();
        CanvasManager.instance.SetPlayerOptions();
    }

    public void SetSoundVolume(float value) {

        float volume = value > 0 ? Mathf.Log10(value) * 20f : -80f;

        audioMixer.SetFloat("SoundVolume", volume);
    }

    public void SetMusicVolume(float value) {

        float volume = value > 0 ? Mathf.Log10(value) * 20f : -80f;

        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void LoadLevel(LevelSO level) {

        int currentSceneIndex = ScenesManager.instance.GetCurrentScene().buildIndex;

        if (level == levelMainScene) {

            ScenesManager.instance.UnloadScene(currentSceneIndex);
            //scenesManager.SetCurrentScene(titleSceneIndex);
            Controller.instance.RestartCamera();
            CanvasManager.instance.IsolateCanvasTitle();
            CanvasManager.instance.SetTitleTruck(true);
            //CanvasManager.instance.ShowCanvasGroup(sceneIndex);
        }
        else {
            
            if (currentSceneIndex != ScenesManager.titleSceneIndex)
                ScenesManager.instance.UnloadScene(currentSceneIndex);

            CanvasManager.instance.SetTitleTruck(false);
            SetLevel(level);
            //scenesManager.SetCurrentScene(loadingSceneIndex);
            CanvasManager.instance.SetGameCanvas();
        }

        currentLevel = level;

        ResumeGame();
    }

    private void SetLevel(LevelSO level) {

        ScenesManager.instance.LoadScene(level.sceneIndex);
        backgroundPlane.material.mainTexture = level.backgroundPlaneTexture;
        parallaxManager.parallaxTexture = level.parallaxPlaneTexture;
        parallaxManager.UpdateCopiesTexture();
        postProcessingManager.profile = level.volumeProfile;
        CanvasManager.instance.StartTimer();
    }

    public void RestartScene() {

        if (currentLevel == levelMainScene || ScenesManager.instance.lockScene)
            return;

        // Remove hardcore level count if not completed
        if (GameDataManager.gameData.statistics.ContainsKey("LevelHardcore") && !GameDataManager.gameData.achievements.Contains("safety") && ScenesManager.instance.GetCurrentScene().buildIndex <= 12) {

            GameDataManager.gameData.statistics["LevelHardcore"].value = 0;
            GameDataManager.SaveGameData();
        }

        LoadLevel(currentLevel);
    }

    public void SetCompletedLevel() {

        ScenesManager.instance.lockScene = true;
        GameDataManager.LoadGameData();
        CanvasManager.instance.StopTimer();
        GameDataManager.gameData.progression[currentLevel.episode] = Mathf.Max(GameDataManager.gameData.progression[currentLevel.episode], currentLevel.sceneIndex);
        UpdateLevelTime(currentLevel.sceneIndex, CanvasManager.instance.LevelTime);
        GameDataManager.SaveGameData();
    }

    private void UpdateLevelTime(int sceneIndex, float time) {

        if (GameDataManager.gameData.levelData.Find(x => x.id == sceneIndex) != null) {

            int index = GameDataManager.gameData.levelData.FindIndex(0, x => x.id == sceneIndex);
            GameDataManager.gameData.levelData[index].time = Mathf.Min(time, GameDataManager.gameData.levelData[index].time);
        }
        else
            GameDataManager.gameData.levelData.Add(new LevelData(sceneIndex, time));
	}

	public void OpenKofi() {

        Application.OpenURL("https://ko-fi.com/numnzel");
    }

    public void OpenGit() {

        Application.OpenURL("https://github.com/Numnzel/SpaceCourier");
    }

    public void ExitGame() {

        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.ExecuteMenuItem("Edit/Play");
        #endif
	}

    public void StopGame() {

        Time.timeScale = 0;
    }

    public void ResumeGame() {

        Time.timeScale = 1;
    }
}
