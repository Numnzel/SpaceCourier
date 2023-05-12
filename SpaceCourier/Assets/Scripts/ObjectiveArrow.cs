using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveArrow : MonoBehaviour {

    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image image;
    public GameObject objective;

	private void OnEnable() {

        GameManager.instance.OnApplyArrowsAlpha.AddListener(ChangeAlpha);
    }

	private void OnDisable() {

        GameManager.instance.OnApplyArrowsAlpha.RemoveListener(ChangeAlpha);
    }

	private void Update() {

        if (objective == null)
            Destroy(this.gameObject);
        else {

            // Get the position of the object in screen space
            Vector3 objScreenPos = Camera.main.WorldToScreenPoint(objective.transform.position);

            // Get the directional vector between the arrow and the object
            Vector3 dir = (objScreenPos - rectTransform.position).normalized;

            // Calculate the angle 
            float angle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);

            // Update the rotation
            rectTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void ChangeAlpha(float value) {

        image.color = new Color(image.color.r, image.color.g, image.color.b, value);
    }
}