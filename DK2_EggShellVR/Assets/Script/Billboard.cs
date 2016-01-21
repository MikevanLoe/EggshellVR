using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {
	public Vector3 spin;

	void Update () {
		transform.LookAt (Camera.main.transform);
		transform.Rotate (spin);
	}
}
