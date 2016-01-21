using UnityEngine;
using System.Collections.Generic;

public class TravelState : State<NPCController>
{
	public Vector3 Destination;
	public string NextState;
	public float Speed = 2;

	private Animator NPCAnim;
	
	public TravelState (NPCController c) : base(c)
	{
		NPCAnim = c.GetComponent<Animator> ();
	}
	
	public TravelState (NPCController c, Vector3 dest) : base(c) 
	{
		Destination = dest;
		NPCAnim = c.GetComponent<Animator> ();
	}
	
	public override void Enter()
	{
		NPCAnim.SetBool ("Moving", true);
	}
	
	public override bool Handle()
	{
		//First, turn towards the destination
		float deg = _client.TurnTo (Destination);
		if (deg > 1 || deg < -1)
			return true;
		//Fake Y
		var pos = _client.transform.position;
		pos.y = Destination.y;

		//Check if arrived at destination yet
		if (Vector3.Distance (pos, Destination) > 1) 
		{
			//Move directly at the destination, ignoring Y
			float y = _client.transform.position.y;
			var newPos = _client.transform.position + (Destination - _client.transform.position).normalized * Speed * Time.deltaTime;
			newPos.y = y;
			_client.transform.position = newPos;
		}
		else 
		{
			NPCAnim.SetBool ("Moving", false);
			_client.NPCStateMachine.Set(NextState);
		}
		return true;
	}
}