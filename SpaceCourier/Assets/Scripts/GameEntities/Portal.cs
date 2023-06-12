using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

    [SerializeField] private SphereCollider triggerSphere;
    [SerializeField] private SpriteRenderer minimapSprite;
    [SerializeField] private Sprite openedSprite;
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Ship shipPrefab;
    [SerializeField] private ParticleSystem particles;

    private bool portalClosed = true;
    private Ship ship;
    private Coroutine endLevelCoroutine;

    public static Action<string, int> OnLevelEnd;

    private void Start() {

        CreateShip();
        EnablePortal(false);

        ScenesManager.instance.lockScene = false;
    }

    private void CreateShip() {
        
        ship = Instantiate(shipPrefab, transform);
        ship.transform.position = transform.position;
        ship.transform.rotation = transform.rotation;
        Controller.instance.SetShip(ship);
    }

	private void Update() {

        if (ship != null && ship.loadCount == 0 && portalClosed)
            EnablePortal(true);
	}

	private void OnTriggerEnter(Collider other) {

        Ship ship;

        if (other.TryGetComponent<Ship>(out ship) && !ship.dead && ship.loadCount == 0 && !portalClosed) {

            Rigidbody SRB = ship.GetComponent<Rigidbody>();
            SRB.velocity = Vector3.zero;
            SRB.isKinematic = true;

            if (endLevelCoroutine == null)
                endLevelCoroutine = StartCoroutine(EndLevel(ship));
        }
    }

    private void EnablePortal(bool status) {

        portalClosed = !status;
        triggerSphere.enabled = status;

        if (status) {
            minimapSprite.sprite = openedSprite;
            particles.Play();
        } else {
            minimapSprite.sprite = closedSprite;
            particles.Stop();
        }
    }

    private IEnumerator EndLevel(Ship ship) {

        GameManager.instance.SetCompletedLevel();
        int counter = 0;

        List<MeshRenderer> truckPieces = new List<MeshRenderer>();

        if (!PlayerData.achievements.Find(x => x.id == "rotate").unlocked)
            OnLevelEnd?.Invoke("Rotation", ship.Rotations);

        // Increase hardcore level amount only if we have no count (0) or we're on the next level of the progression.
        if (!PlayerData.achievements.Find(x => x.id == "safety").unlocked)
            if (!PlayerData.statistics.ContainsKey("LevelHardcore") || ScenesManager.instance.GetCurrentScene().buildIndex - 1 == PlayerData.statistics["LevelHardcore"].value)
                OnLevelEnd?.Invoke("LevelHardcore", 1);

        if (ship != null) {

            ship.deathDisabled = true;
            ship.DisableShip();
            truckPieces.AddRange(ship.truckModel.GetComponentsInChildren<MeshRenderer>());

            if (!PlayerData.achievements.Find(x => x.id == "discharge").unlocked && ship.GetComponent<Powered>().Energy == 0)
                OnLevelEnd?.Invoke("AchievementFullDischarge", 1);

            if (!PlayerData.achievements.Find(x => x.id == "ready").unlocked && ship.GetComponent<Powered>().Energy * 1f / ship.GetComponent<Powered>().MaxEnergy * 1f > 0.95f)
                OnLevelEnd?.Invoke("AchievementReadyForAnother", 1);
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

        GameManager.instance.LoadLevel(GameManager.instance.levels[0]);
        StopCoroutine(endLevelCoroutine);
    }
}
