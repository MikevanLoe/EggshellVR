using UnityEngine;
using System.Collections;

public class AudioTester : MonoBehaviour {
	public bool TypeA;

	private MicrophoneInput micInput;
	private float OriginalScale;
	
	
	void Start()
	{
		micInput = new MicrophoneInput();
		micInput.Start ();
		OriginalScale = transform.localScale.y;
	}
	
	void Update()
	{
		float input = 0;
		if (TypeA) {
			input = micInput.GetInput ();
		} else
			input = micInput.GetInputAvg ();
		var newScale = transform.localScale;
		newScale.y = OriginalScale * input;
		transform.localScale = newScale;
	}
}
