using System;

public class Sentence
{
	public string words;
	public float time;

	public Sentence (string text, float allowedTime)
	{
		words = text;
		time = allowedTime;
	}
}