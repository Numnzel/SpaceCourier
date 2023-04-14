using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unloader : MonoBehaviour {

    [SerializeField] private SphereCollider triggerSphere;
    [SerializeField] private SpriteRenderer minimapSprite;
    [SerializeField] private Sprite completedSprite;
    private bool satisfied = false;

    private void OnTriggerEnter(Collider other) {

        Ship ship;

        if (other.TryGetComponent<Ship>(out ship) && !ship.dead && ship.loadCount > 0 && !satisfied) {

            minimapSprite.sprite = completedSprite;
            triggerSphere.enabled = false;
            satisfied = true;
            ship.Unload();
        }
    }
}
