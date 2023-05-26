using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

	[SerializeField] private MeshRenderer meshRenderer;
	private MaterialPropertyBlock materialPropertyBlock;

	private void Awake() {

		InitialInclination();
		SetShader();
	}

	private void SetShader() {

		materialPropertyBlock = new MaterialPropertyBlock();
		meshRenderer.GetPropertyBlock(materialPropertyBlock);
		int property = Shader.PropertyToID("_Position2D");

		materialPropertyBlock.SetVector(property, new Vector2(transform.position.x, transform.position.y));
		meshRenderer.SetPropertyBlock(materialPropertyBlock);
	}

	private void InitialInclination() {

		float maxDegrees = (1.0f + Mathf.Sin(transform.position.x)) + (1.0f + Mathf.Cos(transform.position.z)) * 5.0f;

		Vector3 inclination = new Vector3(transform.localRotation.x, transform.localRotation.y + Random.Range(0, 360f), transform.localRotation.z + Random.Range(0, maxDegrees));
		
		transform.localRotation = Quaternion.Euler(inclination);
	}
}
