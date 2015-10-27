using System;

public class Sentence
{
	public string words;
	public float time;

	public Sentence (string text, float createdTime)
	{
		words = text;
		time = createdTime;
	}
}