using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/AudioList", fileName = "AudioList")]
public class AudioListSO : ScriptableObject {

	public AudioClip[] audioList;
}
