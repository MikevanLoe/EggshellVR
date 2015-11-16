using System;
using UnityEngine;

public class FishingMinigame : MonoBehaviour
{
	public bool isRodActive;
	public bool isHookOut;

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

	}

	public void LockMovement ()
	{
		// Code van Bas
	}

	public void SpawnFish ()
	{

	}

	public void MoveFish ()
	{

	}

	public Vector2 Wander()
	{
		//first, add a small random vector to the target’s position (RandomClamped
		//returns a value between -1 and 1)
		System.Random randomX = new System.Random ();
		System.Random randomZ = new System.Random ();

		WanderTarget += new Vector2 (randomX.Next(-1, 2) * WanderJitter,
		                             randomZ.Next(-1, 2) * WanderJitter);
		
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

	}

	public void UseRod ()
	{

	}
}