using UnityEngine;
using System.Collections;

public class StartText : MonoBehaviour {
	public Transform fishGame;
	public Transform hookText;
	public Transform endText;
	public bool inZone;

	void Update()
	{
		if (Input.GetButtonDown ("Fire2") && inZone)
		{
			fishGame.gameObject.SetActive (true);
			fishGame.GetComponent<FishingMinigame>().ActivateRod();
			hookText.gameObject.SetActive(true);
			endText.gameObject.SetActive(true);
		}
	}
}
