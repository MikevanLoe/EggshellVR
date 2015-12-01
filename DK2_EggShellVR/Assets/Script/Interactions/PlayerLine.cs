using System;
using UnityEngine;

public class PlayerLine : Interaction
{
	private VoiceResponse _voiceResponse;
	private TextMesh _hintMesh;
	private string _hintText;
	private Action _finished;
	
	public PlayerLine (string hint, float dur, Action finished)
	{
		if (hint == null)
			throw new UnityException ("JSON format error! Player input has no hint.");
		_hintText = hint;
		Duration = dur;
		_finished = finished;

		_hintMesh = GameObject.FindGameObjectWithTag ("Player").GetComponentInChildren<TextMesh>();

		_voiceResponse = GameObject.FindGameObjectWithTag ("GameController").GetComponent<VoiceResponse> ();
	}

	public override void Execute()
	{
		_voiceResponse.StartListening (InputFinished);
		_hintMesh.text = _hintText;
	}
	
	public override void Cancel()
	{
		_hintMesh.text = "";
		_voiceResponse.StopListening ();
	}
	
	public void InputFinished(float length)
	{
		//if the speech length is shorter than minimum
		if (length < Duration) 
		{
			//TODO: Make the NPC be like "Uhh wat lol?"
			Execute (); //Start again
			return;
		}
		_hintMesh.text = "";
		_finished ();
	}
}