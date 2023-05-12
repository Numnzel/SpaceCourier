using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveArrows : MonoBehaviour {

	[SerializeField] private ObjectiveArrow arrowPrefab;
	public List<ObjectiveArrow> arrows = new List<ObjectiveArrow>();
	
	public ObjectiveArrow CreateArrow(GameObject objective) {

		ObjectiveArrow arrow = Instantiate(arrowPrefab, transform);
		arrow.objective = objective;
		arrows.Add(arrow);

		return arrow;
	}
}
