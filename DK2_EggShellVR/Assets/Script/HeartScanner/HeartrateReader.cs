using UnityEngine;
using System;
using System.Collections;
using ANT__Heartrate_Scanner;

public class HeartrateReader : MonoBehaviour {
	const float HRMIN = 60;
	const float HRMAX = 200;

	public bool DummyHR = true;
	public float HRLow;
	public float HRRaise = 0.1f;
	public float HRHighGap = 10;
	public Transform Bar;
	public float MinSize = 0.1f;
	public Material IndicatorMat;

	private float _yScaleFull;
	private float _yPosFull;
	private bool _HRFound;
	private float _filler = 1;

	//Heartrate is shown in inspector for debug
	[SerializeField]
	private float Heartrate;

	// Use this for initialization
	void Start () 
	{
		_yScaleFull = Bar.localScale.y;
		_yPosFull = Bar.localPosition.y;
		//Choose between getting live heart rate data or dummy test data
		if(!DummyHR)
			StartCoroutine ("HeartStart");
		else
			StartCoroutine ("HeartrateSweep");
		_filler = 1 / CalculateMeterScale (HRMAX - HRMIN);
	}

	//Stop the ANT device on application quit
	void OnApplicationQuit() 
	{
		HeartrateReciever.Stop();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//If the HR reciever isn't yet ready, don't update
		if (!_HRFound)
			return;

		float BPM = HeartrateReciever.Heartrate; 	//Get the heartrate from the ANT
		Heartrate = BPM; 							//Save the heartrate to a member so it can be viewed in debug

		HRLow = Mathf.Min (HRLow, BPM); 			//Decrease the HRLow if it's lower than bpm
		HRLow = Mathf.Max (HRLow, HRMIN);			//Don't let HRLow drop below minimum

		//Slowly raise HRLow in case of low peak
		HRLow += HRRaise * Time.deltaTime;

		var factor = CalculateMeterScale (BPM);

		//Transform the Bar
		//Change the color from green to yellow to red depending on heart rate
		if(factor < 0.5f)
			IndicatorMat.color = Color.Lerp (Color.green, Color.yellow, factor * 2);
		else
			IndicatorMat.color = Color.Lerp (Color.yellow, Color.red, (factor - 0.5f) * 2);

		//Make the heart rate bar only expand and retract upwards
		//Further explained in Technical Document
		float YNewScale = _yScaleFull * factor;
		float dev = _yScaleFull * ((1 - 	factor));
		var newPos = Bar.localPosition;
		newPos.y = _yPosFull - dev;
		var newScale = Bar.localScale;
		newScale.y = YNewScale;
		Bar.localPosition = newPos;
		Bar.localScale = newScale;
	}

	float CalculateMeterScale (float BPM)
	{
		//Make the bar raise faster at lower levels
		//Refer to the Technical Design document for explanation
		float HRHigh = BPM + HRHighGap;
		float mid = (HRHigh - HRLow) / 2;
		float cur = BPM - HRLow;
		float x = cur - mid;
		float factor = 1 / mid * x;
		factor = Mathf.Clamp01 (factor);
		factor = Mathf.Max (factor, MinSize);
		//The filler float is used so that when the MAX heartrate is reached, the meter is filled
		factor *= _filler;
		return factor;
	}
	
	IEnumerator HeartStart()
	{
		try
		{
			HeartrateReciever.Start ();
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Failed to connect to the heartrate scanner.\n" +
				"Please make sure there is an ANT+ device available and a heartrate sensor nearby broadcasting heartrate before trying again./n" + 
			                 ex.Message);
		}
		//Since HRLow tracks the minimum, its initial value has to be above the minimum
		HRLow = HeartrateReciever.Heartrate;
		_HRFound = true;
		yield return new WaitForSeconds(0.1f);
	}

	//Sweep the heartrate back and forth between min and max
	IEnumerator HeartrateSweep()
	{
		_HRFound = true;
		bool dir = true;
		HeartrateReciever.Heartrate = (int) HRMIN;
		while (true) 
		{
			if (dir)
				HeartrateReciever.Heartrate ++;
			else
				HeartrateReciever.Heartrate --;
			//When the limit is reached, flip the direction;
			if(HeartrateReciever.Heartrate == HRMAX || HeartrateReciever.Heartrate == HRMIN)
				dir = !dir;
			yield return new WaitForSeconds(0.1f);
		}
	}
}
