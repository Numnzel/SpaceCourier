using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

    [SerializeField] private SphereCollider triggerSphere;
    [SerializeField] private SpriteRenderer minimapSprite;
    [SerializeField] private Sprite openedSprite;
    [SerializeField] private Ship shipPrefab;
    [SerializeField] private ParticleSystem particles;

    private bool portalClosed = true;
    private Ship ship;
    private Coroutine endLevelCoroutine;

	private void Start() {

        ship = Instantiate(shipPrefab, transform);
        ship.transform.position = transform.position;
        ship.transform.rotation = transform.rotation;
        Controller.instance.SetShip(ship);
        triggerSphere.enabled = false;
		particles.Stop();
        GameManager.instance.lockScene = false;
    }

	private void Update() {

        if (ship.loadCount == 0 && portalClosed) {

            portalClosed = false;
            triggerSphere.enabled = true;
            minimapSprite.sprite = openedSprite;
            particles.Play();
        }
	}

	private void OnTriggerEnter(Collider other) {

        Ship ship;

        if (other.TryGetComponent<Ship>(out ship) && !ship.dead && ship.loadCount == 0 && !portalClosed) {

            Rigidbody SRB = ship.GetComponent<Rigidbody>();
            SRB.velocity = Vector3.zero;
            SRB.isKinematic = true;

            particles.Stop();
            endLevelCoroutine = StartCoroutine(EndLevel(ship));
        }
    }

    private IEnumerator EndLevel(Ship ship) {

        GameManager.instance.lockScene = true;
        int counter = 0;

        List<MeshRenderer> truckPieces = new List<MeshRenderer>();

        if (ship != null) {

            ship.DisableShip();
            truckPieces.AddRange(ship.truckModel.GetComponentsInChildren<MeshRenderer>());
        }

        while (++counter < 100) {

            yield return new WaitForSeconds(0.05f);

            if (ship != null) {

                ship.transform.localScale *= 0.98f;
                Vector3 posD = ship.transform.position - transform.position;
                ship.transform.position -= posD * 0.02f;

				foreach (MeshRenderer piece in truckPieces) {
                    
                    if (piece == null)
                        continue;

                    piece.material.color *= new Color(1.02f, 1.03f, 1.1f);
				}
            }
        }
        GameManager.instance.LoadScene(0);
    }
}
