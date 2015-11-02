using System;
using UnityEngine;

public class TimeLimitLogic : MonoBehaviour
{
	public Sentence sentence;
	public Transform baseTime;
	public Transform takenTime;
	public Transform overTime;

	private float startTime;
	private float activeTime;
	private Sentence zin = new Sentence("Test zin", 5.0f);

	void Start(/*Sentence activeSentence*/)
	{
		sentence = zin; //activeSentence;
		startTime = Time.time;
	}

	void Update()
	{
		activeTime = Time.time - startTime;

		if (activeTime >= 2*sentence.time)
		{

		}
		else if (activeTime >= sentence.time)
		{
			baseTime.gameObject.SetActive (false);
			overTime.gameObject.SetActive (true);
			float xTransform = 0.8f * (((1 / sentence.time) * activeTime) - 1);
			overTime.localScale = new Vector3 (xTransform, 0.1f, 0.001f);
		} 
		else if (activeTime < sentence.time)
		{
			GameObject takenObj = takenTime.gameObject;
			takenObj.SetActive (true);
			float xTransform = 0.8f * ((1 / sentence.time) * activeTime);
			takenTime.localScale = new Vector3 (xTransform, 0.1f, 0.001f);
		}
	}
}