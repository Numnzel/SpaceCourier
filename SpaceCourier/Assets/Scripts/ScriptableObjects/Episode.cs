using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Data/Episode", fileName = "Episode")]
public class Episode : ScriptableObject {

	public int id;
	public string title;
}
