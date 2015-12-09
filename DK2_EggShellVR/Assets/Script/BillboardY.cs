using UnityEngine;
using System.Collections;

public class BillboardY : MonoBehaviour {

	private Camera Cam;

	void Update () {
		if (Cam == null || !Cam.enabled)
			Cam = Camera.main;
		else {
			transform.Rotate(0, 0, transform.rotation.eulerAngles.y - 180 - Cam.transform.rotation.eulerAngles.y);
		}
	}
}
