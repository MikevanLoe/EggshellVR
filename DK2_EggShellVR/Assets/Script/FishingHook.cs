using UnityEngine;
using System.Collections;

public class FishingHook : MonoBehaviour {
	private float OffTime;
	void Update()
	{
		if(Input.GetButtonDown("Fire1"))
	   	{
			OffTime = Time.time + 1f;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (Time.time < OffTime)
			return;
		other.gameObject.GetComponent<FishAI> ().Hooked();
		//Hook
		other.transform.parent = transform;
	}
}
