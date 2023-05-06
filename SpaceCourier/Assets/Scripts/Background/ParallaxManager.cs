using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour {

    [SerializeField] private float planeSize;
    [SerializeField] private GameObject planePrefab;

    public Texture2D parallaxTexture;

    private List<GameObject> copies = new List<GameObject>();
    private float translationX;
    private float translationZ;
    private float centeringOffsetX;
    private float centeringOffsetZ;

    void Start() {

        for (int i = 0; i < 25; i++)
            copies.Add(Instantiate(planePrefab, transform));

        translationX = planeSize * transform.localScale.x;
        translationZ = planeSize * transform.localScale.z;
        centeringOffsetX = (translationX * 5f) / 2f;
        centeringOffsetZ = (translationZ * 5f) / 2f;

        UpdateCopiesPosition();
    }

    void Update() {

        Translation();
    }

    private void Translation() {

        float deltaX = Camera.main.transform.position.x - transform.position.x;
        float deltaZ = Camera.main.transform.position.z - transform.position.z;

        if (Mathf.Abs(deltaX) > translationX) {

            transform.position += new Vector3(deltaX, 0, 0);
            UpdateCopiesPosition();
        }

        if (Mathf.Abs(deltaZ) > translationZ) {

            transform.position += new Vector3(0, 0, deltaZ);
            UpdateCopiesPosition();
        }
	}

    private void UpdateCopiesPosition() {

        int k = 0;
		for (int i = 0; i < 5; i++)
			for (int j = 0; j < 5; j++)
                copies[k++].transform.position = new Vector3(
                    (transform.position.x + (translationX * i)) - centeringOffsetX,
                    transform.position.y,
                    (transform.position.z + (translationZ * j)) - centeringOffsetZ);
    }

    public void UpdateCopiesTexture() {

		foreach (GameObject copy in copies)
            copy.GetComponent<MeshRenderer>().material.mainTexture = parallaxTexture;
	}
}
