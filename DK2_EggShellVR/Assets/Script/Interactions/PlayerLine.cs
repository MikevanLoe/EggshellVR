using System;
using UnityEngine;
using System.Collections;

public class PlayerLine : Interaction
{
	private VoiceResponse _voiceResponse;
	private TextMesh _hintMesh;
	private GameObject _talkIcon;
	private string _hintText;
	private Action _finished;
	private CutsceneController _cutCont;
	
	public PlayerLine (string hint, float dur, Action finished)
	{
		if (hint == null)
			throw new UnityException ("JSON format error! Player input has no hint.");
		_hintText = hint;
		Duration = dur;
		_finished = finished;

		_hintMesh = GameObject.Find ("HintText").GetComponent<TextMesh>();
		_talkIcon = GameObject.Find ("TalkIcon");
		_voiceResponse = GameObject.FindGameObjectWithTag ("GameController").GetComponent<VoiceResponse> ();
		_cutCont = GameObject.FindGameObjectWithTag ("GameController").GetComponent<CutsceneController> ();
	}

	public override void Execute()
	{
		_voiceResponse.StartListening (InputFinished);
		_hintMesh.text = _hintText;
		_hintMesh.color = new Color (0.68f, 0.73f, 1.0f);
		_talkIcon.SetActive(true);
	}
	
	public override void Cancel()
	{
		_hintMesh.text = "";
		_talkIcon.SetActive (false);
		_voiceResponse.StopListening ();
	}
	
	public void InputFinished(float length)
	{
		//if the speech length is shorter than minimum
		if (length < Duration) 
		{
			_cutCont.StartCoroutine(_cutCont.Blink());
			Execute (); //Start again
			return;
		}
		_hintMesh.text = "";
		_talkIcon.SetActive (false);
		_finished ();
	}
}