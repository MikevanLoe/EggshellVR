using UnityEngine;
using System.Collections;

public class EndText : MonoBehaviour {
	public Transform fishGame;
	
	void Update()
	{
		if (Input.GetButtonDown ("Fire2")) {
			fishGame.gameObject.SetActive (false);
			this.gameObject.SetActive(false);
		}
	}
}
