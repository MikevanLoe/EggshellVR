using System;
using UnityEngine;

public class MicrophoneInput
{
	private AudioClip _micClip;
	private int _sampleRateMin;
	private float[] _samples;
	private string _device;

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
		_device = device;
		_micClip = Microphone.Start (_device, true, 1, _sampleRateMin);
		_samples = new float[_micClip.samples * _micClip.channels];
	}

	//Stop the recording on quit
	void OnApplicationQuit() 
	{
		Microphone.End (_device);
	}

	//Returns the avg feed from the microphone over the last second
	public float GetInputAvg()
	{
		//Store the audio clip in a float array
		_micClip.GetData (_samples, Microphone.GetPosition(""));
		
		//Get the avarage value of all samples in the current recording
		int count = 0;
		float avg = 0f;
		for (int i = 0; i < _micClip.samples; i++) 
		{
			count++;
			//Add the absolute of the current value to the avarage calculation
			avg = avg + (Math.Abs(_samples[i]) - avg) / count;
		}
		return avg;
	}
	
	//Returns the avg feed from the microphone over the last tenth of a second
	public float GetInput()
	{
		//Store the audio clip in a float array
		int offset = Microphone.GetPosition ("");
		_micClip.GetData (_samples, Microphone.GetPosition (""));
		
		//Get the avarage value of all samples in the current recording
		int count = 0;
		float avg = 0f;
		for (int i = offset - (int) (_micClip.samples * 0.1f); i <= offset; i++) 
		{
			int index = (i + _micClip.samples) % _micClip.samples;
			count++;
			//Add the absolute of the current value to the avarage calculation
			avg = avg + (Math.Abs(_samples[index]) - avg) / count;
		}
		return avg;
	}

	public void GetSamples(out float[] Samples)
	{
		Samples = new float[_micClip.samples * _micClip.channels];
		_micClip.GetData (Samples, 0);
	}
}