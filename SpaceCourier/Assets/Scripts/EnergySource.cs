using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySource : MonoBehaviour {

    private List<Powered> poweredObjects;
    [SerializeField] private Rigidbody RB; // used for autosetting luminity
    [SerializeField] private SphereCollider triggerSphere;
    [SerializeField] private float energySphereRadiusMultiplier;
    [SerializeField] private float luminity;
    [SerializeField] private float energyMultiplier;

    void Awake() {

        poweredObjects = new List<Powered>();

        if (triggerSphere == null) {

            Debug.LogWarning("This " + gameObject.name + " has no collider.");
            return;
        }

        SetLuminity();
        SetEnergySphere();
    }

	private void OnTriggerEnter(Collider other) {

        Powered powered;

        if (other.TryGetComponent<Powered>(out powered) && !powered.disabled)
            poweredObjects.Add(powered);
	}

	private void OnTriggerExit(Collider other) {

        Powered powered;

        if (other.TryGetComponent<Powered>(out powered))
            poweredObjects.Remove(powered);
	}

    private void OnDrawGizmos() {

        SetLuminity();
        SetEnergySphere();
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, triggerSphere.radius * transform.localScale.x);
    }

    private void SetLuminity() {
       
        // Get luminity from mass
        if (RB != null)
            luminity = 1 + Mathf.Log(RB.mass);
    }

    private void SetEnergySphere() {

        triggerSphere.radius = luminity * energySphereRadiusMultiplier;
        triggerSphere.isTrigger = true;
    }

    private void Update() {

        foreach (Powered powered in poweredObjects) {

            if (powered.disabled)
                continue;

            float dist = Vector3.Distance(transform.position, powered.transform.position);
            float eForce = luminity / dist;

            powered.AddEnergy(Mathf.CeilToInt(eForce * energyMultiplier * Time.deltaTime));
        }
    }
}
