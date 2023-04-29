using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

	//private MaterialPropertyBlock materialPropertyBlock;
	//[SerializeField] private Renderer meshRenderer;

	private void Awake() {

		//SetShader();
		InitialInclination();
	}

	/* private void SetShader() {

		materialPropertyBlock = new();
		meshRenderer.GetPropertyBlock(materialPropertyBlock);

		int property = Shader.PropertyToID("_Seed");
		float seed = (transform.position.x + transform.position.z) / 10.0f;
		
		materialPropertyBlock.SetFloat(property, seed);
		meshRenderer.SetPropertyBlock(materialPropertyBlock);
	} */

	private void InitialInclination() {

		float maxDegrees = (1.0f + Mathf.Sin(transform.position.x)) + (1.0f + Mathf.Cos(transform.position.z)) * 5.0f;

		Vector3 inclination = new Vector3(transform.localRotation.x, transform.localRotation.y + Random.Range(0, 360f), transform.localRotation.z + Random.Range(0, maxDegrees));
		
		transform.localRotation = Quaternion.Euler(inclination);
	}
}
