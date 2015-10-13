using System;

public class SampleAggregator
{
	public event EventHandler<SampleAvgEventArgs> FrameCalculated;
	public event EventHandler Restart = delegate { };
	float avgValue;
	public int NotificationCount { get; set; }
	int count;

	//Resets the counter and avarage for recording the next frame
	private void Reset()
	{
		avgValue = count = 0;
	}

	//Adds sample to current frame
	public void Add(float value)
	{
		count++;
		//Add the absolute of the current value to the avarage calculation
		avgValue = avgValue + (Math.Abs(value) - avgValue) / count;
		//When a frame is finished
		if (count >= NotificationCount)
		{
			//If anyone is listening to FrameCalculated event
			if (FrameCalculated != null)
			{
				FrameCalculated(this, new SampleAvgEventArgs(avgValue));
			}
			Reset();
		}
	}
}

public class SampleAvgEventArgs : EventArgs
{
	public SampleAvgEventArgs(float avgValue)
	{
		AvgSample = avgValue;
	}
	public float AvgSample { get; private set; }
}