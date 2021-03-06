using System;
using UnityEngine;

public class MicrophoneInput
{
	private static AudioClip _micClip;
	private static int _sampleRateMin;
	private static float[] _samples;
	private static string _device;

	//Initialize a microphone device
	public static void Init(string device = "")
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
		
		_device = device;
	}
	
	//Start reording from the set microphone
	public static void Start()
	{
		if (Microphone.IsRecording (_device))
			throw new UnityException ("Microphone already recording somewhere else!");
		_micClip = Microphone.Start (_device, true, 1, _sampleRateMin);
		_samples = new float[_micClip.samples * _micClip.channels];
	}

	public static void Stop()
	{
		Microphone.End (_device);
	}

	//Stop the recording on quit
	void OnApplicationQuit() 
	{
		Microphone.End (_device);
	}

	//Returns the avg feed from the microphone over the last second
	public static float GetInputAvg()
	{
		//Store the audio clip in a float array
		_micClip.GetData (_samples, Microphone.GetPosition(""));
		
		//Get the avarage value of all samples in the current recording
		int count = 0;
		float avg = 0f;
		for (int i = Microphone.GetPosition(""); count < _micClip.samples; i++) 
		{
			i %= _micClip.samples;
			count++;
			if(_samples[i] == 0)
				break;
			//Add the absolute of the current value to the avarage calculation
			avg = avg + (Math.Abs(_samples[i]) - avg) / count;
		}
		return avg;
	}
	
	//Returns the avg feed from the microphone over the last tenth of a second
	public static float GetInput()
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

	public static void GetSamples(out float[] Samples)
	{
		Samples = new float[_micClip.samples * _micClip.channels];
		_micClip.GetData (Samples, 0);
	}
}