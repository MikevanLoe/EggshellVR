using System;
using UnityEngine;

public class TimeLimitLogic : MonoBehaviour
{
	const float YSizeTimer = 0.1f;
	const float ZSizeTimer = 0.001f;
	const float MaxXTimer = 0.8f;

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