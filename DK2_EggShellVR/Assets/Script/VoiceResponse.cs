using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoiceResponse : MonoBehaviour
{
	const float DurationMinimum = 0.334f;
	const float DurationMaximum = 20;
	const float MicInMin = 0.15f;

	public PresentationController presentation;
	public GameObject ErrorMessage;

	private MicrophoneInput micInput;

	private int _samplesPerSecond;
	private float[] _sampleAvg;
	private int _offset;
	private bool _high;
	private float _highStart;
	
	public bool Listening 
	{
		get;
		private set;
	}


	void Start()
	{
		micInput = new MicrophoneInput();
		micInput.Start ();
		_samplesPerSecond = (int) Mathf.Ceil (1 / Time.fixedDeltaTime); 
		_sampleAvg = new float[_samplesPerSecond];
	}

	void FixedUpdate()
	{
		if (!Listening)
			return;
		float input = micInput.GetInputAvg ();
		_sampleAvg [_offset] = input;
		_offset++;
		_offset %= _samplesPerSecond;
		//Store the avarage input volume over a short while
		int count = 0;
		float avg = 0f;
		for (int i = 0; i < _sampleAvg.Length; i++) 
		{
			count++;
			//If the current sample is 0, it's unassigned to and so is everything after it
			float sample = _sampleAvg[i];
			if(sample == 0)
				break;
			//Add the absolute of the current value to the avarage calculation
			avg = avg + (sample - avg) / count;
		}
		if (input > avg * 2) {
			_highStart = Time.time;
		} else if (input < avg / 2 && input > MicInMin) {
			float duration = Time.time - _highStart;
			if (duration < DurationMinimum)
				return;

			Listening = false;
			// Give the player the points he earned
			presentation.SentenceSpoken (duration);
		} else if (Time.time - _highStart > DurationMaximum) {
			ErrorMessage.SetActive(true);
		}
	}

	public void StartListening()
	{
		_highStart = Time.time;
		Listening = true;
	}

	//TODO: Will I ever even need this? If I read this one distant future day and the answer is no I'll know what to do.
	public bool IsSpeaking()
	{
		return false;
	}

	public void SetSentence(Sentence s)
	{
		return;
	}
}
	