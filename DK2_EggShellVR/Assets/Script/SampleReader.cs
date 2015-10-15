using UnityEngine;
using System.Collections;

public class SampleReader : MonoBehaviour {
	public TextMesh tm;

	private MicrophoneInput mi;
	// Use this for initialization
	void Start () {
		mi = new MicrophoneInput();
		mi.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		tm.text = Mathf.Round (mi.GetInputAvg() * 10).ToString ();
	}
}
