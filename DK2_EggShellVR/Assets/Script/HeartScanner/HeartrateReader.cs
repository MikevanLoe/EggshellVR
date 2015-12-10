using UnityEngine;
using System;
using System.Collections;
using ANT__Heartrate_Scanner;

public class HeartrateReader : MonoBehaviour {
	const int HRMIN = 60;
	const int HRMAX = 200;

	public bool DummyHR = true;
	public int Heartrate;
	public float HRRaise = 0.1f;
	public float HRHighGap = 10;
	public Transform Bar;
	public float MinSize = 0.1f;
	public Material IndicatorMat;

	private bool _HRFound;
	private float _filler = 1;
	private float _HRLow;

	//Heartrate is shown in inspector for debug

	// Use this for initialization
	void Start () 
	{
		//Choose between getting live heart rate data or dummy test data
		if(!DummyHR)
			StartCoroutine ("HeartStart");
		else
			StartCoroutine ("HeartrateSweep");
		//Set  the heartrate to the distance between maximum and minumum
		Heartrate = HRMAX - HRMIN;
		//The filler ensures that the meter is filled at max. See Technical Documentation.
		_filler = 1 / CalculateMeterScale ();
	}

	//Stop the ANT device on application quit
	void OnApplicationQuit()
	{
		HeartrateReciever.Stop();
	}

	void Update ()
	{
		//If the HR reciever isn't yet ready, don't update
		if (!_HRFound)
			return;

		Heartrate = HeartrateReciever.Heartrate; 	//Get the heartrate from the ANT

		_HRLow = Mathf.Min (_HRLow, Heartrate); 	//Decrease the HRLow if it's higher than bpm
		_HRLow = Mathf.Max (_HRLow, HRMIN);			//Don't let HRLow drop below minimum

		_HRLow += HRRaise * Time.deltaTime;			//Slowly raise HRLow in case of low peak

		var factor = CalculateMeterScale ();

		//Transform the Bar
		//Change the color from green to yellow to red depending on heart rate
		if(factor < 0.5f)
			IndicatorMat.color = Color.Lerp (Color.green, Color.yellow, factor * 2);
		else
			IndicatorMat.color = Color.Lerp (Color.yellow, Color.red, (factor - 0.5f) * 2);
	}

	public float CalculateMeterScale ()
	{
		//Make the bar raise faster at lower levels
		//Refer to the Technical Design document for explanation
		float HRHigh = Heartrate + HRHighGap;
		float mid = (HRHigh - _HRLow) / 2;
		float cur = Heartrate - _HRLow;
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
		_HRLow = HeartrateReciever.Heartrate;
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
