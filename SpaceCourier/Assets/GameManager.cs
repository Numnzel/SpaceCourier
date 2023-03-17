using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    [SerializeField] private CanvasGroup canvasMenu;
    [SerializeField] private CanvasGroup canvasOptions;
    [SerializeField] private RawImage minimap;
    [SerializeField] private RawImage minimapBackground;
    private Stack<CanvasGroup> canvasFocus = new Stack<CanvasGroup>();
    private Dictionary<CanvasGroup, bool> canvasGroupsStatus = new Dictionary<CanvasGroup, bool>();

    void Awake() {

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

	private void Start() {

		canvasGroupsStatus.Add(canvasMenu, true);
		canvasGroupsStatus.Add(canvasOptions, true);
	}

    public void EnterMenuOrReturn() {

        if (canvasFocus.Count == 0)
            ToggleMenu();
        else
            EscapeFocus();
	}
    public void EscapeFocus() { SetCanvasGroup(canvasFocus.Peek(), false); }
	public void ToggleMenu() { ToogleCanvasGroup(canvasMenu); }
	public void ToggleOptions() { ToogleCanvasGroup(canvasOptions); }

    public void ToogleCanvasGroup(CanvasGroup canvasGroup) {

        if (canvasGroup == null)
            return;

        bool status;
        if (canvasGroupsStatus.TryGetValue(canvasGroup, out status)) {

            canvasGroupsStatus[canvasGroup] = !status;
            EnableCanvasGroup(canvasGroup, status);
        }
    }

    public void SetCanvasGroup(CanvasGroup canvasGroup, bool set) {

        if (canvasGroup == null)
            return;

        bool status;
        if (canvasGroupsStatus.TryGetValue(canvasGroup, out status)) {

            canvasGroupsStatus[canvasGroup] = !set;
            EnableCanvasGroup(canvasGroup, set);
        }
    }

    private void EnableCanvasGroup(CanvasGroup canvasGroup, bool set) {

        UIUtils.EnableCanvasGroup(canvasGroup, set);

        // Update focus of the canvas
        if (!canvasGroupsStatus[canvasGroup])
            canvasFocus.Push(canvasGroup);
        else
            canvasFocus.Pop();
    }

    public void SetMinimapAlpha(Single value) {

        SetImageAlpha(minimap, value);
        SetImageAlpha(minimapBackground, value);
    }

    private void SetImageAlpha(RawImage image, float value) {

        UIUtils.SetImageAlpha(ref image, value);
    }
}
