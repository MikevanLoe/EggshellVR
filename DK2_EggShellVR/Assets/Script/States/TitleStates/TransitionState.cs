using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;

public class TransitionState : State<TitleController>
{
	private Transform _target;
	private Transform _mover;
	private float _damping = 3;
	private float _startDist;
	private Quaternion _startRot;
	public bool _started = false;

	public TransitionState (TitleController c) : base(c) 
	{
		_target = GameObject.FindGameObjectWithTag("Player").transform;
		_mover = _client.transform.FindChild ("CameraObject");
	}

	public override void Enter()
	{
		//When transitioning, double rotation speed
		_client.RotPerSecond *= 2f;
	}

	public override bool Handle()
	{
		//Before we can start, make sure the camera isn't behind the mountain
		if (!_started) 
		{
			//Between these angles is the mountain. Only start when NOT behind them.
			if (_client.Angle < 94 || _client.Angle > 163) 
			{
				_mover.SetParent (null);
				_startDist = Vector3.Distance (_mover.position, _target.position);
				_startRot = _mover.rotation;
				_started = true;
			}
			return true;
		}

		//Until the camera is close enough, keep moving towards it
		float dist = Vector3.Distance (_mover.position, _target.position);
		if (dist > 0.2f) 
		{
			
			if(Debug.isDebugBuild)
			{
				//Be INSTANT
				_damping = Time.deltaTime;
			}
			//Damping is used for smooth movement, basically, it moves a set 
			//amount of the distance every second.
			Vector3 dir = (_target.position - _mover.position) / _damping;
			//Because near the end it would move very slow, a fixed amount is added
			_mover.position = _mover.position + (dir + dir.normalized * 2) * Time.deltaTime;
			//Smoothly rotate the camera to be looking at the same direction as the target
			_mover.rotation = Quaternion.Lerp(_startRot, _target.rotation, 1 - 1 / _startDist * dist);

			return true;
		}

		//Unlock the player controller and turn on the camera
		var rfpc = _target.GetComponent<RigidbodyFirstPersonController> ();
		rfpc.MouseLocked = false;
		rfpc.LockedInput = false;
		_target.GetComponent<PlayerController> ().enabled = true;
		_target.transform.FindChild ("MainCamera").gameObject.SetActive (true);
		Camera.SetupCurrent (_target.transform.FindChild ("MainCamera").GetComponent<Camera> ());
		
		//Turn off the title objects
		_mover.gameObject.SetActive (false);
		_client.gameObject.SetActive (false);

		return true;
	}
}