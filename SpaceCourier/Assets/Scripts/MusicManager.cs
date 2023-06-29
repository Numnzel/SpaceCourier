using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

	public AudioSource audioSource;
	public AudioListSO audioList;
	public bool isPaused = false;

	private int trackIndex = 0;
	private Coroutine musicCoroutine;
	private float elapsedTime = 0;

	public static Action<string, int> OnTrackEnd;


	private List<string> tracksListened = new List<string>();

	private void Start() {

		StartTracks();

		// Make a list of listened tracks from player data
		if (!GameDataManager.gameData.achievements.Contains("audio"))
			foreach (AudioClip audioTrack in audioList.audioList) {

				string key = "TrackListen" + audioSource.clip.name;

				if (GameDataManager.gameData.statistics.ContainsKey(key) && GameDataManager.gameData.statistics[key].value > 0)
					tracksListened.Add(key);
			}
	}

	private IEnumerator PlayTracks() {

		if (audioList != null && audioList.audioList.Length > 0)
			PlayTrack(trackIndex);

		while (audioList != null && audioList.audioList.Length > 0) {

			if (elapsedTime > audioSource.clip.length) {

				// Give listen stat if track was not listened and music volume is not zero
				if (!tracksListened.Contains("TrackListen" + audioSource.clip.name) && audioSource.volume > 0) {

					OnTrackEnd?.Invoke("TrackListen", 1);
					OnTrackEnd?.Invoke("TrackListen" + audioSource.clip.name, 1);
				}
				
				PlayNextTrack();
			}

			elapsedTime += Time.deltaTime;

			yield return new WaitUntil(() => { return !isPaused; });
		}
	}

	public void PlayTrack(int index) {

		ChangeTrack(index);
		audioSource.Play();
	}

	public void PlayNextTrack() {

		PlayTrack(trackIndex + 1);
	}

	public void StartTracks() {

		if (musicCoroutine == null)
			musicCoroutine = StartCoroutine("PlayTracks");
	}

	public void StopTracks() {

		if (audioSource.isPlaying)
			audioSource.Stop();

		if (musicCoroutine != null)
			StopCoroutine(musicCoroutine);
	}

	public void PauseTrack(bool pause) {

		isPaused = pause;

		if (pause) {

			if (audioSource.isPlaying)
				audioSource.Pause();
		}
		else
			audioSource.Play();
	}

	private void ChangeTrack(int index) {

		trackIndex = index % audioList.audioList.Length;
		audioSource.clip = audioList.audioList[trackIndex];
		elapsedTime = 0;
	}
}
