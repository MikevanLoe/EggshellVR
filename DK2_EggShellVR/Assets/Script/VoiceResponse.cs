using UnityEngine;
using System.Collections;

public class VoiceResponse : MonoBehaviour {

	private float micIn;	
	private float maxMicIn;
	private float minMicIn;
	private float score;

	void Update()
	{
		//Calculate the highest and lowest input
		maxMicIn = Mathf.Max(maxMicIn, micIn);
		minMicIn = Mathf.Min (minMicIn, micIn);

		//If the current input is loud enough, increase the score
		if (micIn > ( (maxMicIn + minMicIn) / 2))
		{
			score += 10;
		}
	}
}
