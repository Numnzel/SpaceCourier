using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    [SerializeField] private CanvasGroup canvasMenu;
    [SerializeField] private CanvasGroup canvasOptions;
    [SerializeField] private CanvasGroup canvasTitle;
    [SerializeField] private CanvasGroup canvasGame;
    [SerializeField] private RawImage minimap;
    [SerializeField] private RawImage minimapBackground;
    [SerializeField] private ScenesManager scenesManager;
    private Stack<CanvasGroup> canvasFocus = new Stack<CanvasGroup>();

    public bool lockScene = true;

    const int titleSceneIndex = 0;

    void Awake() {

        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else
            Destroy(gameObject);
    }

	private void Start() {

        //scenesManager.SetCurrentScene(titleSceneIndex);
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

    public void SetMainCanvas(CanvasGroup canvasGroup) {

        HideAllCanvasGroup();
        ShowCanvasGroup(canvasGroup);
    }

    public void SetMinimapAlpha(Single value) {

        SetImageAlpha(minimap, value);
        SetImageAlpha(minimapBackground, value);
    }

    private void SetImageAlpha(RawImage image, float value) {

        UIUtils.SetImageAlpha(ref image, value);
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
            SetMainCanvas(canvasGame);
        }
	}

    public void RestartScene() {

        int currentSceneIndex = scenesManager.GetCurrentScene().buildIndex;

        if (currentSceneIndex == titleSceneIndex || lockScene)
            return;

        LoadScene(currentSceneIndex);
    }

    public void ExitGame() {

        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.ExecuteMenuItem("Edit/Play");
        #endif
	}
}
