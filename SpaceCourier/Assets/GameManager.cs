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
    public List<LevelSO> levels = new List<LevelSO>();
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

        //scenesManager.SetCurrentScene(titleSceneIndex);
        DataManager.InitializePlayerData();
        CanvasManager.instance.LoadUserConfiguration();
        Physics.gravity = Vector3.zero;
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

        int loadingSceneIndex = level.sceneIndex;
        int currentSceneIndex = ScenesManager.instance.GetCurrentScene().buildIndex;

        if (loadingSceneIndex == ScenesManager.titleSceneIndex) {
            ScenesManager.instance.UnloadScene(currentSceneIndex);
            //scenesManager.SetCurrentScene(titleSceneIndex);
            CanvasManager.instance.IsolateCanvasTitle();
        }
        else {
            
            if (currentSceneIndex != ScenesManager.titleSceneIndex)
                ScenesManager.instance.UnloadScene(currentSceneIndex);
            
            SetLevel(level);
            //scenesManager.SetCurrentScene(loadingSceneIndex);
            CanvasManager.instance.SetGameCanvas();
        }
	}

    private void SetLevel(LevelSO level) {

        ScenesManager.instance.LoadScene(level.sceneIndex);
        backgroundPlane.material.mainTexture = level.backgroundPlaneTexture;
        parallaxManager.parallaxTexture = level.parallaxPlaneTexture;
        parallaxManager.UpdateCopiesTexture();
        postProcessingManager.profile = level.volumeProfile;
    }

    public void RestartScene() {

        int currentSceneIndex = ScenesManager.instance.GetCurrentScene().buildIndex;

        if (currentSceneIndex == ScenesManager.titleSceneIndex || ScenesManager.instance.lockScene)
            return;

        LoadLevel(levels[currentSceneIndex]);
    }

    public void SetCompletedLevel() {

        int currentSceneIndex = ScenesManager.instance.GetCurrentScene().buildIndex;

        Debug.Log("COMPLETED LEVEL");
        DataManager.LoadPlayerData();
        PlayerData.progression = Mathf.Max(PlayerData.progression, currentSceneIndex);
        DataManager.SavePlayerData();
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
}
