using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {

	public TextMeshProUGUI textTimeVal;
	public TextMeshProUGUI textNextVal;
	[SerializeField] private Image imgStatus;
	[SerializeField] private Sprite sprLock;
	[SerializeField] private Sprite sprRankGold;
	[SerializeField] private Sprite sprRankSilver;
	[SerializeField] private Sprite sprRankBronze;
	[SerializeField] private Sprite sprRankNone;

	public void SetLock(bool value) {

		if (value)
			imgStatus.sprite = sprLock;
		else
			imgStatus.sprite = sprRankNone;
	}

	public void SetRank(int rank) {

		switch (rank) {

			case 3:
				imgStatus.sprite = sprRankGold;
				break;
			case 2:
				imgStatus.sprite = sprRankSilver;
				break;
			case 1:
				imgStatus.sprite = sprRankBronze;
				break;
			default:
				imgStatus.sprite = sprRankNone;
				break;
		}
	}
}
