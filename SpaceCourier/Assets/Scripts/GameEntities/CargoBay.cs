using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoBay : MonoBehaviour {

    [SerializeField] private SphereCollider triggerSphere;
    [SerializeField] private SpriteRenderer minimapSprite;
    [SerializeField] private Sprite completedSprite;
    [SerializeField] private Animator animator;
    [SerializeField] private Renderer meshRenderer1;
    [SerializeField] private Renderer meshRenderer2;

    private bool satisfied = false;
    private MaterialPropertyBlock materialPropertyBlock;
    ObjectiveArrows arrows;
    ObjectiveArrow arrow;

    private void Start() {

        arrows = GameManager.instance.objectiveArrows;

        if (arrow == null)
            arrow = arrows.CreateArrow(this.gameObject);
    }

	private void OnTriggerEnter(Collider other) {

        Ship ship;

        if (other.TryGetComponent<Ship>(out ship) && !ship.dead && ship.loadCount > 0 && !satisfied) {

            SetShader();
            animator.SetBool("IsLoaded", true);
            minimapSprite.sprite = completedSprite;
            triggerSphere.enabled = false;
            satisfied = true;
            Destroy(arrow.gameObject);

            ship.Unload();
        }
    }

    private void SetShader() {

		materialPropertyBlock = new MaterialPropertyBlock();
		meshRenderer1.GetPropertyBlock(materialPropertyBlock);
		meshRenderer2.GetPropertyBlock(materialPropertyBlock);

		int property = Shader.PropertyToID("_EmissiveColor");
		Color color = new Color(1.0f, 0, 0);
		
		materialPropertyBlock.SetColor(property, color);
		meshRenderer1.SetPropertyBlock(materialPropertyBlock);
		meshRenderer2.SetPropertyBlock(materialPropertyBlock);
	}
}
