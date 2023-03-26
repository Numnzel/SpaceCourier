using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

    [SerializeField] private SphereCollider triggerSphere;
    [SerializeField] private SpriteRenderer minimapSprite;
    [SerializeField] private Sprite openedSprite;
    [SerializeField] private Ship shipPrefab;
    [SerializeField] private ParticleSystem particles;
    private bool blocked = true;
    private Ship ship;

	private void Start() {

        ship = Instantiate(shipPrefab, transform);
        ship.transform.position = transform.position;
        ship.transform.rotation = transform.rotation;
        Controller.instance.SetShip(ship);
        triggerSphere.enabled = false;
		particles.Stop();
    }

	private void Update() {

        if (ship.loadCount == 0 && blocked) {

            blocked = false;
            triggerSphere.enabled = true;
            minimapSprite.sprite = openedSprite;
            particles.Play();
        }
	}

	private void OnTriggerEnter(Collider other) {

        Ship ship;

        if (other.TryGetComponent<Ship>(out ship) && !ship.dead && ship.loadCount == 0 && !blocked) {

            Rigidbody SRB = ship.GetComponent<Rigidbody>();
            SRB.velocity = Vector3.zero;
            SRB.isKinematic = true;
        }
    }
}
