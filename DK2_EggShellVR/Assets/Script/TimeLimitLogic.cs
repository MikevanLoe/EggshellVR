using System;
using UnityEngine;

public class TimeLimitLogic : MonoBehaviour
{
	public Sentence sentence;

	private float startTime;
	private float takenTime;

	public TimeLimitLogic (Sentence activeSentence)
	{
		sentence = activeSentence;
		startTime = Time.time;
	}

	void Update()
	{
		takenTime = Time.time - startTime;
	}
}