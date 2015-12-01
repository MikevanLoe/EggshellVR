using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class VoiceResponse : MonoBehaviour
{
	const float DurationMinimum = 0.4f;
	const float DurationMaximum = 20;

	private int _samplesPerSecond;
	private int _offset;
	private bool _speaking;
	private float _low = 0;
	private float _high = 1;
	private float _highStart;
	private float _recordDelay;
	private Action<float> _alertListener;
	private bool _init = true;
	
	public bool Listening 
	{
		get;
		private set;
	}


	void Start()
	{
		//The avarage array has to fit a sample every fixed update.
		_samplesPerSecond = (int) Mathf.Ceil (1 / Time.fixedDeltaTime);
		MicrophoneInput.Init ();
	}

	void FixedUpdate()
	{
		if (!Listening)
			return;
		float input = MicrophoneInput.GetInputAvg ();
		if (Input.GetKey (KeyCode.Z))
			input = 1;

		if (_recordDelay > Time.time)
			return;
		if (input < 0.0001f)
			return;
		//Microphone input calibration to detect the difference between silence and speech
		if (_init) 
		{
			//Initial max and min are the very first recorded input
			_high = input;
			_low = input;
			_init = false;
		}
		//But high and low can't be the same, else the code will break...
		if (_high == _low) 
		{
			_high += _low / 10;
		}

		//The limit changes relative to the difference between high and low
		float LimitRestoreRate = (float) (_high - _low) * 0.001f;
		float LimitRaiseMax = LimitRestoreRate / Time.fixedDeltaTime;
		//The limits approach input to avoid freak peaks that ruin calibration
		if (input > _high)
			_high = Mathf.Min (_high + LimitRaiseMax, input);
		else
			_high = Mathf.Max(_high - LimitRestoreRate, input);
		
		if (input < _low)
			_low = Mathf.Max (_low - LimitRaiseMax, input);
		else
			_low = Mathf.Min (_low + LimitRestoreRate, input);

		//If the input is somewhere near the max, assume the player is speaking
		if (input > _high - (_high - _low) / 4) 
		{
			if(!_speaking)
			{
				_speaking = true;
				_highStart = Time.time;					//record the moment the player started speaking
			}
		}
		else if (_speaking) 							//If the player stopped speaking
		{
			_speaking = false;
			float duration = Time.time - _highStart;	//See how long the player was speaking
			//If the speech was too short, assume it was noise and reset
			if (duration < DurationMinimum)
			{
				_highStart = Time.time;
				return;
			}
			//Stop listening when finished
			MicrophoneInput.Stop ();
			Listening = false;
			
			//Alert the listener after clearing the action
			Action<float> action = _alertListener;
			_alertListener = null;
			action(duration);
		}
	}

	public void ForceStop()
	{
		float duration = Time.time - _highStart;	//See how long the player was speaking
		if (!_speaking)
			duration = 0;
		_speaking = false;

		//Stop listening when finished
		MicrophoneInput.Stop ();
		Listening = false;
		
		//Alert the listener after clearing the action
		Action<float> action = _alertListener;
		_alertListener = null;
		action(duration);
	}

	public void StartListening(Action<float> method)
	{
		if (Listening)
			throw new UnityException ("StartListening called when already listening!");
		_highStart = Time.time;
		_recordDelay = Time.time + 0.1f;
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

	public bool IsSpeaking()
	{
		return _speaking;
	}
}
	