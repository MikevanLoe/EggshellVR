using UnityEngine;
using System.Collections;

public class ActivationZone : MonoBehaviour {
	public Transform fishingGame;

	void OnTriggerEnter()
	{
		// Word melding enter ipv game
		fishingGame.gameObject.SetActive (true);
	}

	void OnTriggerExit()
	{
		// Word melding exit aan ipv game uit
		fishingGame.gameObject.SetActive (false);
	}
}
