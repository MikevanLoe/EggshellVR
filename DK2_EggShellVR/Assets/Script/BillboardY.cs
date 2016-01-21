using UnityEngine;
using System.Collections;

public class BillboardY : MonoBehaviour {
	void Update () {
		transform.Rotate(0, 0, transform.rotation.eulerAngles.y - 180 - Camera.main.transform.rotation.eulerAngles.y);
	}
}
