using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class RecenterState : State<TitleController>
{
	//Object that tells player to recenter
	private GameObject RecenterText;

	public RecenterState (TitleController c, GameObject r) : base(c) 
	{
		RecenterText = r;
	}

	public override void Enter ()
	{
		RecenterText.SetActive (true);
	}
	
	public override bool Handle()
	{
		//Wait for player to press key and start recenter process
		if (Input.anyKeyDown && RecenterText.activeSelf) 
		{
			_client.StartCoroutine(_client.FadeIn(Recenter));
			//Hide recenter text
			RecenterText.SetActive(false);
		}
		return true;
	}

	public void Recenter()
	{
		InputTracking.Recenter ();
		_client.SetState ("TitleState");
		_client.StartCoroutine(_client.FadeIn(null));
	}
}