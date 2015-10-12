using UnityEngine;
using System.Collections;

public class VoiceResponse : MonoBehaviour {
	// Volume tussen 0 en 1
	// Punten gebaseerd op jou tijd en de 'stocktijd'

	private var micIn;
	private var maxMicIn;
	private int score;

	void Update()
	{
		maxMicIn = Mathf.Max(maxMicIn, micIn);

		if (micIn > maxMicIn/2)
		{
			score += 10;
		}
	}
}
