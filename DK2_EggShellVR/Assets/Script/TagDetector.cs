using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TagDetector : MonoBehaviour {
	public string Tag;
	public List<GameObject> Listeners;
	
	void OnTriggerEnter(Collider other) {
		if (other.tag == Tag) {
			Listeners.ForEach(l => l.SendMessage("DetectOn", SendMessageOptions.DontRequireReceiver));
		}
	}
	
	void OnTriggerExit(Collider other) {
		if (other.tag == Tag) {
			Listeners.ForEach(l => l.SendMessage("DetectOff", SendMessageOptions.DontRequireReceiver));
		}
	}
}
