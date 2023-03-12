using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

    [SerializeField] private SphereCollider triggerSphere;
    [SerializeField] private SpriteRenderer minimapSprite;
    [SerializeField] private Sprite openedSprite;
    [SerializeField] private Ship ship;
    [SerializeField] private ParticleSystem particles;
    private bool blocked = true;

	private void Start() {

        triggerSphere.enabled = false;
		particles.Stop();
    }

	private void Update() {

        if (ship.loadCount == 0) {

            blocked = false;
            triggerSphere.enabled = true;
            minimapSprite.sprite = openedSprite;
            particles.Play();
        }
	}

	private void OnTriggerEnter(Collider other) {

        Ship ship;

        if (other.TryGetComponent<Ship>(out ship) && !ship.dead && ship.loadCount == 0 && !blocked) {

            Debug.Log("LEVEL ENDED");
        }
    }
}
