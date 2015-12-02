using UnityEngine;
using System.Collections;

public class FishingHook : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		other.gameObject.GetComponent<FishAI> ().Hooked();
	}
}
