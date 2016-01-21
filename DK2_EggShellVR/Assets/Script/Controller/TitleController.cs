using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.VR;

public class TitleController : MonoBehaviour {
	public GameObject RecenterText;
	public Transform Logo;
	public Transform Selection;
	public List<Transform> Menu;
	public IntroScene IntroController;
	public Material FadeMaterial;
	public Transform Center;
	public float RotPerSecond = 7;
	public float Angle;

	private StateMachine<TitleController> _states;
	private float DistanceToCenter;

	void Start () {
		//Instantiate and set all the states
		_states = new StateMachine<TitleController> ();
		RecenterState recenterState = new RecenterState (this, RecenterText);
		TitleState titleState = new TitleState (this, Selection, Menu);
		TransitionState transState = new TransitionState (this, IntroController);
		_states.Add (recenterState);
		_states.Add (titleState);
		_states.Add (transState);

		//If there is a VR device, start the game with a moment for calibration
		if (VRDevice.isPresent)
			_states.Set ("RecenterState");
		else
			_states.Set ("TitleState");

		DistanceToCenter = Vector3.Distance (transform.position, Center.position);
	}
	
	// Let the states handle the update
	void Update () {
		_states.GetCurState ().Handle ();
	}

	public void SetState(string state)
	{
		_states.Set (state);
	}

	void FixedUpdate()
	{
		//Rotate around center
		Angle += Time.deltaTime * RotPerSecond; 									//Increase the angle
		Angle %= 360;
		var dir = Quaternion.AngleAxis(Angle, Vector3.forward) * Vector3.right;		//Get a Vector based on the angle
		dir.z = dir.y;
		dir.y = 0;
		transform.position = Center.position + dir * DistanceToCenter;
		transform.LookAt (Center);
	}

	//Change state to transition
	public void GotoTransition()
	{
		//Hide the title screen
		Selection.gameObject.SetActive (false);
		Menu.ForEach(m => m.gameObject.SetActive(false));
		Logo.gameObject.SetActive (false);
		_states.Set ("TransitionState");
	}
	
	/// <summary>
	/// Fade out the screen to seemlessly recenter the VR
	/// </summary>
	public IEnumerator FadeIn(Action Finished)
	{
		float stepA = 1f * Time.deltaTime;		//Amount of alpha to add every frame
		Color c = FadeMaterial.color;
		
		//Fade to black
		while(c.a < 1f)
		{
			c.a += stepA;
			FadeMaterial.color = c;
			yield return new WaitForEndOfFrame();
		}
		
		//Round off color
		c.a = 1f;
		FadeMaterial.color = c;
		
		if(Finished != null)
			Finished ();
	}
	
	/// <summary>
	/// Fade out the screen to seemlessly recenter the VR
	/// </summary>
	public IEnumerator FadeOut(Action Finished)
	{
		float stepA = 1f * Time.deltaTime;		//Amount of alpha to add every frame
		Color c = FadeMaterial.color;
		
		//Remove fade
		while(c.a > 0)
		{
			c.a -= stepA;
			FadeMaterial.color = c;
			yield return new WaitForEndOfFrame();
		}
		
		//Round off color
		c.a = 0f;
		FadeMaterial.color = c;

		if(Finished != null)
			Finished ();
	}
}
