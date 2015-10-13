using System;
using NAudio.Wave;
public class MicMaster
{
	private float _micGain;

	private MicInput _micInput;

	public float MicGain
	{
		get { return _micGain; }
	}

	public MicMaster ()
	{
		_micInput = new MicInput ();
	}

	public bool Start()
	{
		//If there is no input device
		if (WaveIn.DeviceCount < 1)
			return false;
		//Begin monitoring on default device
		_micInput.BeginMonitoring (0);
		_micInput.SampleAggregator.FrameCalculated += SampleIn;
        	return true;
	}

	//Get gain whenever there is input
	public void SampleIn(object sender, SampleAvgEventArgs e)
	{
		_micGain = e.AvgSample;
	}
}
