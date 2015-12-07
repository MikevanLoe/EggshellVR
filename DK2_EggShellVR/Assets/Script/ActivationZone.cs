using UnityEngine;
using System.Collections;

public class ActivationZone : MonoBehaviour {
	public Transform startText;
	
	void OnTriggerEnter()
	{
		startText.gameObject.SetActive(true);
	}

	void OnTriggerExit()
	{
		startText.gameObject.SetActive(false);
	}
}
