using System;
using UnityEngine;

public class TimeLimitLogic : MonoBehaviour
{
	public float timeLimit;
	public Transform baseTime;
	public Transform takenTime;
	public Transform overTime;

	private float startTime;

	private float MaxXTimer = 0.8f;
	private Sentence zin = new Sentence("Test zin", 5.0f);

	void Start()
	{
		timeLimit = zin.time;
		startTime = Time.time;
	}

	void Update()
	{
		float activeTime = Time.time - startTime;

		if (activeTime < timeLimit)
		{
			GameObject takenObj = takenTime.gameObject;
			takenObj.SetActive (true);
			float xTransform = MaxXTimer * ((1 / timeLimit) * activeTime);
			takenTime.localScale = new Vector3 (xTransform, takenTime.localScale.y, takenTime.localScale.z);
		}
		else if (activeTime <= 2 * timeLimit)
		{
			baseTime.gameObject.SetActive (false);
			overTime.gameObject.SetActive (true);
			float xTransform = MaxXTimer * (((1 / timeLimit) * activeTime) - 1);
			overTime.localScale = new Vector3 (xTransform, overTime.localScale.y, overTime.localScale.z);
		}
	}
}