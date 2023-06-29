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
    [SerializeField] private CanvasGroup canvasTitle;
    [SerializeField] private CanvasGroup canvasGame;
    [SerializeField] private CanvasGroup canvasAndroid;
    [SerializeField] private CanvasGroup canvasLevels;
    [SerializeField] private CanvasGroup canvasLevelsExpert;
    [SerializeField] private RectTransform canvasBars;
    [SerializeField] private LevelButton canvasLevelButtonPrefab;
    [SerializeField] private RawImage minimap;
    [SerializeField] private RawImage minimapBackground;
    [SerializeField] private Slider sliderMinimap;
    [SerializeField] private Slider sliderBars;
    [SerializeField] private Slider sliderSound;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderArrows;
    [SerializeField] private List<ProgressSlider> slidersProgression;
    [SerializeField] private Button buttonMutePropulsion;
    [SerializeField] private Button buttonHideRadio;
    [SerializeField] private Button buttonRadio;
    [SerializeField] private TextMeshProUGUI sliderMinimapVal;
    [SerializeField] private TextMeshProUGUI sliderBarsVal;
    [SerializeField] private TextMeshProUGUI sliderSoundVal;
    [SerializeField] private TextMeshProUGUI sliderMusicVal;
    [SerializeField] private TextMeshProUGUI sliderArrowsVal;
    [SerializeField] private TextMeshProUGUI gameVersion;
    [SerializeField] private TextMeshProUGUI gameTimer;
    [SerializeField] private GameObject titleTruck;
    [SerializeField] private List<Transform> canvasLevelButtonsParent;
    private Dictionary<string, List<LevelButton>> canvasLevelButtons = new Dictionary<string, List<LevelButton>>();
    private Stack<CanvasGroup> canvasFocus = new Stack<CanvasGroup>();

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip soundClick1;
    [SerializeField] private AudioClip soundClick2;
    [SerializeField] private AudioClip soundAchievementUnlocked;

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

    public void SetCanvasOptions() {

        GameDataManager.LoadGameData();
        UpdateOptionsControlsValues();
    }

    public void SetCanvasAchievements() {

        GameDataManager.LoadGameData();
        LoadAndDisplayAchievements();
    }

    public void SetCanvasLevels(Episode episode) {

        GameDataManager.LoadGameData();
        SetProgressionBar(episode);
        LoadAndSetLevelButtons(episode);
    }

    private void SetProgressionBar(Episode episode) {

        int value = GameDataManager.gameData.progression[episode.title];
        int maxValue = GameManager.instance.levels.FindAll(x => x.episode == episode.title).Count;

        slidersProgression[episode.id-1].SetProgress(value, maxValue);
    }

    private void LoadAndSetLevelButtons(Episode episode) {

        List<LevelButton> buttons = new List<LevelButton>();
        List<LevelSO> levels = GameManager.instance.levels.FindAll(x => x.episode == episode.title);
        const int xoffs = 450;
        const int yoffs = -150;
        const int rows = 4;
        int y = 0;
        int x = 0;

        // Find existing buttons
        if (canvasLevelButtons.ContainsKey(episode.title))
            buttons = canvasLevelButtons[episode.title];

        // Set buttons
        foreach (LevelSO level in levels) {

            LevelButton button = buttons.Find(x => x.textLevelName.text == level.title);

            if (button == null) {

                button = Instantiate(canvasLevelButtonPrefab, canvasLevelButtonsParent[episode.id - 1]);
                button.GetComponent<Button>().onClick.AddListener(delegate { GameManager.instance.LoadLevel(level); });
                buttons.Add(button);
            }
            
            // Set button position
            button.transform.localPosition = new Vector3(xoffs * x, yoffs * (y % rows), 0);

            y = (++y % rows == 0) ? 0 : y;
            x = (y == 0) ? ++x : x;

            // Set button info
            button.textLevelName.text = level.title;

            // Lock button if progression is not enough
            bool unlocked = GameDataManager.gameData.progression[episode.title] >= level.progressRequeriment;
            button.SetLock(!unlocked);
            button.GetComponent<Button>().interactable = unlocked;

            // Set current rank
            int rank = 0;
            float levelTime = 0;
            LevelData levelData = GameDataManager.gameData.levelData.Find(a => a.id == level.sceneIndex);

            if (levelData != null && levelData.time > 0) {

                levelTime = Mathf.Floor(levelData.time);

                if (levelTime <= level.rankTimes.y)
                    rank = 3;
                else if (levelTime <= level.rankTimes.x)
                    rank = 2;
                else
                    rank = 1;

                // Show current/next times and ranks
                button.textTimeVal.text = FormatTime(levelTime);
                button.SetRank(rank);

                switch (rank) {

                    case 3:
                        button.textNextVal.text = "";
                        break;
                    case 2:
                        button.textNextVal.text = FormatTime(level.rankTimes.y);
                        break;
                    case 1:
                    default:
                        button.textNextVal.text = FormatTime(level.rankTimes.x);
                        break;
                }
            }
        }

        // Store buttons
        if (canvasLevelButtons.ContainsKey(episode.title))
            canvasLevelButtons[episode.title].AddRange(buttons);
        else
            canvasLevelButtons.Add(episode.title, buttons);
    }

    private void LoadAndDisplayAchievements() {

        foreach (AchievementDisplay achievementDisplay in achievementContent.GetComponentsInChildren<AchievementDisplay>())
            Destroy(achievementDisplay.gameObject);

        int i = 0;
		foreach (Achievement achievement in AchievementManager.instance.achievementList)
            CreateAchievementDisplay(achievement, achievementPrefab, achievementContent).GetComponent<RectTransform>().position += new Vector3(0, -150f * i++);
    }

    private AchievementDisplay CreateAchievementDisplay(Achievement achievement, GameObject prefab, GameObject parent) {
         
        AchievementDisplay achievementDisplay = Instantiate(prefab, parent.transform).GetComponent<AchievementDisplay>();
        achievementDisplay.imgIcon.sprite = achievement.icon;
        achievementDisplay.textName.text = achievement.name;
        achievementDisplay.textDescription.text = achievement.description;

        Stat achievementStat = GameDataManager.gameData.InitializeStatistic(achievement.requirement.name);
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

    public void ApplyPlayerData() {

        PlayerDataManager.WritePlayerData();
        SetPlayerOptions();
    }

    public void SetPlayerOptions() {

        OnApplyArrowsAlpha.Invoke(PlayerDataManager.playerData.optionValue_arrowsAlpha);
        OnApplyMinimapAlpha.Invoke(PlayerDataManager.playerData.optionValue_mapAlpha);
        OnApplyBarsScale.Invoke(PlayerDataManager.playerData.optionValue_uiScale);
        OnApplyMusicVolume.Invoke(PlayerDataManager.playerData.optionValue_music);
        OnApplySoundVolume.Invoke(PlayerDataManager.playerData.optionValue_sound);
        OnApplyMuteTruckPropulsion.Invoke(PlayerDataManager.playerData.optionValue_mutePropulsion);
        OnApplyHideRadio.Invoke(PlayerDataManager.playerData.optionValue_hideRadio);
    }

    private void UpdateOptionsControlsValues() {

        sliderArrows.value = PlayerDataManager.playerData.optionValue_arrowsAlpha;
        sliderMinimap.value = PlayerDataManager.playerData.optionValue_mapAlpha;
        sliderBars.value = PlayerDataManager.playerData.optionValue_uiScale;
        sliderSound.value = PlayerDataManager.playerData.optionValue_sound;
        sliderMusic.value = PlayerDataManager.playerData.optionValue_music;
        buttonMutePropulsion.GetComponent<SwitchButton>().SetValue(PlayerDataManager.playerData.optionValue_mutePropulsion);
        buttonHideRadio.GetComponent<SwitchButton>().SetValue(PlayerDataManager.playerData.optionValue_hideRadio);
        SetConfigurationIndicator(sliderArrowsVal, PlayerDataManager.playerData.optionValue_arrowsAlpha * 100);
        SetConfigurationIndicator(sliderMinimapVal, PlayerDataManager.playerData.optionValue_mapAlpha * 100);
        SetConfigurationIndicator(sliderBarsVal, PlayerDataManager.playerData.optionValue_uiScale * 50 + 100);
        SetConfigurationIndicator(sliderSoundVal, PlayerDataManager.playerData.optionValue_sound * 100);
        SetConfigurationIndicator(sliderMusicVal, PlayerDataManager.playerData.optionValue_music * 100);
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

        PlayerDataManager.playerData.optionValue_mapAlpha = Mathf.Min(sliderMinimap.value, sliderMinimap.maxValue);
        SetConfigurationIndicator(sliderMinimapVal, PlayerDataManager.playerData.optionValue_mapAlpha * 100);
    }

    public void SetConfigurationBarsScale() {

        PlayerDataManager.playerData.optionValue_uiScale = Mathf.Min(sliderBars.value, sliderBars.maxValue);
        SetConfigurationIndicator(sliderBarsVal, PlayerDataManager.playerData.optionValue_uiScale * 50 + 100);
    }

    public void SetConfigurationSoundVolume() {

        PlayerDataManager.playerData.optionValue_sound = Mathf.Min(sliderSound.value, sliderSound.maxValue);
        SetConfigurationIndicator(sliderSoundVal, PlayerDataManager.playerData.optionValue_sound * 100);
    }

    public void SetConfigurationMusicVolume() {

        PlayerDataManager.playerData.optionValue_music = Mathf.Min(sliderMusic.value, sliderMusic.maxValue);
        SetConfigurationIndicator(sliderMusicVal, PlayerDataManager.playerData.optionValue_music * 100);
    }

    public void SetConfigurationArrowsAlpha() {

        PlayerDataManager.playerData.optionValue_arrowsAlpha = Mathf.Min(sliderArrows.value, sliderArrows.maxValue);
        SetConfigurationIndicator(sliderArrowsVal, PlayerDataManager.playerData.optionValue_arrowsAlpha * 100);
    }

    public void SetConfigurationHideRadio() {

        SwitchButton button = buttonHideRadio.GetComponent<SwitchButton>();
        button.SwitchValue();
        PlayerDataManager.playerData.optionValue_hideRadio = button.value;
    }

    public void SetConfigurationMutePropulsion() {

        SwitchButton button = buttonMutePropulsion.GetComponent<SwitchButton>();
        button.SwitchValue();
        PlayerDataManager.playerData.optionValue_mutePropulsion = button.value;
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
