using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySource : MonoBehaviour {

    private List<Powered> PoweredUnits;
    [SerializeField] private Rigidbody RB; // used for autosetting luminity
    [SerializeField] private SphereCollider triggerSphere;
    [SerializeField] private float areaMultiplier;
    [SerializeField] private float luminity;
    [SerializeField] private float luminityMultiplier;

    void Awake() {

        PoweredUnits = new List<Powered>();

        if (triggerSphere == null) {

            Debug.LogWarning("This " + gameObject.name + " has no collider.");
            return;
        }

        // Get luminity from mass
        if (RB != null)
            luminity = RB.mass;

        triggerSphere.radius = (luminity * areaMultiplier) / transform.localScale.x;
        triggerSphere.isTrigger = true;
    }

	private void OnTriggerEnter(Collider other) {

        Powered powered;

        if (other.TryGetComponent<Powered>(out powered) && !powered.disabled)
            PoweredUnits.Add(powered);
	}

	private void OnTriggerExit(Collider other) {

        Powered powered;

        if (other.TryGetComponent<Powered>(out powered))
            PoweredUnits.Remove(powered);
	}

    private void OnDrawGizmos() {

        // Get luminity from mass
        if (RB != null)
            luminity = RB.mass;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, luminity * areaMultiplier);
    }

    void Update() {

        foreach (Powered powered in PoweredUnits) {

            if (powered.disabled)
                continue;

            float dist = Vector3.Distance(transform.position, powered.transform.position);
            float eForce = luminity / dist;

            powered.AddEnergy(Mathf.CeilToInt(eForce * luminityMultiplier * Time.deltaTime));
        }
    }
}
