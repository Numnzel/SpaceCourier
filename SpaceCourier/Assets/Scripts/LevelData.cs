using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData {

	public int id;
	public float time;

	public LevelData(int id, float time) {

		this.id = id;
		this.time = time;
	}
}