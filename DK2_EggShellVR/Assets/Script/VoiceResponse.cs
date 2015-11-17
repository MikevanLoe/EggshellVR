using UnityEngine;
using System.Collections;

public class VoiceResponse : MonoBehaviour
{
	public PresentationController presentation;

	private float micIn, maxMicIn, minMicIn, startTime, endTime, elapsed;
	private MicrophoneInput micInput;
	private Sentence _curSentence;

	public SpeechIndicator si;

	void Start()
	{
		micInput = new MicrophoneInput();
		micInput.Start ();
	}

	void Update()
	{
		micIn = micInput.GetInputAvg ();

		// Calculate the highest and lowest input
		maxMicIn = Mathf.Max (maxMicIn, micIn);
		minMicIn = Mathf.Min (minMicIn, micIn);

		// Start the timer and give points to the player
		ScoreSystem (_curSentence);
	}


	public void ScoreSystem(Sentence spoken)
	{
		if (micIn > ((maxMicIn + minMicIn) / 2))
			si.InputOn ();
		else
			si.InputOff ();
		if (_curSentence == null)
			return;

		// If the current input is loud enough, start the timer
		if (micIn > ((maxMicIn + minMicIn) / 2) && !(startTime > 0.0f))
		{
			startTime = Time.time;
		}

		// If the current input is too soft, stop the timer and give the points
		if (micIn < ((maxMicIn + minMicIn) / 2) && startTime > 0.0f)
		{
			endTime = Time.time;
			elapsed = endTime - startTime;

			// Calculate the point reduction for speaking to fast or slow
			float delta = Mathf.Abs(spoken.Time - elapsed);
			float deltaFactor = Mathf.Clamp01(1 / spoken.Time * delta);

			// Give the player the points he earned
			presentation.SentenceSpoken(deltaFactor);

			// Reset Timer
			startTime = 0.0f;
		}
	}

	public void SetSentence(Sentence s)
	{
		_curSentence = s;
	}
}
	