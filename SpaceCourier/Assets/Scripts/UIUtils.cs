using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIUtils {

	public static void SetCanvasGroup(CanvasGroup canvasGroup, bool enable) {

		canvasGroup.alpha = enable ? 1.0f : 0.0f;
		canvasGroup.interactable = enable;
		canvasGroup.blocksRaycasts = enable;
	}

	public static void SetCanvasGroupInteractable(CanvasGroup canvasGroup, bool enable) {

		canvasGroup.interactable = enable;
	}

	public static void SetCanvasScale(RectTransform canvas, float value) {
		
		canvas.localScale = new Vector3(value, value, value) + Vector3.one;
	}

	public static void SetImageAlpha(ref RawImage image, float value) {

		image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Clamp01(value));
	}
}
