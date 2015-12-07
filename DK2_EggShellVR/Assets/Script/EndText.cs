using UnityEngine;
using System.Collections;

public class EndText : MonoBehaviour {
	public Transform fishGame;
	
	void Update()
	{
		if (Input.GetButtonDown ("Fire2")) {
			fishGame.GetComponent<FishingMinigame>().ActivateRod();
			fishGame.gameObject.SetActive (false);
			this.gameObject.SetActive(false);
		}
	}
}
