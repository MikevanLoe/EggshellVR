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

		//If the current input is loud enough, increase the score
		ScoreSystem (zin);
	}


	public void ScoreSystem(Sentence spoken)
	{
		if (micIn > ((maxMicIn + minMicIn) / 2) && !(startTime > 0.0f))
		{
			startTime = Time.time;
		}
		if (micIn < ((maxMicIn + minMicIn) / 2) && startTime > 0.0f)
		{
			endTime = Time.time;
			elapsed = endTime - startTime;

			if (elapsed > spoken.time)
			{
				// Calculate point reduction for speaking too slow
				float pointReduction = (( elapsed / spoken.time ) * 100 ) - 100;

				// Apply the score and its reduction
				score += 100 - pointReduction;
			}
			else if (elapsed < spoken.time)
			{
				// Calculate the percentage of points you get for speaking too fast
				float pointPercentage = ( elapsed / spoken.time );
				
				// Apply the score and its reduction
				score += 100 * pointPercentage;
			}
			else
			{
				// Apply the maximum score
				score += 100;
			}
		}
	}
}
	