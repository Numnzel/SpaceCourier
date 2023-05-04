using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    [SerializeField] private CanvasGroup canvasMenu;
    [SerializeField] private CanvasGroup canvasOptions;
    [SerializeField] private CanvasGroup canvasTitle;
    [SerializeField] private CanvasGroup canvasGame;
    [SerializeField] private CanvasGroup canvasAndroid;
    [SerializeField] private CanvasGroup canvasLevels;
    [SerializeField] private RectTransform canvasBars;
    [SerializeField] private CanvasGroup canvasReadme;
    [SerializeField] private RawImage minimap;
    [SerializeField] private RawImage minimapBackground;
    [SerializeField] private Slider sliderMinimap;
    [SerializeField] private Slider sliderScale;
    [SerializeField] private Slider sliderSound;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private ScenesManager scenesManager;
    [SerializeField] private Button[] canvasLevelsButtons;
    private Stack<CanvasGroup> canvasFocus = new Stack<CanvasGroup>();

    public bool lockScene = true;
    private bool runningWindows = Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;

    const int titleSceneIndex = 0;

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
        LoadUserConfiguration();
        ShowCanvasGroup(canvasTitle);
    }

	public void EnterMenuOrReturn() {

        if (!canvasFocus.Contains(canvasMenu) && !canvasFocus.Contains(canvasTitle))
            ShowMenu();
        else
            HideCanvasGroup();
	}

	public void ShowMenu() {
        
        if (scenesManager.GetCurrentScene().buildIndex != titleSceneIndex)
            ShowCanvasGroup(canvasMenu);
    }
	
    public void ShowOptions() { ShowCanvasGroup(canvasOptions); }
    public void ShowTitle() { ShowCanvasGroup(canvasTitle); }
    public void ShowReadme() { ShowCanvasGroup(canvasReadme); }
    public void ShowLevels() {
        
        DataManager.LoadPlayerData();
        for (int i = 0; i < canvasLevelsButtons.Length; i++)
            canvasLevelsButtons[i].interactable = !(i > PlayerData.progression);

        ShowCanvasGroup(canvasLevels);
    }
    
    public void SetGameCanvas() {

        SetMainCanvas(canvasGame);

        UIUtils.SetCanvasGroup(canvasAndroid, !RunningWindows);
    }

    public void ShowCanvasGroup(CanvasGroup canvasGroup) {

        if (canvasGroup == null)
            return;

        UIUtils.SetCanvasGroup(canvasGroup, true);
        canvasFocus.Push(canvasGroup);
    }
    
    public void HideCanvasGroup() {

        if (canvasFocus.Count <= 1)
            return;

        UIUtils.SetCanvasGroup(canvasFocus.Peek(), false);
        canvasFocus.Pop();
    }

    private void HideAllCanvasGroup() {
        
        while (canvasFocus.Count > 0) {
            
            UIUtils.SetCanvasGroup(canvasFocus.Peek(), false);
            canvasFocus.Pop();
        }
    }

    public void LoadUserConfiguration() {

        if (!DataManager.LoadPlayerData())
            return;

        ApplyConfiguration();

        sliderMinimap.value = PlayerData.optionValue_mapAlpha;
        sliderScale.value = PlayerData.optionValue_uiScale;
        sliderSound.value = PlayerData.optionValue_sound;
        sliderMusic.value = PlayerData.optionValue_music;
    }

    public void SaveUserConfiguration() {

        ApplyConfiguration();
        DataManager.SavePlayerData();
    }

    private void ApplyConfiguration() {

        SetMinimapAlpha(PlayerData.optionValue_mapAlpha);
        SetBarsScale(PlayerData.optionValue_uiScale);
        SetSoundVolume(PlayerData.optionValue_sound);
        SetMusicVolume(PlayerData.optionValue_music);
    }

    public void SetMainCanvas(CanvasGroup canvasGroup) {

        HideAllCanvasGroup();
        ShowCanvasGroup(canvasGroup);
    }

    public void SetConfigurationMinimapAlpha() {

        PlayerData.optionValue_mapAlpha = Mathf.Min(sliderMinimap.value, sliderMinimap.maxValue);
    }

    public void SetConfigurationBarsScale() {

        PlayerData.optionValue_uiScale = Mathf.Min(sliderScale.value, sliderScale.maxValue);
    }

    public void SetConfigurationSoundVolume() {

        PlayerData.optionValue_sound = Mathf.Min(sliderSound.value, sliderSound.maxValue);
    }

    public void SetConfigurationMusicVolume() {

        PlayerData.optionValue_music = Mathf.Min(sliderMusic.value, sliderMusic.maxValue);
    }

    private void SetMinimapAlpha(float value) {

        UIUtils.SetImageAlpha(ref minimap, value);
        UIUtils.SetImageAlpha(ref minimapBackground, value);
    }

    private void SetBarsScale(float value) {

        UIUtils.SetCanvasScale(canvasBars, value);
    }

    private void SetSoundVolume(float value) {

        float volume = value > 0 ? Mathf.Log10(value) * 20f : -80f;

        audioMixer.SetFloat("SoundVolume", volume);
    }

    private void SetMusicVolume(float value) {

        float volume = value > 0 ? Mathf.Log10(value) * 20f : -80f;

        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void LoadScene(int loadingSceneIndex) {

        int currentSceneIndex = scenesManager.GetCurrentScene().buildIndex;

        if (loadingSceneIndex == titleSceneIndex) {
            scenesManager.UnloadScene(currentSceneIndex);
            //scenesManager.SetCurrentScene(titleSceneIndex);
            SetMainCanvas(canvasTitle);
        }
        else {
            
            if (currentSceneIndex != titleSceneIndex)
                scenesManager.UnloadScene(currentSceneIndex);
            
            scenesManager.LoadScene(loadingSceneIndex);
            //scenesManager.SetCurrentScene(loadingSceneIndex);
            SetGameCanvas();
        }
	}

    public void RestartScene() {

        int currentSceneIndex = scenesManager.GetCurrentScene().buildIndex;

        if (currentSceneIndex == titleSceneIndex || lockScene)
            return;

        LoadScene(currentSceneIndex);
    }

    public void SetCompletedLevel() {

        int currentSceneIndex = scenesManager.GetCurrentScene().buildIndex;

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
