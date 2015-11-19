using System;
using UnityEngine;

public class FishingMinigame : MonoBehaviour
{
	public bool isRodActive;
	public bool isHookOut;
	public Transform rod;
	public Transform hook;
	public Transform spawnZone;
	public Animator anim;

	public float WanderRadius;
	public float WanderDistance;
	public float WanderJitter;
	public Vector2 Heading;
	
	private Vector2 WanderTarget;

	public void Start ()
	{
		LockMovement ();
		SpawnFish ();
	}

	public void Update ()
	{
		// Activate the rod when the right mouse button is pressed
		if(Input.GetButtonDown("Fire2"))
			ActivateRod ();

		// Use the rod when the left mouse button is pressed and the rod is active	
		if(Input.GetButtonDown("Fire1") && isRodActive)
			UseRod ();

		MoveFish ();	
	}

	public void LockMovement ()
	{
		// Code van Bas
	}

	public void SpawnFish ()
	{
		// random position in SpawnZone
	}

	public void MoveFish ()
	{
		// Code die gebruikt maakt van Wander()

	}

	public Vector2 Wander()
	{
		//first, add a small random vector to the target’s position (RandomClamped
		//returns a value between -1 and 1)
		WanderTarget += new Vector2 (UnityEngine.Random.Range(-1, 2) * WanderJitter,
		                             UnityEngine.Random.Range(-1, 2) * WanderJitter);
		
		//reproject this new vector back onto a unit circle
		WanderTarget.Normalize ();
		
		//increase the length of the vector to the same as the radius
		//of the wander circle
		WanderTarget *= WanderRadius;
		
		//move the target into a position WanderDist in front of the agent
		Vector2 WanderForce = Heading + WanderTarget;
		return WanderForce;
	}

	public void ActivateRod ()
	{
		isRodActive = !isRodActive;
		rod.gameObject.SetActive (isRodActive);

		if (!isRodActive)
		{
			isHookOut = true;
			UseRod ();
		}
	}

	public void UseRod ()
	{
		isHookOut = !isHookOut;
		hook.gameObject.SetActive (isHookOut);
	}

	void OnEnable()
	{

	}
}