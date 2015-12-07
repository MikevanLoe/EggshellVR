﻿using UnityEngine;
using System.Collections;

public class StartText : MonoBehaviour {
	public Transform fishGame;
	public Transform endText;

	void Update()
	{
		if (Input.GetButtonDown ("Fire2")) {
			fishGame.gameObject.SetActive (true);
			fishGame.GetComponent<FishingMinigame>().ActivateRod();
			endText.gameObject.SetActive(true);
			this.gameObject.SetActive(false);
		}
	}
}
