using System;
using System.Collections;
using NAudio.Wave;
using NAudio.Mixer;

public class MicInput {
	WaveIn waveIn;
	public readonly SampleAggregator sampleAggregator;
	UnsignedMixerControl volumeControl;
	double desiredVolume = 100;
	RecordingState recordingState;
	WaveFileWriter writer;
	WaveFormat recordingFormat;
	
	public WaveFormat RecordingFormat
	{
		get
		{
			return recordingFormat;
		}
		set
		{
			recordingFormat = value;
			sampleAggregator.NotificationCount = value.SampleRate / 10;
		}
	}
	
	public SampleAggregator SampleAggregator
	{
		get
		{
			return sampleAggregator;
		}
	}

	public MicInput()
	{
		sampleAggregator = new SampleAggregator();
		RecordingFormat = new WaveFormat(44100, 1);
	}

	//Start getting input from a microphone
	public void BeginMonitoring(int recordingDevice)
	{
		//We can't start the recording if it's already started
		if (recordingState != RecordingState.Stopped)
		{
			throw new InvalidOperationException("Can't begin monitoring while we are in this state: " + recordingState.ToString());
		}
		//NAudio microphone input objects
		waveIn = new WaveIn ();
		waveIn.DeviceNumber = recordingDevice;
		waveIn.DataAvailable += OnDataAvailable;
		waveIn.WaveFormat = recordingFormat;
		waveIn.StartRecording();
		GetVolumeControl();

		recordingState = RecordingState.Monitoring;
	}

	//Get the Volume control and set it's volume to desired
	private void GetVolumeControl()
	{
		if (Environment.OSVersion.Version.Major >= 6) // Vista and over
		{
			//Find the microphone volume settings in Windows
			var mixerLine = waveIn.GetMixerLine();
			foreach (var control in mixerLine.Controls)
			{
				if (control.ControlType == MixerControlType.Volume)
				{
					//Set the microphone volume to desired volume
					volumeControl = control as UnsignedMixerControl;
					volumeControl.Percent = desiredVolume;
					break;
				}
			}
		}
		else
		{
			throw new SystemException("This game only supports Windows Vista and higher.");
		}
	}

	//Function to respond when there is data from the microphone
	void OnDataAvailable(object sender, WaveInEventArgs e)
	{
		//Get mic input
		byte[] buffer = e.Buffer;
		int bytesRecorded = e.BytesRecorded;
		
		for (int index = 0; index < e.BytesRecorded; index += 2)
		{
			//Read the buffer and convert to a number
			short sample = (short)((buffer[index + 1] << 8) |
			                       buffer[index + 0]);
			//Covert data to a value between 0 and 1
			float sample32 = sample / 32768f;
			//Aggregate mic data
			sampleAggregator.Add(sample32);
		}
	}
}
