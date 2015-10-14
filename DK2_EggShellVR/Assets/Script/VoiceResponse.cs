using UnityEngine;
using System.Collections;

public class VoiceResponse : MonoBehaviour
{
	public float score;

	private float micIn, maxMicIn, minMicIn, startTime, endTime, elapsed;
	private Sentence zin = new Sentence("Test zin", 5.0f);

	void Update()
	{
		//Calculate the highest and lowest input
		maxMicIn = Mathf.Max(maxMicIn, micIn);
		minMicIn = Mathf.Min (minMicIn, micIn);

		//
		ScoreSystem (zin);
	}


	public void ScoreSystem(Sentence spoken)
	{
		// If the current input is loud enough, start the timer
		if (micIn > ((maxMicIn + minMicIn) / 2) && !(startTime > 0.0f))
		{
			startTime = Time.time;
		}

		// If the current input is to soft, stop the timer and give the points
		if (micIn < ((maxMicIn + minMicIn) / 2) && startTime > 0.0f)
		{
			endTime = Time.time;
			elapsed = endTime - startTime;

			// Calculate the point reduction for speaking to fast or slow
			float pointReduction = Mathf.Abs( ( (1/spoken.time) * elapsed ) - 1);

			// Give the player the points he earned
			score += 100 * (1-pointReduction);

			// Reset Timer
			startTime = 0.0f;
		}
	}
}
	