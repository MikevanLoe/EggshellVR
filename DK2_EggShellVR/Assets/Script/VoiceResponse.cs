using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class VoiceResponse : MonoBehaviour
{
	const float DurationMinimum = 0.4f;
	const float DurationMaximum = 20;
	const float MicInMin = 0.15f;

	public PresentationController presentation;
	public GameObject ErrorMessage;

	private int _samplesPerSecond;
	private float[] _sampleAvg;
	private int _offset;
	private bool _high;
	private float _highStart;
	private Action<float> _alertListener;
	
	public bool Listening 
	{
		get;
		private set;
	}


	void Start()
	{
		//The avarage array has to fit a sample every fixed update.
		_samplesPerSecond = (int) Mathf.Ceil (1 / Time.fixedDeltaTime);
		_sampleAvg = new float[_samplesPerSecond];
		MicrophoneInput.Init ();
	}

	void FixedUpdate()
	{
		if (!Listening)
			return;
		float input = MicrophoneInput.GetInputAvg ();
		_sampleAvg [_offset] = input;
		_offset++;
		if(_offset >= _samplesPerSecond)
			_offset = 0;

		//Store the avarage input volume over a short while
		int count = 0;
		float avg = 0f;
		for (int i = 0; i < _sampleAvg.Length; i++) 
		{
			count++;
			float sample = _sampleAvg[i];
			//If the current sample is 0, it's unassigned to and so is everything after it so we can stop
			if(sample == 0)
				break;
			//Add the absolute of the current value to the avarage calculation
			avg = avg + (sample - avg) / count;
		}
		if (input > avg * 2) 								//Detect a raise
		{								
			_highStart = Time.time; 						//record the moment of the raise
		} 
		else if (input < avg / 2) 							//Detect a fall
		{							
			float duration = Time.time - _highStart;		//See how long the player was speaking
			//If the speech was too short, assume it was noise and reset
			if (duration < DurationMinimum)
			{
				_highStart = Time.time;
				return;
			}
			
			MicrophoneInput.Stop ();						//Stop listening when finished
			Listening = false;					
			_sampleAvg = new float[_samplesPerSecond];		//Reset the avarage
			_offset = 0;
			Action<float> a = _alertListener;
			_alertListener = null;
			a(duration);
			//_alertListener(duration);						//Alert the listener
		} 
		else if (Time.time - _highStart > DurationMaximum) 
		{ 													//If the raise time is longer than the maximum
			ErrorMessage.SetActive(true);					//Display an error
		}
	}

	public void StartListening(Action<float> method)
	{
		if (Listening)
			throw new UnityException ("StartListening called when already listening!");
		_highStart = Time.time;
		MicrophoneInput.Start ();
		_alertListener = method;
		Listening = true;
	}

	public void StopListening()
	{
		MicrophoneInput.Stop ();
		Listening = false;
		_alertListener = null;
	}

	public void SetSentence(Sentence s)
	{
		return;
	}
}
	