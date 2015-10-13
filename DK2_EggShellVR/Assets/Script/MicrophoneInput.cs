using System;
using UnityEngine;
public class MicrophoneInput
{
	private AudioClip _micClip;
	private int _sampleRateMin;

	public MicrophoneInput()
	{
		//Get the microphone sample rate
		int max;
		Microphone.GetDeviceCaps (Microphone.devices[0], out _sampleRateMin, out max);
		//If the sample rate is 0, that means it supports any rate. 
		//In this case, set it to the default 44100.
		if (_sampleRateMin == 0) 
		{
			_sampleRateMin = 44100;
		}
	}
	
	//Start reording from the default microphone
	public void Start(string device = "")
	{
		_micClip = Microphone.Start (device, true, 1, _sampleRateMin);
	}

	//Returns the avg feed from the microphone over the last second
	public float GetInputAvg()
	{
		//Store the audio clip in a float array
		float[] samples = new float[_micClip.samples * _micClip.channels];
		_micClip.GetData (samples, 0);
		
		//Get the avarage value of all samples in the current recording
		int count = 0;
		float avg = 0f;
		for (int i = 0; i < _micClip.samples; i++) 
		{
			count++;
			//Add the absolute of the current value to the avarage calculation
			avg = avg + (Math.Abs(samples[i]) - avg) / count;
		}
		return avg;
	}
}