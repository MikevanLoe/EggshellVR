using UnityEngine;
using System.Collections;

public class ActivationZone : MonoBehaviour {
	public Transform startText;
	
	void OnTriggerEnter()
	{
		if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().HasItem("Hengel")) {
			startText.gameObject.SetActive(true);
			startText.GetComponent<StartText>().inZone = true;
		}
	}

	void OnTriggerExit()
	{
		if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().HasItem("Hengel"))
			startText.GetComponent<StartText>().inZone = false;
	}
}
