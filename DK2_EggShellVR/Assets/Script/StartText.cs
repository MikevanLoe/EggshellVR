using UnityEngine;
using System.Collections;

public class StartText : MonoBehaviour {
	public Transform fishGame;
	public Transform endText;
	public bool inZone;

	void Update()
	{
		if (Input.GetButtonDown ("Fire2") && inZone)
		{
			fishGame.gameObject.SetActive (true);
			fishGame.GetComponent<FishingMinigame>().ActivateRod();
			endText.gameObject.SetActive(true);
		}
	}
}
