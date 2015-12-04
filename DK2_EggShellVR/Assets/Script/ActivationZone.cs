using UnityEngine;
using System.Collections;

public class ActivationZone : MonoBehaviour {
	public Transform fishingGame;

	void OnTriggerEnter()
	{
		fishingGame.gameObject.SetActive (true);
	}

	void OnTriggerExit()
	{
		fishingGame.gameObject.SetActive (false);
	}
}
