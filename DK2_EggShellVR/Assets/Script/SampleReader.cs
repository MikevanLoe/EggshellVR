using UnityEngine;
using System.Collections;

public class SampleReader : MonoBehaviour {
	public TextMesh tm;

	private MicMaster mm;
	// Use this for initialization
	void Start () {
		mm = new MicMaster ();
		mm.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		tm.text = mm.MicGain.ToString ();
	}
}
