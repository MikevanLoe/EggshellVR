using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {
	private GameObject Cam;
	void Start () {
		Cam = GameObject.FindGameObjectWithTag ("MainCamera");
	}

	void Update () {
		transform.rotation = Cam.transform.rotation;
	}
}
