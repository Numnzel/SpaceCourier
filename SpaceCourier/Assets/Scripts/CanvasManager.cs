using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour {

    public static CanvasManager instance;

    [SerializeField] private CanvasGroup canvasMenu;
    [SerializeField] private CanvasGroup canvasOptions;
    [SerializeField] private CanvasGroup canvasTitle;
    [SerializeField] private CanvasGroup canvasGame;
    [SerializeField] private CanvasGroup canvasAndroid;
    [SerializeField] private CanvasGroup canvasLevels;
    [SerializeField] private CanvasGroup canvasControls;
    [SerializeField] private CanvasGroup canvasReadme;
    [SerializeField] private RectTransform canvasBars;
    [SerializeField] private Button[] canvasLevelsButtons;
    [SerializeField] private RawImage minimap;
    [SerializeField] private RawImage minimapBackground;
    [SerializeField] private Slider sliderMinimap;
    [SerializeField] private Slider sliderScale;
    [SerializeField] private Slider sliderSound;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderArrows;
    private Stack<CanvasGroup> canvasFocus = new Stack<CanvasGroup>();
    public const int titleSceneIndex = 0;

    [Header("Option Events")]
    public UnityEvent<float> OnApplyArrowsAlpha;
    public UnityEvent<float> OnApplyMinimapAlpha;
    public UnityEvent<float> OnApplyBarsScale;
    public UnityEvent<float> OnApplyMusicVolume;
    public UnityEvent<float> OnApplySoundVolume;

    void Awake() {

        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else
            Destroy(gameObject);
    }


    private void Start() {

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

        if (ScenesManager.instance.GetCurrentScene().buildIndex != titleSceneIndex)
            ShowCanvasGroup(canvasMenu);
    }

    public void ShowOptions() { ShowCanvasGroup(canvasOptions); }
    public void ShowTitle() { ShowCanvasGroup(canvasTitle); }
    public void ShowReadme() { ShowCanvasGroup(canvasReadme); }
    public void ShowControls() { ShowCanvasGroup(canvasControls); }
    public void ShowLevels() {

        DataManager.LoadPlayerData();
        for (int i = 0; i < canvasLevelsButtons.Length; i++)
            canvasLevelsButtons[i].interactable = !(i > PlayerData.progression);

        ShowCanvasGroup(canvasLevels);
    }

    public void SetGameCanvas() {

        IsolateCanvas(canvasGame);

        UIUtils.SetCanvasGroup(canvasAndroid, !GameManager.instance.RunningWindows);
    }

    public void ShowCanvasGroup(CanvasGroup canvasGroup) {

        if (canvasGroup == null)
            return;

        UIUtils.SetCanvasGroup(canvasGroup, true); // show and enable the new canvas group

        if (canvasFocus.Count > 0)
            UIUtils.SetCanvasGroupInteractable(canvasFocus.Peek(), false); // disable previous canvas group

        canvasFocus.Push(canvasGroup); // set canvas group as focus
    }

    public void HideCanvasGroup() {

        if (canvasFocus.Count <= 1)
            return;

        UIUtils.SetCanvasGroup(canvasFocus.Peek(), false); // hide and disable current focused canvas group
        canvasFocus.Pop(); // remove focus from current canvas
        UIUtils.SetCanvasGroupInteractable(canvasFocus.Peek(), true); // enable previous canvas group
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

        sliderArrows.value = PlayerData.optionValue_arrowsAlpha;
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

        OnApplyArrowsAlpha.Invoke(PlayerData.optionValue_arrowsAlpha);
        OnApplyMinimapAlpha.Invoke(PlayerData.optionValue_mapAlpha);
        OnApplyBarsScale.Invoke(PlayerData.optionValue_uiScale);
        OnApplyMusicVolume.Invoke(PlayerData.optionValue_music);
        OnApplySoundVolume.Invoke(PlayerData.optionValue_sound);
    }

    public void IsolateCanvasTitle() {

        IsolateCanvas(canvasTitle);
	}

    public void IsolateCanvas(CanvasGroup canvasGroup) {

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

    public void SetConfigurationArrowsAlpha() {

        PlayerData.optionValue_arrowsAlpha = Mathf.Min(sliderArrows.value, sliderArrows.maxValue);
    }

    public void SetMinimapAlpha(float value) {

        UIUtils.SetImageAlpha(ref minimap, value);
        UIUtils.SetImageAlpha(ref minimapBackground, value);
    }

    public void SetBarsScale(float value) {

        UIUtils.SetCanvasScale(canvasBars, value);
    }
}
