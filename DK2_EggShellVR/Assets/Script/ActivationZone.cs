using UnityEngine;
using System.Collections;

public class ActivationZone : MonoBehaviour {
	public GameObject FishingRod;
	public FishingMinigame FishingGame;
	public string Tag;

	private bool Near;

	void Update()
	{
		if (!Near)
			return;
		if (Input.GetButtonDown ("Fire2")) {
			if(!FishingRod.activeSelf)
			{
				FishingGame.ActivateRod();
			}
			else{
				FishingGame.ActivateRod();
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag != Tag)
			return;
		if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().HasItem("Hengel")) {
			Near = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag != Tag)
			return;
		if (GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ().HasItem ("Hengel")) {
			Near = false;
		}
	}
}
