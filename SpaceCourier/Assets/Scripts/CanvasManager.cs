using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class CanvasManager : MonoBehaviour {

    public static CanvasManager instance;

    [Header("Canvas")]
    [SerializeField] private CanvasGroup canvasMenu;
    [SerializeField] private CanvasGroup canvasOptions;
    [SerializeField] private CanvasGroup canvasTitle;
    [SerializeField] private CanvasGroup canvasGame;
    [SerializeField] private CanvasGroup canvasAndroid;
    [SerializeField] private CanvasGroup canvasLevels;
    [SerializeField] private CanvasGroup canvasControls;
    [SerializeField] private CanvasGroup canvasReadme;
    [SerializeField] private CanvasGroup canvasHowToPlay;
    [SerializeField] private CanvasGroup canvasCredits;
    [SerializeField] private CanvasGroup canvasChangelog;
    [SerializeField] private CanvasGroup canvasAchievements;
    [SerializeField] private RectTransform canvasBars;
    [SerializeField] private LevelButton[] canvasLevelsButtons;
    [SerializeField] private RawImage minimap;
    [SerializeField] private RawImage minimapBackground;
    [SerializeField] private Slider sliderMinimap;
    [SerializeField] private Slider sliderBars;
    [SerializeField] private Slider sliderSound;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderArrows;
    [SerializeField] private Slider sliderProgression;
    [SerializeField] private Button buttonMutePropulsion;
    [SerializeField] private Button buttonHideRadio;
    [SerializeField] private Button buttonRadio;
    [SerializeField] private TextMeshProUGUI sliderMinimapVal;
    [SerializeField] private TextMeshProUGUI sliderBarsVal;
    [SerializeField] private TextMeshProUGUI sliderSoundVal;
    [SerializeField] private TextMeshProUGUI sliderMusicVal;
    [SerializeField] private TextMeshProUGUI sliderArrowsVal;
    [SerializeField] private TextMeshProUGUI sliderProgressionVal;
    [SerializeField] private TextMeshProUGUI gameVersion;
    [SerializeField] private TextMeshProUGUI gameTimer;
    [SerializeField] private GameObject titleTruck;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip soundClick1;
    [SerializeField] private AudioClip soundClick2;
    [SerializeField] private AudioClip soundAchievementUnlocked;
    private Stack<CanvasGroup> canvasFocus = new Stack<CanvasGroup>();

    [Header("Achievements")]
    [SerializeField] private GameObject achievementPrefab;
    [SerializeField] private GameObject achievementContent;
    [SerializeField] private GameObject achievementPopupPrefab;
    [SerializeField] private GameObject achievementPopupContent;
    [SerializeField] private float achievementPopupFadeinDuration;
    [SerializeField] private float achievementPopupFadeoutDuration;
    [SerializeField] private float achievementPopupDuration;
    private List<Achievement> achievementsInDisplay = new List<Achievement>();

    [Header("Option Events")]
    public UnityEvent<float> OnApplyArrowsAlpha;
    public UnityEvent<float> OnApplyMinimapAlpha;
    public UnityEvent<float> OnApplyBarsScale;
    public UnityEvent<float> OnApplyMusicVolume;
    public UnityEvent<float> OnApplySoundVolume;
    public UnityEvent<bool> OnApplyMuteTruckPropulsion;
    public UnityEvent<bool> OnApplyHideRadio;

    public const int titleSceneIndex = 0;

    private Coroutine timeCounter;
    private float levelTime;
    public float LevelTime { get => levelTime; }

    void Awake() {

        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else
            Destroy(gameObject);
    }

	private void OnEnable() {

        AchievementManager.OnAchievementUnlock += (achievement) => StartCoroutine(PopupAchievement(achievement));
    }

	private void Start() {

        IsolateCanvasTitle();
        gameVersion.text = Application.version;
        SetTitleTruck(true);
    }

    public void EnterMenuOrReturn() {

        if (!canvasFocus.Contains(canvasMenu) && !canvasFocus.Contains(canvasTitle)) {

            GameManager.instance.StopGame();
            ShowMenu();
        }
        else {

            GameManager.instance.ResumeGame();
            HideCanvasGroup();
        }
    }

    public void ShowMenu() {

        if (ScenesManager.instance.GetCurrentScene().buildIndex != titleSceneIndex)
            ShowCanvasGroup(canvasMenu);
    }

    public void ShowOptions() {

        DataManager.LoadPlayerData();
        LoadAndSetCurrentConfigurationControlsValues();
        ShowCanvasGroup(canvasOptions);
    }

    public void ShowTitle() { ShowCanvasGroup(canvasTitle); }
    public void ShowReadme() { ShowCanvasGroup(canvasReadme); }
    public void ShowControls() { ShowCanvasGroup(canvasControls); }
    public void ShowHowToPlay() { ShowCanvasGroup(canvasHowToPlay); }
    public void ShowCredits() { ShowCanvasGroup(canvasCredits); }
    public void ShowChangelog() { ShowCanvasGroup(canvasChangelog); }
    public void ShowAchievements() {

        DataManager.LoadPlayerData();
        LoadAndDisplayAchievements();
        ShowCanvasGroup(canvasAchievements);
    }

    public void ShowLevels() {

        DataManager.LoadPlayerData();
        LoadAndSetLevelButtons();
        ShowCanvasGroup(canvasLevels);
    }

    private void LoadAndSetLevelButtons() {

        // Set progression bar
        sliderProgression.value = PlayerData.progression;
        sliderProgressionVal.text = Mathf.RoundToInt(sliderProgression.value).ToString() + "/12";

        // Set level buttons
        for (int i = 0; i < canvasLevelsButtons.Length; i++) {

            LevelButton levelButton = canvasLevelsButtons[i];

            if (levelButton == null)
                continue;

            // Lock button if progression is not enough
            bool unlocked = PlayerData.progression >= i;
            levelButton.SetLock(!unlocked);
            levelButton.GetComponent<Button>().interactable = unlocked;

            // Set current rank
            int rank = 0;
            float levelTime = 0;
            LevelSO levelInfo = GameManager.instance.levels[i+1];

            if (PlayerData.levelTime.ContainsKey(i+1) && PlayerData.levelTime[i+1] > 0) {

                levelTime = Mathf.Floor(PlayerData.levelTime[i+1]);

                if (levelTime <= levelInfo.rankTimes.y)
                    rank = 3;
                else if (levelTime <= levelInfo.rankTimes.x)
                    rank = 2;
                else
                    rank = 1;

                // Show current/next times and ranks
                levelButton.textTimeVal.text = FormatTime(levelTime);
                levelButton.SetRank(rank);

                switch (rank) {

                    case 3:
                        levelButton.textNextVal.text = "";
                        break;
                    case 2:
                        levelButton.textNextVal.text = FormatTime(levelInfo.rankTimes.y);
                        break;
                    case 1:
                    default:
                        levelButton.textNextVal.text = FormatTime(levelInfo.rankTimes.x);
                        break;
                }
            }
        }
    }

    private void LoadAndDisplayAchievements() {

        foreach (AchievementDisplay achievementDisplay in achievementContent.GetComponentsInChildren<AchievementDisplay>())
            Destroy(achievementDisplay.gameObject);

        int i = 0;
		foreach (Achievement achievement in PlayerData.achievements)
            CreateAchievementDisplay(achievement, achievementPrefab, achievementContent).GetComponent<RectTransform>().position += new Vector3(0, -150f * i++);
    }

    private AchievementDisplay CreateAchievementDisplay(Achievement achievement, GameObject prefab, GameObject parent) {
         
        AchievementDisplay achievementDisplay = Instantiate(prefab, parent.transform).GetComponent<AchievementDisplay>();
        achievementDisplay.imgIcon.sprite = achievement.icon;
        achievementDisplay.textName.text = achievement.name;
        achievementDisplay.textDescription.text = achievement.description;

        Stat achievementStat = PlayerData.InitializeStatistic(achievement.requirement.name);
        achievementDisplay.slider.SetProgress(achievementStat.value, achievement.requirement.value);

        return achievementDisplay;
    }

    private IEnumerator PopupAchievement(Achievement achievement) {

        achievementsInDisplay.Add(achievement);

        AchievementDisplay aDisplay = CreateAchievementDisplay(achievement, achievementPopupPrefab, achievementPopupContent);
        RectTransform rectTransform = aDisplay.GetComponent<RectTransform>();
        CanvasGroup aGroup = aDisplay.GetComponent<CanvasGroup>();

        rectTransform.position += new Vector3(0, -150f * achievementsInDisplay.Count-1);

        float timeStamp = Time.time + achievementPopupFadeinDuration;
        float currentAlpha = 0f;
        float currentScale = 1.5f;

        aGroup.alpha = currentAlpha;
        rectTransform.localScale = currentScale * Vector3.one;

        audioSource.PlayOneShot(soundAchievementUnlocked);

        while (Time.time < timeStamp) {

            float t = (Time.time - (timeStamp - achievementPopupFadeinDuration)) / achievementPopupFadeinDuration;

            currentScale = Mathf.Lerp(2.0f, 1.0f, t);
            currentAlpha = Mathf.Lerp(0f, 1.0f, t);

            aGroup.alpha = currentAlpha;
            rectTransform.localScale = currentScale * Vector3.one;

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(achievementPopupDuration);

        timeStamp = Time.time + achievementPopupFadeoutDuration;

        while (Time.time < timeStamp) {

            float t = (Time.time - (timeStamp - achievementPopupFadeinDuration)) / achievementPopupFadeinDuration;

            currentAlpha = Mathf.Lerp(1.0f, 0f, t);

            aGroup.alpha = currentAlpha;

            yield return new WaitForEndOfFrame();
        }

        achievementsInDisplay.Remove(achievement);
        Destroy(aDisplay.gameObject);
    }

    public void SetGameCanvas() {

        IsolateCanvas(canvasGame);

        UIUtils.SetCanvasGroup(canvasAndroid, !GameManager.instance.RunningWindows);
    }

    public void ShowCanvasGroup(CanvasGroup canvasGroup) {

        if (canvasGroup == null)
            return;

        if (canvasGroup != canvasTitle)
            PlayButtonSoundOpen();

        UIUtils.SetCanvasGroup(canvasGroup, true); // show and enable the new canvas group

        if (canvasFocus.Count > 0)
            UIUtils.SetCanvasGroupInteractable(canvasFocus.Peek(), false); // disable previous canvas group

        canvasFocus.Push(canvasGroup); // set canvas group as focus
    }

    public void HideCanvasGroup() {

        if (canvasFocus.Count <= 1)
            return;

        PlayButtonSoundClose();

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

    public void SaveUserConfiguration() {

        PlayButtonSoundOpen();

        DataManager.SavePlayerData();
        ApplyConfiguration();
    }

    public void ApplyConfiguration() {

        OnApplyArrowsAlpha.Invoke(PlayerData.optionValue_arrowsAlpha);
        OnApplyMinimapAlpha.Invoke(PlayerData.optionValue_mapAlpha);
        OnApplyBarsScale.Invoke(PlayerData.optionValue_uiScale);
        OnApplyMusicVolume.Invoke(PlayerData.optionValue_music);
        OnApplySoundVolume.Invoke(PlayerData.optionValue_sound);
        OnApplyMuteTruckPropulsion.Invoke(PlayerData.optionValue_mutePropulsion);
        OnApplyHideRadio.Invoke(PlayerData.optionValue_hideRadio);
    }

    private void LoadAndSetCurrentConfigurationControlsValues() {

        sliderArrows.value = PlayerData.optionValue_arrowsAlpha;
        sliderMinimap.value = PlayerData.optionValue_mapAlpha;
        sliderBars.value = PlayerData.optionValue_uiScale;
        sliderSound.value = PlayerData.optionValue_sound;
        sliderMusic.value = PlayerData.optionValue_music;
        buttonMutePropulsion.GetComponent<SwitchButton>().SetValue(PlayerData.optionValue_mutePropulsion);
        buttonHideRadio.GetComponent<SwitchButton>().SetValue(PlayerData.optionValue_hideRadio);
        SetConfigurationIndicator(sliderArrowsVal, PlayerData.optionValue_arrowsAlpha * 100);
        SetConfigurationIndicator(sliderMinimapVal, PlayerData.optionValue_mapAlpha * 100);
        SetConfigurationIndicator(sliderBarsVal, PlayerData.optionValue_uiScale * 50 + 100);
        SetConfigurationIndicator(sliderSoundVal, PlayerData.optionValue_sound * 100);
        SetConfigurationIndicator(sliderMusicVal, PlayerData.optionValue_music * 100);
    }

    public void IsolateCanvasTitle() {

        IsolateCanvas(canvasTitle);
	}

    public void IsolateCanvas(CanvasGroup canvasGroup) {

        HideAllCanvasGroup();
        ShowCanvasGroup(canvasGroup);
    }

    public void SetTitleTruck(bool set) {

        if (set)
            titleTruck.transform.localScale = new Vector3(100, 100, 100);
        else
            titleTruck.transform.localScale = Vector3.zero;
	}

    public void SetConfigurationMinimapAlpha() {

        PlayerData.optionValue_mapAlpha = Mathf.Min(sliderMinimap.value, sliderMinimap.maxValue);
        SetConfigurationIndicator(sliderMinimapVal, PlayerData.optionValue_mapAlpha * 100);
    }

    public void SetConfigurationBarsScale() {

        PlayerData.optionValue_uiScale = Mathf.Min(sliderBars.value, sliderBars.maxValue);
        SetConfigurationIndicator(sliderBarsVal, PlayerData.optionValue_uiScale * 50 + 100);
    }

    public void SetConfigurationSoundVolume() {

        PlayerData.optionValue_sound = Mathf.Min(sliderSound.value, sliderSound.maxValue);
        SetConfigurationIndicator(sliderSoundVal, PlayerData.optionValue_sound * 100);
    }

    public void SetConfigurationMusicVolume() {

        PlayerData.optionValue_music = Mathf.Min(sliderMusic.value, sliderMusic.maxValue);
        SetConfigurationIndicator(sliderMusicVal, PlayerData.optionValue_music * 100);
    }

    public void SetConfigurationArrowsAlpha() {

        PlayerData.optionValue_arrowsAlpha = Mathf.Min(sliderArrows.value, sliderArrows.maxValue);
        SetConfigurationIndicator(sliderArrowsVal, PlayerData.optionValue_arrowsAlpha * 100);
    }

    public void SetConfigurationHideRadio() {

        SwitchButton button = buttonHideRadio.GetComponent<SwitchButton>();
        button.SwitchValue();
        PlayerData.optionValue_hideRadio = button.value;
    }

    public void SetConfigurationMutePropulsion() {

        SwitchButton button = buttonMutePropulsion.GetComponent<SwitchButton>();
        button.SwitchValue();
        PlayerData.optionValue_mutePropulsion = button.value;
    }

    public void SetRadio(bool value) {

        Image radioImage = buttonRadio.GetComponent<Image>();
        float alphaValue = value ? 0 : 1.0f;

        buttonRadio.enabled = !value;
        UIUtils.SetImageAlpha(ref radioImage, alphaValue);
    }

    private void SetConfigurationIndicator(TextMeshProUGUI text, float value) {

        text.text = Mathf.RoundToInt(value).ToString() + "%";
	}

    public void SetMinimapAlpha(float value) {

        UIUtils.SetImageAlpha(ref minimap, value);
        UIUtils.SetImageAlpha(ref minimapBackground, value);
    }

    public void SetBarsScale(float value) {

        UIUtils.SetCanvasScale(canvasBars, value);
    }

    public void StartTimer() {

        if (timeCounter != null)
            StopTimer();

        timeCounter = StartCoroutine(CountLevelTime());
	}

    public void StopTimer() {

        StopCoroutine(timeCounter);
    }

    public void PlayButtonSoundOpen() {

        audioSource.PlayOneShot(soundClick2);
    }

    public void PlayButtonSoundClose() {

        audioSource.PlayOneShot(soundClick1);
    }

    private IEnumerator CountLevelTime() {

        float levelTimeMem = Time.time;

        while (true) {

            levelTime = Time.time - levelTimeMem;
            gameTimer.text = FormatTime(LevelTime);
            yield return new WaitForSeconds(0.01f);
        }
	}

    private string FormatTime(float timeToDisplay) {

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
