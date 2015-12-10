using UnityEngine;
using System.Collections;

public class ActivationZone : MonoBehaviour {
	public Transform startText;
	
	void OnTriggerEnter()
	{
		startText.gameObject.SetActive(true);
		startText.GetComponent<StartText>().inZone = true;
	}

	void OnTriggerExit()
	{
		startText.GetComponent<StartText>().inZone = false;
	}
}
