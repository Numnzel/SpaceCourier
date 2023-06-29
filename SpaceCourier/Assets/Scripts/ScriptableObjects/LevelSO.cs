using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Data/Level", fileName = "Level")]
public class LevelSO : ScriptableObject {

	public int sceneIndex;
	public int progressRequeriment;
	public string episode;
	public string title;
	public Texture2D backgroundPlaneTexture;
	public Texture2D parallaxPlaneTexture;
	public LightingSettings lightingSettings;
	public VolumeProfile volumeProfile;
	public Vector2 rankTimes = new Vector2();
}
