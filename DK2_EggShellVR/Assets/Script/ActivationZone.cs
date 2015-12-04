using UnityEngine;
using System.Collections;

public class ActivationZone : MonoBehaviour {
	public Transform fishingGame;

	void OnTriggerEnter(Collider other)
	{
		fishingGame.gameObject.SetActive (true);
	}
}
