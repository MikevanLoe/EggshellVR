using UnityEngine;
using System.Collections;

public class SpeechIndicator : MonoBehaviour {
	public Material indicator;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void InputOn()
	{
		indicator.color = Color.green;
	}

	public void InputOff()
	{
		indicator.color = Color.red;
	}
}
