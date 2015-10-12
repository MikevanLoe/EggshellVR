using UnityEngine;
using System.Collections;

public class VoiceResponse : MonoBehaviour {
	// Volume tussen 0 en 1
	// Punten gebaseerd op jou tijd en de 'stocktijd'

	private float micIn;
	private float maxMicIn;
	private float minMicIn;
	private float score;

	void Update()
	{
		maxMicIn = Mathf.Max(maxMicIn, micIn);
		minMicIn = Mathf.Min (minMicIn, micIn);

		if (micIn > ( (maxMicIn + minMicIn) / 2))
		{
			score += 10;
		}
	}
}
