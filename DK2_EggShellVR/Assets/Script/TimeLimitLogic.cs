using System;
using UnityEngine;

public class TimeLimitLogic : MonoBehaviour
{
	public float YSizeTimer;
	public float ZSizeTimer;
	public float MaxXTimer;

	public float sentence;
	public Transform baseTime;
	public Transform takenTime;
	public Transform overTime;

	private float startTime;
	private Sentence zin = new Sentence("Test zin", 5.0f);

	void Start(/*Sentence activeSentence*/)
	{
		sentence = zin.time; //activeSentence;
		startTime = Time.time;
	}

	void Update()
	{
		float activeTime = Time.time - startTime;

		if (activeTime <= 2*sentence)
		{
			baseTime.gameObject.SetActive (false);
			overTime.gameObject.SetActive (true);
			float xTransform = MaxXTimer * (((1 / sentence) * activeTime) - 1);
			overTime.localScale = new Vector3 (xTransform, YSizeTimer, ZSizeTimer);
		}
		else if (activeTime < sentence)
		{
			GameObject takenObj = takenTime.gameObject;
			takenObj.SetActive (true);
			float xTransform = MaxXTimer * ((1 / sentence) * activeTime);
			takenTime.localScale = new Vector3 (xTransform, YSizeTimer, ZSizeTimer);
		}
	}
}