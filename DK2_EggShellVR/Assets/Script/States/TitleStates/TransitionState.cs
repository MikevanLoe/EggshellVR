using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;

public class TransitionState : State<TitleController>
{
	private Transform _target;
	private Transform _mover;
	private IntroScene IntroCont;

	public TransitionState (TitleController c, IntroScene i) : base(c) 
	{
		_target = GameObject.FindGameObjectWithTag("Player").transform;
		_mover = _client.transform.FindChild ("CameraObject");
		IntroCont = i;
		IntroCont.Finish = Transition;
	}

	public override void Enter()
	{
		_client.StartCoroutine(_client.FadeIn(StartIntro));
	}

	public void StartIntro()
	{
		IntroCont.gameObject.SetActive (true);
	}

	public void Transition()
	{
		//Unlock the player controller and turn on the camera
		var rfpc = _target.GetComponent<RigidbodyFirstPersonController> ();
		rfpc.MouseLocked = false;
		rfpc.LockedInput = false;
		_target.GetComponent<PlayerController> ().enabled = true;
		_target.transform.FindChild ("MainCamera").gameObject.SetActive (true);
		Camera.SetupCurrent (_target.transform.FindChild ("MainCamera").GetComponent<Camera> ());
		
		//Turn off the title objects
		_mover.gameObject.SetActive (false);

		_client.StartCoroutine(_client.FadeOut(End));
	}

	public void End()
	{
		_client.gameObject.SetActive (false);
	}
}